// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Content.Workflow.Entities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Settings;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Modules.Html;
    using DotNetNuke.Modules.Html.Components;
    using DotNetNuke.Modules.Html.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc;
    using DotNetNuke.Web.Mvc.Page;
    using DotNetNuke.Website.Controllers;
    using DotNetNuke.Website.Models;
    using Microsoft.Extensions.DependencyInjection;

    public class DNN_HTMLController : ModuleSettingsController
    {
        private readonly INavigationManager navigationManager;
        private readonly HtmlTextController htmlTextController;
        private readonly HtmlTextLogController htmlTextLogController = new HtmlTextLogController();
        private readonly WorkflowStateController workflowStateController = new WorkflowStateController();
        private readonly HtmlModuleSettingsRepository settingsRepository;

        public DNN_HTMLController()
        {
            this.navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            this.htmlTextController = new HtmlTextController(this.navigationManager);
            this.settingsRepository = new HtmlModuleSettingsRepository();
        }

        public enum WorkflowType
        {
#pragma warning disable SA1602 // Enumeration items should be documented
            DirectPublish = 1,
#pragma warning restore SA1602 // Enumeration items should be documented
#pragma warning disable SA1602 // Enumeration items should be documented
            ContentStaging = 2,
#pragma warning restore SA1602 // Enumeration items should be documented
        }

        [ChildActionOnly]
        public ActionResult HtmlModule(ModuleInfo module)
        {
            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = this.htmlTextController.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = this.htmlTextController.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = string.Empty;
            if (content != null)
            {
                html = System.Web.HttpUtility.HtmlDecode(content.Content);
            }

            return this.View(module, new HtmlModuleModel()
            {
                Html = html,
            });
        }

        [ChildActionOnly]
        public ActionResult MyWork(ModuleInfo module)
        {
            var objHtmlTextUsers = new HtmlTextUserController();
            var lst = objHtmlTextUsers.GetHtmlTextUser(this.UserInfo.UserID).Cast<HtmlTextUserInfo>();
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/DesktopModules/HTML/edit.css");
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/Portals/_default/Skins/_default/WebControlSkin/Default/GridView.default.css");
            return this.View(module, new MyWorkModel()
            {
                LocalResourceFile = Path.Combine(Path.GetDirectoryName(module.ModuleControl.ControlSrc), Localization.LocalResourceDirectory + "/" + Path.GetFileNameWithoutExtension(module.ModuleControl.ControlSrc)),
                ModuleId = module.ModuleID,
                TabId = module.TabID,
                RedirectUrl = this.navigationManager.NavigateURL(),
                HtmlTextUsers = lst.Select(u => new HtmlTextUserModel()
                {
                    Url = this.navigationManager.NavigateURL(u.TabID),
                    ModuleID = u.ModuleID,
                    ModuleTitle = u.ModuleTitle,
                    StateName = u.StateName,
                }).ToList(),
            });
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult EditHTML(ModuleInfo module)
        {
            var model = new EditHtmlViewModel();

            // model.LocalResourceFile = "/DesktopModules/Html/" + Localization.LocalResourceDirectory + "/EditHTML";
            model.LocalResourceFile = Path.Combine(Path.GetDirectoryName(module.ModuleControl.ControlSrc), Localization.LocalResourceDirectory + "/" + Path.GetFileNameWithoutExtension(module.ModuleControl.ControlSrc));
            model.ShowEditView = true;
            model.ModuleId = module.ModuleID;
            model.TabId = module.TabID;
            model.PortalId = this.PortalSettings.PortalId;
            int workflowID = this.htmlTextController.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            try
            {
                var htmlContentItemID = -1;
                var htmlContent = this.htmlTextController.GetTopHtmlText(module.ModuleID, false, workflowID);

                if (htmlContent != null)
                {
                    htmlContentItemID = htmlContent.ItemID;
                    var html = System.Web.HttpUtility.HtmlDecode(htmlContent.Content);
                    model.EditorContent = html;
                }

                var workflowStates = this.workflowStateController.GetWorkflowStates(workflowID);
                var maxVersions = this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId);

                model.MaxVersions = maxVersions;

                model.WorkflowType = workflowStates.Count == 1 ? WorkflowType.DirectPublish : WorkflowType.ContentStaging;
                if (htmlContentItemID != -1)
                {
                    this.PopulateModelWithContent(model, htmlContent);
                }
                else
                {
                    this.PopulateModelWithInitialContent(model, workflowStates[0] as WorkflowStateInfo);
                }

                model.ShowPublishOption = model.WorkflowType != WorkflowType.DirectPublish;
                model.ShowCurrentVersion = model.WorkflowType != WorkflowType.DirectPublish;
                model.ShowPreviewVersion = model.WorkflowType != WorkflowType.DirectPublish;
                model.ShowHistoryView = false;
                model.ShowMasterContentButton = false;

                // model.RenderOptions = this.GetRenderOptions();
                model.RedirectUrl = this.navigationManager.NavigateURL();
            }
            catch (Exception exc)
            {
                // Gérer l'exception
                // Exceptions.ProcessModuleLoadException(this, exc);
                throw new Exception("EditHTML", exc);
            }

            MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/Resources/Shared/scripts/jquery/jquery.form.min.js");
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/Portals/_default/Skins/_default/WebControlSkin/Default/GridView.default.css");
            MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/DesktopModules/HTML/edit.js");
            return this.View(module, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EditHtmlViewModel model)
        {
            try
            {
                int workflowID = this.htmlTextController.GetWorkflow(model.ModuleId, model.TabId, this.PortalSettings.PortalId).Value;
                var htmlContent = this.GetLatestHTMLContent(workflowID, model.ModuleId);

                htmlContent.Content = model.EditorContent;

                var draftStateID = this.workflowStateController.GetFirstWorkflowStateID(workflowID);
                var publishedStateID = this.workflowStateController.GetLastWorkflowStateID(workflowID);

                switch (model.WorkflowType)
                {
                    case WorkflowType.DirectPublish:
                        this.htmlTextController.UpdateHtmlText(htmlContent, this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId));
                        break;
                    case WorkflowType.ContentStaging:
                        if (model.Publish)
                        {
                            // if it's already published set it to draft
                            if (htmlContent.StateID == publishedStateID)
                            {
                                htmlContent.StateID = draftStateID;
                            }
                            else
                            {
                                htmlContent.StateID = publishedStateID;

                                // here it's in published mode
                            }
                        }
                        else
                        {
                            // if it's already published set it back to draft
                            if (htmlContent.StateID != draftStateID)
                            {
                                htmlContent.StateID = draftStateID;
                            }
                        }

                        this.htmlTextController.UpdateHtmlText(htmlContent, this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId));
                        break;
                }

                return new EmptyResult();
            }
            catch (Exception exc)
            {
                // Gérer l'exception
                // return this.View("Error", new ErrorViewModel { Message = exc.Message });
                throw new Exception(exc.Message, exc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowHistory(EditHtmlViewModel model)
        {
            model.ShowHistoryView = true;
            model.LocalResourceFile = "DesktopModules\\HTML\\App_LocalResources/EditHTML";

            // model.LocalResourceFile = Path.Combine(Path.GetDirectoryName(this.ActiveModule.ModuleControl.ControlSrc), Localization.LocalResourceDirectory + "/" + Path.GetFileNameWithoutExtension(this.ActiveModule.ModuleControl.ControlSrc));
            try
            {
                int workflowID = this.htmlTextController.GetWorkflow(model.ModuleId, model.TabId, this.PortalSettings.PortalId).Value;
                var htmlContent = this.GetLatestHTMLContent(workflowID, model.ModuleId);

                var maxVersions = this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId);
                model.MaxVersions = maxVersions;

                // var htmlLogging = this.htmlTextLogController.GetHtmlTextLog(htmlContent.ItemID);
                var versions = this.htmlTextController.GetAllHtmlText(model.ModuleId);
                model.VersionItems = versions.Cast<HtmlTextInfo>().ToList();

                return this.PartialView(this.ActiveModule, "_History", model);
            }
            catch (Exception exc)
            {
                // Gérer l'exception
                // return this.View("Error", new ErrorViewModel { Message = exc.Message });
                throw new Exception(exc.Message, exc);
            }
        }

        public ActionResult ShowEdit(EditHtmlViewModel model)
        {
            model.ShowHistoryView = true;
            model.LocalResourceFile = "DesktopModules\\HTML\\App_LocalResources/EditHTML";
            try
            {
                int workflowID = this.htmlTextController.GetWorkflow(model.ModuleId, model.TabId, this.PortalSettings.PortalId).Value;
                var htmlContent = this.GetLatestHTMLContent(workflowID, model.ModuleId);

                model.ShowPublishOption = model.WorkflowType != WorkflowType.DirectPublish;
                model.ShowCurrentVersion = model.WorkflowType != WorkflowType.DirectPublish;

                var workflowStates = this.workflowStateController.GetWorkflowStates(workflowID);
                var maxVersions = this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId);

                var htmlContentItemID = -1;

                if (htmlContent != null)
                {
                    htmlContentItemID = htmlContent.ItemID;
                    var html = System.Web.HttpUtility.HtmlDecode(htmlContent.Content);
                    model.EditorContent = html;
                }

                model.MaxVersions = maxVersions;

                model.WorkflowType = workflowStates.Count == 1 ? WorkflowType.DirectPublish : WorkflowType.ContentStaging;
                if (htmlContentItemID != -1)
                {
                    this.PopulateModelWithContent(model, htmlContent);
                }
                else
                {
                    this.PopulateModelWithInitialContent(model, workflowStates[0] as WorkflowStateInfo);
                }

                return this.PartialView(this.ActiveModule, "_Edit", model);
            }
            catch (Exception exc)
            {
                // Gérer l'exception
                // return this.View("Error", new ErrorViewModel { Message = exc.Message });
                throw new Exception(exc.Message, exc);
            }
        }

        public ActionResult HistoryRemove(EditHtmlViewModel model)
        {
            // int workflowID = this.htmlTextController.GetWorkflow(model.ModuleId, model.TabId, this.PortalSettings.PortalId).Value;
            // var htmlContent = this.GetLatestHTMLContent(workflowID, model.ModuleId);
            this.htmlTextController.DeleteHtmlText(model.ModuleId, model.ItemID);

            return this.ShowEdit(model);
        }

        public ActionResult HistoryRollback(EditHtmlViewModel model)
        {
            int workflowID = this.htmlTextController.GetWorkflow(model.ModuleId, model.TabId, this.PortalSettings.PortalId).Value;
            var htmlContent = this.htmlTextController.GetHtmlText(model.ModuleId, model.ItemID);
            htmlContent.ItemID = -1;
            htmlContent.ModuleID = model.ModuleId;
            htmlContent.WorkflowID = workflowID;
            htmlContent.StateID = this.workflowStateController.GetFirstWorkflowStateID(workflowID);
            this.htmlTextController.UpdateHtmlText(htmlContent, this.htmlTextController.GetMaximumVersionHistory(this.PortalSettings.PortalId));
            return this.ShowEdit(model);
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult LoadSettings(int moduleId)
        {
            var moduleSettings = this.settingsRepository.GetSettings(this.ActiveModule);
            var workflow = this.htmlTextController.GetWorkflow(this.ActiveModule.ModuleID, this.ActiveModule.TabID, this.ActiveModule.PortalID);

            var model = new HtmlModuleSettingsModel
            {
                // Assigner les valeurs des paramètres au modèle directement depuis le repository
                ReplaceTokens = moduleSettings.ReplaceTokens,
                UseDecorate = moduleSettings.UseDecorate,
                SearchDescLength = moduleSettings.SearchDescLength,
                Workflows = this.GetWorkflows(), // Récupérer les workflows disponibles
                ApplyTo = workflow.Key,
                SelectedWorkflow = workflow.Value.ToString(),
            };

            return this.PartialView(this.ActiveModule, "LoadSettings", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSettings(HtmlModuleSettingsModel model)
        {
            // if (ModelState.IsValid)
            {
                try
                {
                    // Récupérer les paramètres existants
                    var moduleSettings = this.settingsRepository.GetSettings(this.ActiveModule);

                    // Mettre à jour les paramètres dans le repository
                    moduleSettings.ReplaceTokens = model.ReplaceTokens;
                    moduleSettings.UseDecorate = model.UseDecorate;
                    moduleSettings.SearchDescLength = model.SearchDescLength;

                    // Sauvegarder les paramètres mis à jour
                    this.settingsRepository.SaveSettings(this.ActiveModule, moduleSettings);

                    // Gérer le CacheTime
                    this.UpdateCacheTime(model.ReplaceTokens);

                    // Mettre à jour les workflows selon la sélection
                    this.UpdateWorkflow(model.SelectedWorkflow, model.ApplyTo, model.Replace);
                }
                catch (Exception exc)
                {
                    // Gérer les exceptions
                    // Exceptions.ProcessModuleLoadException(this, exc);
                    throw new Exception("sModuleLoadException", exc);
                }

                return this.UpdateDefaultSettings(model);
            }
        }

        private List<WorkflowStateInfo> GetWorkflows()
        {
            // Récupérer les workflows disponibles
            var workflowStateController = new WorkflowStateController();
            var workflows = workflowStateController.GetWorkflows(this.ActiveModule.PortalID);
            return workflows.Cast<WorkflowStateInfo>().Where(w => !w.IsDeleted).ToList(); // Filtrer les workflows non supprimés
        }

        private void UpdateWorkflow(string selectedWorkflow, string applyTo, bool replace)
        {
            var htmlTextController = new HtmlTextController(this.navigationManager);
            var workflow = this.htmlTextController.GetWorkflow(this.ActiveModule.ModuleID, this.ActiveModule.TabID, this.ActiveModule.PortalID);

            // Mettre à jour le workflow selon la sélection
            switch (applyTo)
            {
                case "Module":
                    htmlTextController.UpdateWorkflow(this.ActiveModule.ModuleID, applyTo, int.Parse(selectedWorkflow), replace);
                    break;
                case "Page":
                    htmlTextController.UpdateWorkflow(this.ActiveModule.TabID, applyTo, int.Parse(selectedWorkflow), replace);
                    break;
                case "Site":
                    htmlTextController.UpdateWorkflow(this.ActiveModule.PortalID, applyTo, int.Parse(selectedWorkflow), replace);
                    break;
            }
        }

        private void UpdateCacheTime(bool replaceTokens)
        {
            // Récupérer le module actuel
            var module = ModuleController.Instance.GetModule(this.ActiveModule.ModuleID, this.ActiveModule.TabID, false);
            if (replaceTokens)
            {
                // Désactiver le cache si ReplaceTokens est activé
                module.CacheTime = 0;
            }
            else
            {
                // Réinitialiser le CacheTime à sa valeur par défaut si nécessaire
                module.CacheTime = 60; // ou toute autre valeur par défaut
            }

            // Mettre à jour le module avec le nouveau CacheTime
            ModuleController.Instance.UpdateModule(module);
        }

        private HtmlTextInfo GetLatestHTMLContent(int workflowID, int moduleId)
        {
            var htmlContent = this.htmlTextController.GetTopHtmlText(moduleId, false, workflowID);
            if (htmlContent == null)
            {
                htmlContent = new HtmlTextInfo();
                htmlContent.ItemID = -1;
                htmlContent.StateID = this.workflowStateController.GetFirstWorkflowStateID(workflowID);
                htmlContent.WorkflowID = workflowID;
                htmlContent.ModuleID = moduleId;
            }

            return htmlContent;
        }

        private void PopulateModelWithContent(EditHtmlViewModel model, HtmlTextInfo htmlContent)
        {
            model.CurrentWorkflowInUse = htmlContent.WorkflowName;
            model.CurrentWorkflowState = htmlContent.StateName;
            model.CurrentVersion = htmlContent.Version.ToString();

            // model.Content = this.FormatContent(htmlContent.Content);
        }

        private void PopulateModelWithInitialContent(EditHtmlViewModel model, WorkflowStateInfo firstState)
        {
            // model.EditorContent = this.LocalizeString("AddContent");
            model.CurrentWorkflowInUse = firstState.WorkflowName;
            model.ShowCurrentWorkflowState = false;
            model.ShowCurrentVersion = false;
        }
    }
}
