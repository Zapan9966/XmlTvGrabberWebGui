using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XmlTvGrabberWebGui.Data;
using XmlTvGrabberWebGui.Helpers.GlobalProperties;
using XmlTvGrabberWebGui.Helpers.Logger;
using XmlTvGrabberWebGui.Models;

namespace XmlTvGrabberWebGui.Helpers
{
    public class GrabberHelper
    {
        private readonly IGlobalProperties _globals;
        private readonly ILogger _logger;
        private readonly GrabberContext _context;
        private readonly Config _config;

        public GrabberHelper(
            IGlobalProperties globals,
            ILogger logger,
            GrabberContext context,
            Config config)
        {
            _globals = globals;
            _logger = logger;
            _context = context;
            _config = config;
        }

        public string DoWork(Action<int, object> action = null)
        {
            string result = string.Empty;

            if (!Directory.Exists(_globals.TempFolder))
            {
                action?.Invoke(0, "<div>Création du dossier temporaire</div>");
                _logger.LogInformation(AppLogEvents.GrabberCreateTempFolder, $"Création du dossier temporaire");
                Directory.CreateDirectory(_globals.TempFolder);
            }

            foreach (var xmlUrl in _context.XmlUrls.ToList().OrderByDescending(x => x.Index))
            {
                AutoResetEvent reset = new AutoResetEvent(false);
                try
                {
                    // Définition des propriétés globales
                    _globals.CurrentXmlTvUrl = xmlUrl.Url;
                    _globals.FileProcessingId = (_context.Traces.ToList().Where(t => t.Filename == xmlUrl.Url).Max(t => t.FileProcessingId) ?? 0) + 1;

                    action?.Invoke(0, $"<div>Traitement de l'URL: <b class='text-info'>{xmlUrl.Url}</b></div>");
                    string downloadOutput = Path.Combine(_globals.TempFolder, Path.GetFileName(xmlUrl.Url));

                    // *************************************************************************
                    //                      Nettoyage dossier temporaire
                    // *************************************************************************
                    action?.Invoke(0, "<div>Nettoyage du dossier temporaire</div>");
                    _logger.LogInformation(AppLogEvents.GrabberCreateTempFolderCleanUp, $"Nettoyage du dossier temporaire");
                    var di = new DirectoryInfo(_globals.TempFolder);
                    di.GetFiles().ToList().ForEach(f => f.Delete());

                    // *************************************************************************
                    //                      Téléchargement du fichier XML
                    // *************************************************************************
                    action?.Invoke(0, $"<div>Téléchargement du fichier: {xmlUrl.Url}</div>");
                    _logger.LogInformation(AppLogEvents.GrabberFileDownload, $"Téléchargement du fichier: {xmlUrl.Url}");

                    Task.Run(() =>
                    {
                        using var client = new WebClient();
                        client.DownloadProgressChanged += (s, args) => action?.Invoke(args.ProgressPercentage, null);
                        client.DownloadFileCompleted += (s, args) =>
                        {
                            action?.Invoke(0, "");
                            if (args.Error != null)
                            {
                                _logger.LogError(AppLogEvents.GrabberFileDownloadException, args.Error, "Une erreur s'est produite lors du téléchargement");
                                action?.Invoke(0,
                                    $"<div class='text-danger font-weight-bold'>Une erreur s'est produite lors du téléchargement: {args.Error.Message}</div>" +
                                    $"<div class='text-danger font-weight-bold'>Stacktrace: {args.Error.StackTrace}</div>");
                            }
                            reset.Set();
                        };
                        client.DownloadFileTaskAsync(new Uri(xmlUrl.Url), downloadOutput);
                    });
                    // Attente de la fin du téléchargement
                    reset.WaitOne();

                    // Si le téléchargement a réussi
                    if (File.Exists(downloadOutput) && new FileInfo(downloadOutput).Length > 0)
                    {
                        // *************************************************************************
                        //                      Décompression du fichier
                        // *************************************************************************
                        if (Path.GetExtension(downloadOutput) == ".zip")
                        {
                            action?.Invoke(0, $"<div>Extraction de l'archive</div>");
                            _logger.LogInformation(AppLogEvents.GrabberExtractZip, $"Extraction de l'archive zip");
                            ZipFile.ExtractToDirectory(downloadOutput, _globals.TempFolder, true);
                        }
                        else if (Path.GetExtension(downloadOutput) == ".gz")
                        {
                            action?.Invoke(0, $"<div>Extraction de l'archive</div>");
                            _logger.LogInformation(AppLogEvents.GrabberExtractGZip, $"Extraction de l'archive gzip");
                            var file = new FileInfo(downloadOutput);

                            using var fs = file.OpenRead();
                            string currentFileName = file.FullName;
                            string newFileName = currentFileName.Remove(currentFileName.Length - file.Extension.Length);

                            if (File.Exists(newFileName))
                                File.Delete(newFileName);

                            using var dfs = File.Create(newFileName);
                            using var ds = new GZipStream(fs, CompressionMode.Decompress);
                            ds.CopyTo(dfs);
                        }
                        else if (Path.GetExtension(downloadOutput) != ".xml")
                        {
                            action?.Invoke(0, $"<div class='text-warning'>Format de fichier non pris en charge</div>");
                            _logger.LogWarning(AppLogEvents.GrabberUnsupportedFileExtension,
                                $"Format de fichier ({Path.GetExtension(downloadOutput)}) non pris en charge");
                        }

                        // *************************************************************************
                        //                  Traitement de tous les fichiers XML
                        // *************************************************************************
                        var xmlFiles = di.GetFiles().Where(f => f.Extension == ".xml");
                        if (xmlFiles.Any())
                        {
                            // S'il y a au moins un fichier qui contient des données
                            var notEmptyFiles = xmlFiles.Where(f => f.Length > 0);
                            if (notEmptyFiles.Any())
                            {
                                // *************************************************************************
                                //                      Chargement des fichiers
                                // *************************************************************************
                                var tvFiles = notEmptyFiles
                                    .Select(x => new { x.Name, TV = x.FullName.Deserialize() })
                                    .Where(t => t.TV != null);

                                // S'il y a au moins un fichier XMLTV valide
                                if (tvFiles.Any())
                                {
                                    // S'il y a au moin un fichier avec des programmes
                                    var tvFilesWithPrograms = tvFiles.Where(t => t.TV.Programmes.Any());
                                    if (tvFilesWithPrograms.Any())
                                    {
                                        foreach (var tvFile in tvFilesWithPrograms)
                                        {
                                            action?.Invoke(0, $"<div>Traitement du fichier: {tvFile.Name}</div>");
                                            _logger.LogInformation(AppLogEvents.GrabberProcessingFile, $"Chargement du fichier: {tvFile.Name}");
                                            var tv = tvFile.TV;

                                            _logger.LogInformation(AppLogEvents.GrabberFilteringCategories, "Filtrage des catégories");
                                            action?.Invoke(0,
                                                $"<div>Nombre de programmes: {tv.Programmes.Count}</div>" +
                                                $"<div>Filtrage des catégories:</div>");

                                            int nMissing = 0;
                                            int nMatch = 0;

                                            List<XmlCategory> newCategories = new List<XmlCategory>();
                                            var xmlCategories = _context.XmlCategories.ToList();
                                            var tvHeadendCategories = _context.TvHeadendCategories.ToList();

                                            // *************************************************************************
                                            //                      Filtrage catégories
                                            // *************************************************************************
                                            int programmesCount = tv.Programmes.Count;
                                            for (int i = 0; i < programmesCount; i++)
                                            {
                                                for (int j = 0; j < tv.Programmes[i].Categories.Count; j++)
                                                {
                                                    var category = tv.Programmes[i].Categories[j];
                                                    string value = category.Value?.ToLowerInvariant() ?? "";

                                                    var xmlCategory = xmlCategories.FirstOrDefault(x => x.Name.ToLowerInvariant() == value);
                                                    bool isTveCategory = tvHeadendCategories.Any(t => t.Name.ToLowerInvariant() == value);
                                                    bool isNewCatgory = newCategories.Any(c => c.Name.ToLowerInvariant() == value);

                                                    // Si la catégorie n'existe pas dans la base de données
                                                    if (xmlCategory == null && !isTveCategory && !isNewCatgory)
                                                    {
                                                        // Ajout de la catégorie XML dans la base de données
                                                        newCategories.Add(new XmlCategory { Name = category.Value ?? "", TvHeadendCategoryId = null });
                                                        nMissing++;
                                                    }
                                                    // Si la catégorie correspond déjà à un catégorie TvHeadend
                                                    else if (isTveCategory)
                                                    {
                                                        nMatch++;
                                                    }
                                                    // S'il n'y a pas de correspondance avec un catégorie Tvheadend
                                                    else if (string.IsNullOrEmpty(xmlCategory?.TvHeadendCategory?.Name))
                                                    {
                                                        nMissing++;
                                                    }
                                                    else
                                                    {
                                                        category.Value = xmlCategory?.TvHeadendCategory?.Name;
                                                        nMatch++;
                                                    }
                                                }
                                                // Mise à jour de la progression
                                                int percent = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(i + 1) / Convert.ToDecimal(tv.Programmes.Count) * 100));
                                                action?.Invoke(percent, null);
                                            }

                                            // Ajout des nouvelles catégories à la base de données
                                            if (newCategories.Any())
                                            {
                                                _logger.LogInformation(AppLogEvents.GrabberInsertMissingCategories, $"Insertion nouvelles catégories: {newCategories.Count}");
                                                _context.XmlCategories.AddRange(newCategories);
                                                _context.SaveChanges();
                                            }

                                            // Affichage du résultat d'intégration
                                            action?.Invoke(0,
                                                $"<ul class='m-0'>" +
                                                $"  <li>Nb programmes: {tv.Programmes.Count}</li>" +
                                                $"  <li>Catégories modifiées: {nMatch}</li>" +
                                                $"  <li>Catégoris non trouvée: {nMissing}</li>" +
                                                $"  <li>Catégorie ajoutées: {newCategories.Count}</li>" +
                                                $"  <li>Programmes sans catégories: {tv.Programmes.Count(p => !p.Categories.Any())}</li>" +
                                                $"</ul>");
                                            _logger.LogInformation(AppLogEvents.GrabberFilteringResult,
                                                $"{tv.Programmes.Count} programmes, {nMatch} modifiées, {nMissing} catégories non trouvées, " +
                                                $"{newCategories.Count} catégories ajoutées, {tv.Programmes.Count(p => !p.Categories.Any())} sans catégorie");

                                            // *************************************************************************
                                            //                      Création du fichier filtré
                                            // *************************************************************************
                                            FileInfo filtredFile = new FileInfo(Path.Combine(_globals.TempFolder, _config.OutputFilename));
                                            if (filtredFile.Exists)
                                            {
                                                string newFilename = $"{filtredFile.Name}_{di.GetFiles().Count(f => f.Name.Contains(filtredFile.Name))}{filtredFile.Extension}";
                                                filtredFile = new FileInfo(Path.Combine(_globals.TempFolder, newFilename));
                                            }
                                            action?.Invoke(0, $"<div>Création du fichier: {filtredFile.Name}</div>");
                                            _logger.LogInformation(AppLogEvents.GrabberCreateOutputFile, $"Création du fichier: {filtredFile.Name}");
                                            tv.Serialize(filtredFile.FullName);

#if !DEBUG
                                            // *************************************************************************
                                            //                      Envoi du fichier à TvHeadend
                                            // *************************************************************************
                                            if (!string.IsNullOrEmpty(_config.SockPath))
                                            {
                                                action?.Invoke(0, $"<div>Connexion au socket: {_config.SockPath}</div>");
                                                _logger.LogInformation(AppLogEvents.GrabberUnixSocketConnection, $"Connexion au socket: {_config.SockPath}");
                                                using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                                                var endPoint = new UnixDomainSocketEndPoint(_config.SockPath);
                                                client.Connect(endPoint);
                                                action?.Invoke(0, $"<div>Envoi du fichier a TvHeadend</div>");
                                                _logger.LogInformation(AppLogEvents.GrabberUnixSocketSend, $"Envoi du fichier a TvHeadend");
                                                client.Send(Encoding.UTF8.GetBytes(File.ReadAllText(filtredFile.FullName)));
                                            }
#endif
                                        }
                                    }
                                    else
                                    {
                                        action?.Invoke(0, $"<div class='text-warning font-weight-bold'>Il n'y a aucun fichier XMLTV contenant des programmes</div>");
                                        _logger.LogWarning(AppLogEvents.GrabberEmptyPrograms, $"Aucun fichier XMLTV contenant des programmes");
                                        result += $"{_globals.CurrentXmlTvUrl} - Il n'y a aucun fichier XMLTV contenant des programmes\r\n";
                                    }
                                }
                                else
                                {
                                    action?.Invoke(0, $"<div class='text-warning font-weight-bold'>Il n'y a aucun fichier XMLTV valides</div>");
                                    _logger.LogWarning(AppLogEvents.GrabberUnsupportedXmlFile, $"Aucun fichier XMLTV valides");
                                    result += $"{_globals.CurrentXmlTvUrl} - Aucun fichier XMLTV contenant des programmes\r\n";
                                }
                            }
                            else
                            {
                                action?.Invoke(0, $"<div class='text-warning font-weight-bold'>Tous les fichier XML sont vides</div>");
                                _logger.LogWarning(AppLogEvents.GrabberXmlFilesEmpty, $"Tous les fichier XML sont vides");
                                result += $"{_globals.CurrentXmlTvUrl} - Tous les fichier XML sont vides\r\n";
                            }
                        }
                        else
                        {
                            action?.Invoke(0, $"<div class='text-warning font-weight-bold'>Aucun fichier XML à traiter</div>");
                            _logger.LogWarning(AppLogEvents.GrabberNoXmlFile, $"Aucun fichier XML à traiter");
                            result += $"{_globals.CurrentXmlTvUrl} - Aucun fichier XML à traiter\r\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(AppLogEvents.GrabberException, ex, "Grabber exception");
                    action?.Invoke(0,
                        $"<div class='text-danger font-weight-bold'>Une erreur s'est produite: {ex.Message}</div>" +
                        $"<div class='text-danger font-weight-bold'>Stacktrace: {ex.StackTrace}</div>");

                    result += $"{_globals.CurrentXmlTvUrl} - Exception: {ex.Message}\r\nStacktrace: {ex.StackTrace}\r\n";
                }
                finally
                {
                    action?.Invoke(0, "<hr/>");
                }
            }
            return result;
        }

    }
}
