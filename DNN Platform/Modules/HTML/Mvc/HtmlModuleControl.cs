// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Modules.Html
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Localization;
    using Microsoft.Extensions.DependencyInjection;

    public class HtmlModuleControl : ModuleControlBase, IActionable
    {
        private readonly INavigationManager navigationManager;

        // private bool editorEnabled;
        private int workflowID;

        public HtmlModuleControl()
        {
            this.navigationManager = this.DependencyProvider.GetRequiredService<INavigationManager>();
            this.ControlPath = "DesktopModules/HTML";
            this.ID = "HtmlModule.ascx";
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                // add the Edit Text action
                var actions = new ModuleActionCollection();
                actions.Add(
                    this.GetNextActionID(),
                    Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile),
                    ModuleActionType.AddContent,
                    string.Empty,
                    string.Empty,
                    this.EditUrl() + "?mvc=yes",
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);

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
                    this.EditUrl("MyWork") + "?mvc=yes",
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);

                return actions;
            }
        }
    }
}
