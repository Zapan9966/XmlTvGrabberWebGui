using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XmlTvGrabberWebGui.Data;
using XmlTvGrabberWebGui.Helpers;
using XmlTvGrabberWebGui.Helpers.GlobalProperties;
using XmlTvGrabberWebGui.Helpers.Logger;
using XmlTvGrabberWebGui.Helpers.ModelStateSerialization;
using XmlTvGrabberWebGui.Models;
using XmlTvGrabberWebGui.Views.Home;
using XmlTvGrabberWebGui.Views.Shared;

namespace XmlTvGrabberWebGui.Controllers
{
    public class HomeController : Controller
    {
        private readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase | StringComparison.CurrentCultureIgnoreCase;

        private readonly IGlobalProperties _globals;
        private readonly ILogger<HomeController> _logger;
        private readonly GrabberContext _context;

        [TempData]
        public string StatusMessage { get; set; }

        public HomeController(
            IGlobalProperties globals,
            ILogger<HomeController> logger,
            GrabberContext context)
        {
            _globals = globals;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Tableau de bord";

            return View();
        }

        #region Grabber

        public IActionResult Grabber()
        {
            ViewData["Title"] = "Grabber";
            ViewData["AppActivePage"] = AppPages.Grabber;

            return View();
        }

        public IActionResult AutoGrabber()
        {
            _logger.LogInformation(AppLogEvents.GrabberAutoStart, $"Grabber auto-started");

            var config = _context.Configs.FirstOrDefault();
            if (_context.XmlUrls.Any() && !string.IsNullOrEmpty(config?.OutputFilename) && !string.IsNullOrEmpty(config?.SockPath))
            {
                string result = new GrabberHelper(_globals, _logger, _context, config).DoWork();

                // Réinitialisation des propriétés globales
                _globals.ClearUrl();
                _globals.ClearProcessingId();

                return Ok(string.IsNullOrEmpty(result) ? "Work done !" : result);
            }
            else
            {
                string logMessage = "Configuration(s) manquante(s):\r\n";

                if (!_context.XmlUrls.Any())
                    logMessage += " - Aucune URL fichier XMLTV configuré\r\n";

                if (!string.IsNullOrEmpty(config?.OutputFilename))
                    logMessage += " - Nom du fichier de sortie non configuré\r\n";

                if (!string.IsNullOrEmpty(config?.SockPath))
                    logMessage += " - Chemin du socket Unix non configuré\r\n";

                _logger.LogWarning(AppLogEvents.GrabberMissingConfiguration, logMessage);

                return BadRequest(logMessage);
            }
        }

        #endregion

        #region Catégories

        public IActionResult Categories()
        {
            ViewData["Title"] = "Gestion categories";

            return View();
        }

        #region XML

        [ImportModelState]
        [Route("Home/XmlCategories/{showAll:bool?}")]
        public IActionResult XmlCategories(string xmlCategory, string tveCategory, bool? showAll)
        {
            ViewData["Title"] = "Gestion catégories - Affectations";
            ViewData["CategoriesActivePage"] = CategoriesNavClass.Xml;
            ViewData["StatusMessage"] = StatusMessage;

            ViewData["XmlCategory"] = xmlCategory;
            ViewData["TvHeadendCategory"] = tveCategory;

            var model = new XmlCategoriesViewModel
            {
                ShowAll = showAll,
                XmlCategories = _context.XmlCategories.ToList(),
                TvHeadendCategories = _context.TvHeadendCategories
                    .OrderBy(t => t.Group)
                    .ThenBy(t => t.Name)
                    .ToList()
            };

            if (showAll != true)
                model.XmlCategories = model.XmlCategories.Where(x => x.TvHeadendCategoryId == null);

            if (!string.IsNullOrEmpty(xmlCategory))
                model.XmlCategories = model.XmlCategories.Where(f => f.Name.Contains(xmlCategory, _comparison));

            if (!string.IsNullOrEmpty(tveCategory))
                model.XmlCategories = model.XmlCategories.Where(f => f.TvHeadendCategory?.Name.Contains(tveCategory, _comparison) ?? false);

            model.XmlCategories = model.XmlCategories.OrderBy(f => f.Name).ThenBy(f => f.TvHeadendCategory?.Name ?? "");
            model.TvHeadendCategories.Insert(0, new TvHeadendCategory { TvHeadendCategoryId = 0, Name = null, Group = null });

            return View(model);
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        [Route("Home/XmlCategories/{showAll:bool?}")]
        public async Task<IActionResult> XmlCategories(XmlCategory category, string xmlCategory, string tveCategory, bool? showAll)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    StatusMessage = category.XmlCategoryId > 0 ? "Catégorie mise à jour !" : "Catégorie créée avec succés !";

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        category.XmlCategoryId > 0 ? AppLogEvents.XmlCatgoryUpdated : AppLogEvents.XmlCatgoryCreated,
                        category.XmlCategoryId > 0 ? $"Mise à jour catégorie XML: {category}" : $"Création catégorie XML: {category}");
                }
                catch (Exception ex)
                {
                    StatusMessage = "Une erreur s'est produite lors de l'enregistrement";
                    _logger.LogError(AppLogEvents.XmlCatgorySaveError, ex, $"Erreur enregistrement catégorie XML: {category}");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction("XmlCategories", new { xmlCategory, tveCategory, showAll });
        }

