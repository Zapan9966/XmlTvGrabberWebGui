using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XmlTvGrabberWebGui.Helpers;
using XmlTvGrabberWebGui.Helpers.Logger;
using XmlTvGrabberWebGui.Models;
using XmlTvGrabberWebGui.SerializeModels.Programme;

namespace XmlTvGrabberWebGui.Components.Pages
{
    public partial class Grabber
    {
        private string Progress { get; set; }
        private int ProgressPercent { get; set; }

        private bool IsRunning { get; set; }

        private string ResetVisibility { get; set; } = "display:none;";
        private string ProgressBarDisplay { get; set; } = "none";

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ResetVisibility = null;
                    StateHasChanged();
                }
            }

            base.OnAfterRender(firstRender);
        }

        private void Reset()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            try
            {
                Progress = string.Empty;
                StateHasChanged();

                var config = context.Configs.FirstOrDefault();
                if (!string.IsNullOrEmpty(config?.EpgDatabasePath))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "systemctl",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };

                    ProgressChanged($"<div>Arrêt du service 'tvheadend.service'</div>");
                    logger.LogInformation(AppLogEvents.TvHeadendServiceStop, $"Arrêt du service 'tvheadend.service'");
                    process.StartInfo.Arguments = "stop tvheadend.service";
                    process.Start();
                    ProgressChanged(process.StandardOutput.ReadToEnd());
                    process.WaitForExit();

                    if (File.Exists(config.EpgDatabasePath))
                    {
                        ProgressChanged($"<div>Suppression de la base de données EPG</div>");
                        logger.LogInformation(AppLogEvents.TvHeadendEpgDelete, $"Suppression de la base de données EPG");
                        File.Delete(config.EpgDatabasePath);
                    }
                    else
                    {
                        ProgressChanged($"<div class='text-warning'>Fichier '{config?.EpgDatabasePath}' introuvable</div>");
                        logger.LogWarning(AppLogEvents.ConfigurationEpgNotFound, $"Fichier '{config?.EpgDatabasePath}' introuvable");
                    }

                    ProgressChanged($"<div>Démarrage du service 'tvheadend.service'</div>");
                    logger.LogInformation(AppLogEvents.TvHeadendServiceStart, $"Démarrage du service 'tvheadend.service'");
                    process.StartInfo.Arguments = "start tvheadend.service";
                    process.Start();
                    ProgressChanged(process.StandardOutput.ReadToEnd());
                    process.WaitForExit();
                }
                else
                {
                    ProgressChanged($"<div class='text-warning'>Le chemin de la base de données EPG n'est pas configuré</div>");
                    logger.LogWarning(AppLogEvents.ConfigurationEpgPath, "Chemin base de données EPG non configuré");
                }
            }
            catch (Exception ex)
            {
                ProgressChanged($"<div class='text-danger'><b>Une erreur s'est produite: {ex.Message}</b></div>");
                ProgressChanged($"<div class='text-danger'><b>Stacktrace: {ex.StackTrace}</b></div>");
                logger.LogError(AppLogEvents.TvHeadendEpgResetException, ex, "EPG reset exception");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            Progress = string.Empty;
            StateHasChanged();

            var config = context.Configs.FirstOrDefault();
            if (context.XmlUrls.Any() && !string.IsNullOrEmpty(config?.OutputFilename) && !string.IsNullOrEmpty(config?.SockPath))
            {
                using BackgroundWorker worker = new BackgroundWorker { WorkerReportsProgress = true };
                worker.DoWork += (s, e) => new GrabberHelper(globals, logger, context, config).DoWork(worker.ReportProgress);
                worker.ProgressChanged += (s, e) => 
                {
                    ProgressBarDisplay = e.ProgressPercentage > 0 ? "block" : "none";
                    ProgressPercent = e.ProgressPercentage;
                    Progress += e.UserState?.ToString();
                    StateHasChanged();

                    if (e.ProgressPercentage == 0)
                        jsRuntime.InvokeVoidAsync("onProgressUpdate");
                };
                worker.RunWorkerCompleted += (s, e) => 
                {
                    IsRunning = false;
                    // Réinitialisation des propriétés globales
                    globals.ClearUrl();
                    globals.ClearProcessingId();

                    if (e.Error != null)
                    {
                        logger.LogError(AppLogEvents.GrabberException, e.Error, "Grabber exception");
                        ProgressChanged(
                            $"<div class='text-danger font-weight-bold'>Une erreur s'est produite: {e.Error.Message}</div>" +
                            $"<div class='text-danger' font-weight-bold>Stacktrace: {e.Error.StackTrace}</div>");
                    }
                    else
                    {
                        ProgressChanged($"<div class='text-success font-weight-bold'>Intégration des programmes TV terminé</div>");
                        logger.LogInformation(AppLogEvents.GrabberEnd, "Intégration des programmes TV terminé");
                    }
                };
                worker.RunWorkerAsync();
            }
            else
            {
                string logMessage = "Configuration(s) manquante(s):\r\n";

                if (!context.XmlUrls.Any())
                {
                    ProgressChanged($"<div class='text-warning font-weight-bold'>Aucune URL fichier XMLTV configuré</div>");
                    logMessage += " - Aucune URL fichier XMLTV configuré\r\n";
                }

                if (!string.IsNullOrEmpty(config?.OutputFilename))
                {
                    ProgressChanged($"<div class='text-warning font-weight-bold'>Nom du fichier de sortie non configuré</div>");
                    logMessage += " - Nom du fichier de sortie non configuré\r\n";
                }

                if (!string.IsNullOrEmpty(config?.SockPath))
                {
                    ProgressChanged($"<div class='text-warning font-weight-bold'>Chemin du socket Unix non configuré</div>");
                    logMessage += " - Chemin du socket Unix non configuré\r\n";
                }

                ProgressChanged($"<div class='text-warning font-weight-bold'>Veuillez configurer le grabber: <a href='/Home/Configuration'>Configuration</a></div>");
                logger.LogWarning(AppLogEvents.GrabberMissingConfiguration, logMessage);
            }
        }

        private void ProgressChanged(string progress)
        {
            if (!string.IsNullOrEmpty(progress))
            {
                Progress += progress;
                StateHasChanged();
                jsRuntime.InvokeVoidAsync("onProgressUpdate");
            }
        }
    }
}
