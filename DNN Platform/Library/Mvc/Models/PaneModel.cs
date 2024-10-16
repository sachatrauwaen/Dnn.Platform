﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Personalization;
    using DotNetNuke.UI;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.WebControls;
    using DotNetNuke.Web.Mvc.Skins.Controllers;

    public class PaneModel
    {
        private Dictionary<string, ContainerModel> containers;

        public PaneModel(string name, DnnPageController page, SkinModel skin)
        {
            this.Name = name;
            this.Page = page;
            this.ParentSkin = skin;
        }

        public DnnPageController Page { get; private set; }

        public SkinModel ParentSkin { get; private set; }

        public string CssClass { get; set; }

        public Dictionary<string, ContainerModel> Containers
        {
            get
            {
                return this.containers ?? (this.containers = new Dictionary<string, ContainerModel>());
            }
        }

        protected PortalSettings PortalSettings
        {
            get
            {
                return PortalController.Instance.GetCurrentPortalSettings();
            }
        }

        /// <summary>Gets or sets the name (ID) of the Pane.</summary>
        protected string Name { get; set; }

        public void InjectModule(ModuleInfo module)
        {
            // this.containerWrapperControl = new HtmlGenericControl("div");
            // this.PaneControl.Controls.Add(this.containerWrapperControl);

            // inject module classes
            string classFormatString = "DnnModule DnnModule-{0} DnnModule-{1}";
            string sanitizedModuleName = Null.NullString;

            if (!string.IsNullOrEmpty(module.DesktopModule.ModuleName))
            {
                sanitizedModuleName = Globals.CreateValidClass(module.DesktopModule.ModuleName, false);
            }

            if (this.IsVesionableModule(module))
            {
                classFormatString += " DnnVersionableControl";
            }

            // this.containerWrapperControl.Attributes["class"] = string.Format(classFormatString, sanitizedModuleName, module.ModuleID);
            try
            {
                if (!Globals.IsAdminControl() && (this.PortalSettings.InjectModuleHyperLink || Personalization.GetUserMode() != PortalSettings.Mode.View))
                {
                    // this.containerWrapperControl.Controls.Add(new LiteralControl("<a name=\"" + module.ModuleID + "\"></a>"));
                }

                // Load container control
                ContainerModel container = this.LoadModuleContainer(module);

                // Add Container to Dictionary
                this.Containers.Add(container.ID, container);

                // hide anything of type ActionsMenu - as we're injecting our own menu now.
                /*
                container.InjectActionMenu = container.Controls.OfType<ActionBase>().Count() == 0;
                if (!container.InjectActionMenu)
                {
                    foreach (var actionControl in container.Controls.OfType<IActionControl>())
                    {
                        if (actionControl is ActionsMenu)
                        {
                            Control control = actionControl as Control;
                            if (control != null)
                            {
                                control.Visible = false;
                                container.InjectActionMenu = true;
                            }
                        }
                    }
                }

                if (Globals.IsLayoutMode() && Globals.IsAdminControl() == false)
                {
                    // provide Drag-N-Drop capabilities
                    var dragDropContainer = new Panel();
                    Control title = container.FindControl("dnnTitle");

                    // Assume that the title control is named dnnTitle.  If this becomes an issue we could loop through the controls looking for the title type of skin object
                    dragDropContainer.ID = container.ID + "_DD";
                    this.containerWrapperControl.Controls.Add(dragDropContainer);

                    // inject the container into the page pane - this triggers the Pre_Init() event for the user control
                    dragDropContainer.Controls.Add(container);

                    if (title != null)
                    {
                        if (title.Controls.Count > 0)
                        {
                            title = title.Controls[0];
                        }
                    }

                    // enable drag and drop
                    if (title != null)
                    {
                        // The title ID is actually the first child so we need to make sure at least one child exists
                        DNNClientAPI.EnableContainerDragAndDrop(title, dragDropContainer, module.ModuleID);
                        ClientAPI.RegisterPostBackEventHandler(this.PaneControl, "MoveToPane", this.ModuleMoveToPanePostBack, false);
                    }
                }
                else
                {
                    this.containerWrapperControl.Controls.Add(container);
                    if (Globals.IsAdminControl())
                    {
                        this.containerWrapperControl.Attributes["class"] += " DnnModule-Admin";
                    }
                }
                */

                // Attach Module to Container
                container.SetModuleConfiguration(module);

                // display collapsible page panes
                /*
                if (this.PaneControl.Visible == false)
                {
                    this.PaneControl.Visible = true;
                }
                */
            }
            catch (ThreadAbortException)
            {
                // Response.Redirect may called in module control's OnInit method, so it will cause ThreadAbortException, no need any action here.
            }
            catch (Exception exc)
            {
                var lex = new ModuleLoadException(string.Format(Skin.MODULEADD_ERROR, this.Name), exc);
                /*
                if (TabPermissionController.CanAdminPage())
                {
                    // only display the error to administrators
                    this.containerWrapperControl.Controls.Add(new ErrorContainer(this.PortalSettings, Skin.MODULELOAD_ERROR, lex).Container);
                }

                Exceptions.LogException(exc);
                */
                throw lex;
            }
        }

        public void ProcessPane()
        {
            /*
                // remove excess skin non-validating attributes
                this.PaneControl.Attributes.Remove("ContainerType");
                this.PaneControl.Attributes.Remove("ContainerName");
                this.PaneControl.Attributes.Remove("ContainerSrc");
            */
            if (Globals.IsLayoutMode())
            {
                /*
                this.PaneControl.Visible = true;

                // display pane border
                string cssclass = this.PaneControl.Attributes["class"];
                if (string.IsNullOrEmpty(cssclass))
                {
                    this.PaneControl.Attributes["class"] = CPaneOutline;
                }
                else
                {
                    this.PaneControl.Attributes["class"] = cssclass.Replace(CPaneOutline, string.Empty).Trim().Replace("  ", " ") + " " + CPaneOutline;
                }

                // display pane name
                var ctlLabel = new Label { Text = "<center>" + this.Name + "</center><br />", CssClass = "SubHead" };
                this.PaneControl.Controls.AddAt(0, ctlLabel);
                */
            }
            else
            {
                /*
                if (this.PaneControl.Visible == false && TabPermissionController.CanAddContentToPage())
                {
                    this.PaneControl.Visible = true;
                }*/
                if (this.CanCollapsePane())
                {
                    // This pane has no controls so set the width to 0
                    /*
                    if (this.PaneControl.Attributes["style"] != null)
                    {
                        this.PaneControl.Attributes.Remove("style");
                    }
                    */
                    this.CssClass += " DNNEmptyPane";
                }

                // Add support for drag and drop
                if (Globals.IsEditMode())
                {
                    this.CssClass += " dnnSortable";

                    // this call also checks for permission
                }
            }
        }

        private bool CanCollapsePane()
        {
            // This section sets the width to "0" on panes that have no modules.
            // This preserves the integrity of the HTML syntax so we don't have to set
            // the visiblity of a pane to false. Setting the visibility of a pane to
            // false where there are colspans and rowspans can render the skin incorrectly.
            bool canCollapsePane = true;
            if (this.Containers.Count > 0)
            {
                canCollapsePane = false;
            }

            /*
            else if (this.PaneControl.Controls.Count == 1)
            {
                // Pane contains 1 control
                canCollapsePane = false;
                var literal = this.PaneControl.Controls[0] as LiteralControl;
                if (literal != null)
                {
                    // Check  if the literal control is just whitespace - if so we can collapse panes
                    if (string.IsNullOrEmpty(HtmlUtils.StripWhiteSpace(literal.Text, false)))
                    {
                        canCollapsePane = true;
                    }
                }
            }
            else if (this.PaneControl.Controls.Count > 1)
            {
                // Pane contains more than 1 control
                canCollapsePane = false;
            }
            */
            return canCollapsePane;
        }

        private bool IsVesionableModule(ModuleInfo moduleInfo)
        {
            if (string.IsNullOrEmpty(moduleInfo.DesktopModule.BusinessControllerClass))
            {
                return false;
            }

            object controller = DotNetNuke.Framework.Reflection.CreateObject(moduleInfo.DesktopModule.BusinessControllerClass, string.Empty);
            return controller is IVersionable;
        }

        /// <summary>LoadModuleContainer gets the Container for cookie.</summary>
        /// <param name="request">Current Http Request.</param>
        /// <returns>A Container.</returns>
        private ContainerModel LoadContainerFromCookie(HttpRequest request)
        {
            ContainerModel container = null;
            HttpCookie cookie = request.Cookies["_ContainerSrc" + this.PortalSettings.PortalId];
            if (cookie != null)
            {
                if (!string.IsNullOrEmpty(cookie.Value))
                {
                    // container = this.LoadContainerByPath(SkinController.FormatSkinSrc(cookie.Value + ".ascx", this.PortalSettings));
                }
            }

            return container;
        }

        private ContainerModel LoadModuleContainer(ModuleInfo module)
        {
            var containerSrc = Null.NullString;

            // var request = this.PaneControl.Page.Request;
            ContainerModel container = null;

            if (this.PortalSettings.EnablePopUps && UrlUtils.InPopUp())
            {
                containerSrc = module.ContainerPath + "popUpContainer.ascx";

                // Check Skin for a popup Container
                if (module.ContainerSrc == this.PortalSettings.ActiveTab.ContainerSrc)
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(containerSrc)))
                    {
                        container = this.LoadContainerByPath(containerSrc);
                    }
                }

                // error loading container - load default popup container
                if (container == null)
                {
                    containerSrc = Globals.HostPath + "Containers/_default/popUpContainer.ascx";
                    container = this.LoadContainerByPath(containerSrc);
                }
            }
            else
            {
                /*
                container = (this.LoadContainerFromQueryString(module, request) ?? this.LoadContainerFromCookie(request)) ?? this.LoadNoContainer(module);
                if (container == null)
                {
                    // Check Skin for Container
                    var masterModules = this.PortalSettings.ActiveTab.ChildModules;
                    if (masterModules.ContainsKey(module.ModuleID) && string.IsNullOrEmpty(masterModules[module.ModuleID].ContainerSrc))
                    {
                        // look for a container specification in the skin pane
                        if (this.PaneControl != null)
                        {
                            if (this.PaneControl.Attributes["ContainerSrc"] != null)
                            {
                                container = this.LoadContainerFromPane();
                            }
                        }
                    }
                }
                */
                // else load assigned container
                if (container == null)
                {
                    containerSrc = module.ContainerSrc;
                    if (!string.IsNullOrEmpty(containerSrc))
                    {
                        containerSrc = SkinController.FormatSkinSrc(containerSrc, this.PortalSettings);
                        container = this.LoadContainerByPath(containerSrc);
                    }
                }

                // error loading container - load from tab
                if (container == null)
                {
                    containerSrc = this.PortalSettings.ActiveTab.ContainerSrc;
                    if (!string.IsNullOrEmpty(containerSrc))
                    {
                        containerSrc = SkinController.FormatSkinSrc(containerSrc, this.PortalSettings);
                        container = this.LoadContainerByPath(containerSrc);
                    }
                }

                // error loading container - load default
                if (container == null)
                {
                    containerSrc = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalContainer(), this.PortalSettings);
                    container = this.LoadContainerByPath(containerSrc);
                }
            }

            // Set container path
            module.ContainerPath = SkinController.FormatSkinPath(containerSrc);

            // set container id to an explicit short name to reduce page payload
            container.ID = "ctr";

            // make the container id unique for the page
            if (module.ModuleID > -1)
            {
                container.ID += module.ModuleID.ToString();
            }

            container.EditMode = Personalization.GetUserMode() == PortalSettings.Mode.Edit;

            return container;
        }

        private ContainerModel LoadContainerByPath(string containerPath)
        {
            if (containerPath.IndexOf("/skins/", StringComparison.InvariantCultureIgnoreCase) != -1 || containerPath.IndexOf("/skins\\", StringComparison.InvariantCultureIgnoreCase) != -1 || containerPath.IndexOf("\\skins\\", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                containerPath.IndexOf("\\skins/", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new Exception();
            }

            ContainerModel container = null;

            try
            {
                string containerSrc = containerPath;
                if (containerPath.IndexOf(Globals.ApplicationPath, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    containerPath = containerPath.Remove(0, Globals.ApplicationPath.Length);
                }

                // container = ControlUtilities.LoadControl<MvcContainer>(this.PaneControl.Page, containerPath);
                container = new ContainerModel(this.Page, this.ParentSkin);
                container.ContainerSrc = containerSrc;

                // call databind so that any server logic in the container is executed
                // container.DataBind();
            }
            catch (Exception exc)
            {
                // could not load user control
                var lex = new ModuleLoadException(Skin.MODULELOAD_ERROR, exc);
                if (TabPermissionController.CanAdminPage())
                {
                    // only display the error to administrators
                    /*
                    this.containerWrapperControl.Controls.Add(new ErrorContainer(this.PortalSettings, string.Format(Skin.CONTAINERLOAD_ERROR, containerPath), lex).Container);
                    */
                }

                Exceptions.LogException(lex);
            }

            return container;
        }
    }
}