        public async Task<IActionResult> DeleteXmlCategory(int id)
        {
            ViewData["Title"] = "Gestion catégories - Affectations";
            ViewData["CategoriesActivePage"] = CategoriesNavClass.Xml;
            ViewData["StatusMessage"] = StatusMessage;

            var category = await _context.XmlCategories.FindAsync(id);
            if (category == null)
            {
                StatusMessage = $"Une erreur s'est produite, impossible de charger la catégorie ID '{id}'";
                _logger.LogError(AppLogEvents.XmlCatgoryMissingId, $"DeleteXmlCategory - Impossible de charger la catégorie ID '{id}'");
                return RedirectToAction("XmlCategories");
            }

            return View(category);
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteXmlCategory(XmlCategory model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.XmlCategories.FindAsync(model.XmlCategoryId);
                if (category != null)
                {
                    _context.XmlCategories.Remove(category);
                    await _context.SaveChangesAsync();

                    StatusMessage = $"Catégorie '{category.Name}' supprimée avec succés !";
                    _logger.LogInformation(AppLogEvents.XmlCatgoryDeleted, $"Catégorie XML supprimée: {category.Name}");
                }
                else
                {
                    StatusMessage = $"Une erreur s'est produite, impossible de charger la catégorie ID '{model.XmlCategoryId}'.";
                    _logger.LogError(AppLogEvents.XmlCatgoryMissingId, $"DeleteXmlCategory - Impossible de charger la catégorie ID '{model.XmlCategoryId}'");
                }
            }
            return RedirectToAction("XmlCategories");
        }

        #endregion

        #region TvHeadend

