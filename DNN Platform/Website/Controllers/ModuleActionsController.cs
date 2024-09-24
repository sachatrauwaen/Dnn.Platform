// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Modules.Html;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Personalization;
    using DotNetNuke.UI;
    using DotNetNuke.UI.Containers;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using Newtonsoft.Json;

    public class ModuleActionsController : Controller
    {
        private readonly List<int> validIDs = new List<int>();
        private ModuleAction actionRoot;

        public ModuleInstanceContext ModuleContext { get; private set; }

        public bool EditMode
        {
            get
            {
                return Personalization.GetUserMode() != PortalSettings.Mode.View;
            }
        }

        protected ModuleAction ActionRoot
        {
            get
            {
                if (this.actionRoot == null)
                {
                    this.actionRoot = new ModuleAction(this.ModuleContext.GetNextActionID(), Localization.GetString("Manage.Text", Localization.GlobalResourceFile), string.Empty, string.Empty, "manage-icn.png");
                }

                return this.actionRoot;
            }
        }

        protected string AdminText
        {
            get { return Localization.GetString("ModuleGenericActions.Action", Localization.GlobalResourceFile); }
        }

        protected string CustomText
        {
            get { return Localization.GetString("ModuleSpecificActions.Action", Localization.GlobalResourceFile); }
        }

        protected string MoveText
        {
            get { return Localization.GetString(ModuleActionType.MoveRoot, Localization.GlobalResourceFile); }
        }

        protected PortalSettings PortalSettings
        {
            get
            {
                return this.ModuleContext.PortalSettings;
            }
        }

        protected string AdminActionsJSON { get; set; }

        protected string CustomActionsJSON { get; set; }

        protected bool DisplayQuickSettings { get; set; }

        protected string Panes { get; set; }

        protected bool SupportsMove { get; set; }

        protected bool SupportsQuickSettings { get; set; }

        protected bool IsShared { get; set; }

        protected string ModuleTitle { get; set; }

        protected ModuleActionCollection Actions
        {
            get
            {
                return this.ModuleContext.Actions;
            }
        }

        protected ModuleActionCollection HtmlModuleActions
        {
            get
            {
                // add the Edit Text action
                var actions = new ModuleActionCollection();
                actions.Add(
                    this.ModuleContext.GetNextActionID(),
                    "Edit"/*Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile)*/,
                    ModuleActionType.AddContent,
                    string.Empty,
                    string.Empty,
                    this.ModuleContext.EditUrl(),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);

                /*
                // get the content
                var objHTML = new HtmlTextController(this.navigationManager);
                var objWorkflow = new WorkflowStateController();
                this.workflowID = objHTML.GetWorkflow(this.ModuleId, this.TabId, this.PortalId).Value;

                HtmlTextInfo objContent = objHTML.GetTopHtmlText(this.ModuleId, false, this.workflowID);
                if (objContent != null)
                {
                    // if content is in the first state
                    if (objContent.StateID == objWorkflow.GetFirstWorkflowStateID(this.workflowID))
                    {
                        // if not direct publish workflow
                        if (objWorkflow.GetWorkflowStates(this.workflowID).Count > 1)
                        {
                            // add publish action
                            actions.Add(
                                this.GetNextActionID(),
                                Localization.GetString("PublishContent.Action", this.LocalResourceFile),
                                ModuleActionType.AddContent,
                                "publish",
                                "grant.gif",
                                string.Empty,
                                true,
                                SecurityAccessLevel.Edit,
                                true,
                                false);
                        }
                    }
                    else
                    {
                        // if the content is not in the last state of the workflow then review is required
                        if (objContent.StateID != objWorkflow.GetLastWorkflowStateID(this.workflowID))
                        {
                            // if the user has permissions to review the content
                            if (WorkflowStatePermissionController.HasWorkflowStatePermission(WorkflowStatePermissionController.GetWorkflowStatePermissions(objContent.StateID), "REVIEW"))
                            {
                                // add approve and reject actions
                                actions.Add(
                                    this.GetNextActionID(),
                                    Localization.GetString("ApproveContent.Action", this.LocalResourceFile),
                                    ModuleActionType.AddContent,
                                    string.Empty,
                                    "grant.gif",
                                    this.EditUrl("action", "approve", "Review"),
                                    false,
                                    SecurityAccessLevel.Edit,
                                    true,
                                    false);
                                actions.Add(
                                    this.GetNextActionID(),
                                    Localization.GetString("RejectContent.Action", this.LocalResourceFile),
                                    ModuleActionType.AddContent,
                                    string.Empty,
                                    "deny.gif",
                                    this.EditUrl("action", "reject", "Review"),
                                    false,
                                    SecurityAccessLevel.Edit,
                                    true,
                                    false);
                            }
                        }
                    }
                }

                // add mywork to action menu
                actions.Add(
                    this.GetNextActionID(),
                    Localization.GetString("MyWork.Action", this.LocalResourceFile),
                    "MyWork.Action",
                    string.Empty,
                    "view.gif",
                    this.EditUrl("MyWork"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);
                */
                return actions;
            }
        }

        public ActionResult Index(ModuleInfo moduleInfo)
        {
            this.ModuleContext = new ModuleInstanceContext(/*new FakeModuleControl()*/) { Configuration = moduleInfo };
            this.OnInit();
            this.OnLoad();

            var viewModel = new ModuleActionsModel
            {
                ModuleContext = moduleInfo,
                SupportsQuickSettings = this.SupportsQuickSettings,
                DisplayQuickSettings = this.DisplayQuickSettings,

                // QuickSettingsModel = this.qu,
                CustomActionsJSON = this.CustomActionsJSON,
                AdminActionsJSON = this.AdminActionsJSON,
                Panes = this.Panes,
                CustomText = this.CustomText,
                AdminText = this.AdminText,
                MoveText = this.MoveText,
                SupportsMove = this.SupportsMove,
                IsShared = this.IsShared,
                ModuleTitle = moduleInfo.ModuleTitle,
            };

            return this.View(viewModel);
        }

        protected string LocalizeString(string key)
        {
            return Localization.GetString(key, Localization.GlobalResourceFile);
        }

        protected void OnInit()
        {
            // base.OnInit(e);
            // this.ID = "ModuleActions";
            // this.actionButton.Click += this.ActionButton_Click;
            MvcJavaScript.RequestRegistration(CommonJs.DnnPlugins);

            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/admin/menus/ModuleActions/ModuleActions.css", FileOrder.Css.ModuleCss);
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/Resources/Shared/stylesheets/dnnicons/css/dnnicon.min.css", FileOrder.Css.ModuleCss);
            MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/admin/menus/ModuleActions/ModuleActions.js");

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }

        protected void OnLoad()
        {
            // base.OnLoad(e);
            this.ActionRoot.Actions.AddRange(this.Actions);

            var moduleSpecificActions = new ModuleAction(this.ModuleContext.GetNextActionID(), Localization.GetString("ModuleSpecificActions.Action", Localization.GlobalResourceFile), string.Empty, string.Empty, string.Empty);

            ModuleActionCollection moduleActions = this.HtmlModuleActions;

            foreach (ModuleAction action in moduleActions)
            {
                if (ModulePermissionController.HasModuleAccess(action.Secure, "CONTENT", this.ModuleContext.Configuration))
                {
                    if (string.IsNullOrEmpty(action.Icon))
                    {
                        action.Icon = "edit.gif";
                    }

                    /*
                    if (action.ID > maxActionId)
                    {
                        maxActionId = action.ID;
                    }
                    */
                    moduleSpecificActions.Actions.Add(action);

                    if (!UIUtilities.IsLegacyUI(this.ModuleContext.ModuleId, action.ControlKey, this.ModuleContext.PortalId) && action.Url.Contains("ctl"))
                    {
                        action.ClientScript = UrlUtils.PopUpUrl(action.Url, null, this.PortalSettings, true, false);
                    }
                }
            }

            if (moduleSpecificActions.Actions.Count > 0)
            {
                this.ActionRoot.Actions.Add(moduleSpecificActions);
            }

            // this.ActionRoot.Actions.AddRange(this.HtmlModuleActions);
            this.AdminActionsJSON = "[]";
            this.CustomActionsJSON = "[]";
            this.Panes = "[]";
            try
            {
                this.SupportsQuickSettings = false;
                this.DisplayQuickSettings = false;
                this.ModuleTitle = this.ModuleContext.Configuration.ModuleTitle;
                var moduleDefinitionId = this.ModuleContext.Configuration.ModuleDefID;
                var quickSettingsControl = ModuleControlController.GetModuleControlByControlKey("QuickSettings", moduleDefinitionId);

                if (quickSettingsControl != null)
                {
                    this.SupportsQuickSettings = true;
                    /*
                    var control = ModuleControlFactory.LoadModuleControl(this.Page, this.ModuleContext.Configuration, "QuickSettings", quickSettingsControl.ControlSrc);
                    control.ID += this.ModuleContext.ModuleId;
                    this.quickSettings.Controls.Add(control);

                    this.DisplayQuickSettings = this.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("QS_FirstLoad", true);
                    ModuleController.Instance.UpdateModuleSetting(this.ModuleContext.ModuleId, "QS_FirstLoad", "False");

                    ClientResourceManager.RegisterScript(this.Page, "~/admin/menus/ModuleActions/dnnQuickSettings.js");
                    */
                }

                if (this.ActionRoot.Visible)
                {
                    // Add Menu Items
                    foreach (ModuleAction rootAction in this.ActionRoot.Actions)
                    {
                        // Process Children
                        var actions = new List<ModuleAction>();
                        foreach (ModuleAction action in rootAction.Actions)
                        {
                            if (action.Visible)
                            {
                                if ((this.EditMode && Globals.IsAdminControl() == false) ||
                                    (action.Secure != SecurityAccessLevel.Anonymous && action.Secure != SecurityAccessLevel.View))
                                {
                                    if (!action.Icon.Contains("://")
                                            && !action.Icon.StartsWith("/")
                                            && !action.Icon.StartsWith("~/"))
                                    {
                                        action.Icon = "~/images/" + action.Icon;
                                    }

                                    if (action.Icon.StartsWith("~/"))
                                    {
                                        action.Icon = Globals.ResolveUrl(action.Icon);
                                    }

                                    actions.Add(action);

                                    if (string.IsNullOrEmpty(action.Url))
                                    {
                                        this.validIDs.Add(action.ID);
                                    }
                                }
                            }
                        }

                        var oSerializer = new JavaScriptSerializer();
                        if (rootAction.Title == Localization.GetString("ModuleGenericActions.Action", Localization.GlobalResourceFile))
                        {
                            this.AdminActionsJSON = oSerializer.Serialize(actions);
                        }
                        else
                        {
                            if (rootAction.Title == Localization.GetString("ModuleSpecificActions.Action", Localization.GlobalResourceFile))
                            {
                                this.CustomActionsJSON = oSerializer.Serialize(actions);
                            }
                            else
                            {
                                this.SupportsMove = actions.Count > 0;
                                this.Panes = oSerializer.Serialize(this.PortalSettings.ActiveTab.Panes);
                            }
                        }
                    }

                    this.IsShared = this.ModuleContext.Configuration.AllTabs
                        || PortalGroupController.Instance.IsModuleShared(this.ModuleContext.ModuleId, PortalController.Instance.GetPortal(this.PortalSettings.PortalId))
                        || TabController.Instance.GetTabsByModuleID(this.ModuleContext.ModuleId).Count > 1;
                }
            }
            catch (Exception exc)
            {
                // Exceptions.ProcessModuleLoadException(this, exc);
                throw exc;
            }
        }
    }
}
