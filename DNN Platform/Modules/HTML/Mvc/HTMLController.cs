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
    using DotNetNuke.Modules.Html;
    using DotNetNuke.Modules.Html.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc;
    using DotNetNuke.Web.Mvc.Page;
    using Microsoft.Extensions.DependencyInjection;

    public class HTMLController : ModuleControllerBase
    {
        private readonly INavigationManager navigationManager;
        private readonly HtmlTextController htmlTextController;
        private readonly HtmlTextLogController htmlTextLogController = new HtmlTextLogController();
        private readonly WorkflowStateController workflowStateController = new WorkflowStateController();

        public HTMLController()
        {
            this.navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            this.htmlTextController = new HtmlTextController(this.navigationManager);
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
        public ActionResult XEditHTML(ModuleInfo module)
        {
            var ctrl = new HtmlTextController();

            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = ctrl.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = ctrl.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = System.Web.HttpUtility.HtmlDecode(content.Content);
            return this.View(new HtmlModuleModel()
            {
                Html = html,
            });
        }

        [HttpPost]
        public ActionResult SaveHTML(ModuleInfo module)
        {
            var input = this.Request.Form["Html"];
            var ctrl = new HtmlTextController();

            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = ctrl.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = ctrl.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = System.Web.HttpUtility.HtmlDecode(content.Content);
            return this.View(new HtmlModuleModel()
            {
                Html = html + "/" + input,
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
            model.ModuleID = module.ModuleID;
            model.TabID = module.TabID;
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

                // model.RenderOptions = this.GetRenderOptions();
                model.RedirectUrl = this.navigationManager.NavigateURL();
            }
            catch (Exception exc)
            {
                // Gérer l'exception
                // Exceptions.ProcessModuleLoadException(this, exc);
                throw new Exception("EditHTML", exc);
            }

            MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/Providers/HtmlEditorProviders/DNNConnect.CKE/js/ckeditor/4.18.0/ckeditor.js");

            return this.View(module, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EditHtmlViewModel model)
        {
            try
            {
                int workflowID = this.htmlTextController.GetWorkflow(model.ModuleID, model.TabID, this.PortalSettings.PortalId).Value;
                var htmlContent = this.GetLatestHTMLContent(workflowID, model.ModuleID);

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
                            htmlContent.StateID = htmlContent.StateID == publishedStateID ? draftStateID : publishedStateID;
                        }
                        else
                        {
                            htmlContent.StateID = draftStateID;
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
            // model.CurrentWorkflowInUse = this.LocalizeString(htmlContent.WorkflowName);
            // model.CurrentWorkflowState = this.LocalizeString(htmlContent.StateName);
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