        [ImportModelState]
        public IActionResult TvHeadendCategories(string group, string name)
        {
            ViewData["Title"] = "Gestion catégories - TvHeadend";
            ViewData["CategoriesActivePage"] = CategoriesNavClass.TvHeadend;
            ViewData["StatusMessage"] = StatusMessage;

            ViewData["Group"] = group;
            ViewData["Name"] = name;

            var categories = _context.TvHeadendCategories.AsEnumerable();

            if (!string.IsNullOrEmpty(group))
                categories = categories.Where(c => c.Group.Contains(group, _comparison));

            if (!string.IsNullOrEmpty(name))
                categories = categories.Where(c => c.Name.Contains(name, _comparison));

            return View(categories.OrderBy(c => c.Group).ThenBy(c => c.Name));
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TvHeadendCategories(TvHeadendCategory category, string searchGroup, string searchName)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    StatusMessage = category.TvHeadendCategoryId > 0 ? "Catégorie mise à jour !" : "Catégorie créée avec succés !";

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        category.TvHeadendCategoryId > 0 ? AppLogEvents.TvHeadendCatgoryUpdated : AppLogEvents.TvHeadendCatgoryCreated,
                        category.TvHeadendCategoryId > 0 ? $"Mise à jour catégorie TvHeadend: {category}" : $"Création catégorie TvHeadend: {category}");
                }
                catch (Exception ex)
                {
                    StatusMessage = "Une erreur s'est produite lors de l'enregistrement.";
                    _logger.LogError(AppLogEvents.TvHeadendCatgorySaveError, ex, $"Erreur enregistrement catégorie TvHeadend: {category}");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction("TvHeadendCategories", new { group = searchGroup, name = searchName });
        }

        public async Task<IActionResult> DeleteTvHeadendCategory(int id)
        {
            ViewData["Title"] = "Gestion catégories - TvHeadend";
            ViewData["CategoriesActivePage"] = CategoriesNavClass.TvHeadend;
            ViewData["StatusMessage"] = StatusMessage;

            var category = await _context.TvHeadendCategories.FindAsync(id);
            if (category == null)
            {
                StatusMessage = $"Une erreur s'est produite, impossible de charger la catégorie ID '{id}'.";
                _logger.LogError(AppLogEvents.TvHeadendCatgoryMissingId, $"DeleteTvHeadendCategory - Impossible de charger la catégorie ID '{id}'");
                return RedirectToAction("TvHeadendCategories");
            }

            return View(category);
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTvHeadendCategory(TvHeadendCategory model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.TvHeadendCategories.FindAsync(model.TvHeadendCategoryId);
                if (category != null)
                {
                    _context.XmlCategories
                        .Where(f => f.TvHeadendCategoryId == category.TvHeadendCategoryId)
                        .ToList()
                        .ForEach(f => f.TvHeadendCategoryId = null);

                    _context.TvHeadendCategories.Remove(category);
                    await _context.SaveChangesAsync();

                    StatusMessage = $"Catégorie '{category.Group} - {category.Name}' supprimée avec succés !";
                    _logger.LogInformation(AppLogEvents.TvHeadendCatgoryDeleted, $"Catégorie TvHeadend supprimée: {category.Group} - {category.Name}");
                }
                else
                {
                    StatusMessage = $"Une erreur s'est produite, impossible de charger la catégorie ID '{model.TvHeadendCategoryId}'";
                    _logger.LogError(AppLogEvents.TvHeadendCatgoryMissingId, $"DeleteTvHeadendCategory - Impossible de charger la catégorie ID '{model.TvHeadendCategoryId}'");
                }
            }
            return RedirectToAction("TvHeadendCategories");
        }

        #endregion

        #endregion

        #region Configuration

        [ImportModelState]
        public IActionResult Configuration()
        {
            ViewData["Title"] = "Configuration";
            ViewData["AppActivePage"] = AppPages.Configuration;
            ViewData["StatusMessage"] = StatusMessage;

            var config = _context.Configs.FirstOrDefault() ?? new Config { ConfigId = 0 };

            return View(config);
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configuration(Config config)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(config);
                    await _context.SaveChangesAsync();
                    StatusMessage = "Configuration mise à jour avec succés";
                    _logger.LogInformation(AppLogEvents.ConfigurationUpdated, "Configuration mise à jour");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    StatusMessage = "Une erreur s'est produite lors de l'enregistrement.";
                    _logger.LogError(AppLogEvents.ConfigurationException, ex, "Une erreur s'est produite lors de l'enregistrement");
                }
            }
            return RedirectToAction("Configuration");
        }

        #endregion

        #region LogViewer

        public IActionResult LogViewer(DateTime? debut, DateTime? fin, LogLevel? logLevel, int? eventId)
        {
            ViewData["Title"] = "Log Viewer";
            ViewData["AppActivePage"] = AppPages.LogViewer;

            ViewData["Debut"] = debut ?? DateTime.Today.AddDays(-6);
            ViewData["Fin"] = fin ?? DateTime.Today;
            ViewData["LogLevel"] = (int?)logLevel;
            ViewData["EventId"] = eventId;

            var logs = _context.Traces.Where(t => t.Date.Date >= (debut ?? DateTime.Today.AddDays(-6)) && t.Date.Date <= (fin ?? DateTime.Today));

            if (logLevel.HasValue)
                logs = logs.Where(t => t.LogLevel == logLevel);

            if (eventId.HasValue)
                logs = logs.Where(t => t.EventId == eventId);

            return View(logs);
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
