// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc.Skins.Controllers;

    public class ContainerModel
    {
        private ModuleInfo moduleConfiguration;
        private ModuleHostModel moduleHost = null;

        public ContainerModel(DnnPageController page, SkinModel skin)
        {
            this.Page = page;
            this.ParentSkin = skin;
        }

        public DnnPageController Page { get; private set; }

        public SkinModel ParentSkin { get; private set; }

        public ModuleHostModel ModuleHost
        {
            get
            {
                return this.moduleHost;
            }
        }

        public IModuleControl ModuleControl
        {
            get
            {
                IModuleControl moduleControl = null;
                if (this.ModuleHost != null)
                {
                    moduleControl = this.ModuleHost.ModuleControl;
                }

                return moduleControl;
            }
        }

        public string ID { get; internal set; }

        public string ContainerPath
        {
            get
            {
                return Path.GetDirectoryName(this.ContainerSrc) + "/";
            }
        }

        public string ContainerSrc { get; internal set; }

        public string ActionName
        {
            get
            {
                if (this.moduleConfiguration.ModuleControl.ControlKey == "Module")
                {
                    return "Index";
                }
                else
                {
                    return string.IsNullOrEmpty(this.ModuleName) ? "Index" : this.FileNameWithoutExtension;
                }
            }
        }

        public string ControllerName
        {
            get
            {
                if (this.moduleConfiguration.ModuleControl.ControlKey == "Module")
                {
                    return "ModuleSettings";
                }
                else
                {
                    return string.IsNullOrEmpty(this.ModuleName) ? this.FileNameWithoutExtension : this.ModuleName;
                }
            }
        }

        public string RazorFile
        {
            get
            {
                return this.moduleConfiguration.ModuleControl.ControlSrc.Replace(".ascx", string.Empty);
            }
        }

        public string ContainerRazorFile
        {
            get
            {
                return "~" + Path.GetDirectoryName(this.ContainerSrc) + "/Views/" + Path.GetFileName(this.ContainerSrc).Replace(".ascx", ".cshtml");
            }
        }

        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return this.moduleConfiguration;
            }
        }

        public bool EditMode { get; internal set; }

        public string Footer { get; private set; }

        public string Header { get; private set; }

        public string ContentPaneCssClass { get; internal set; }

        public string ContentPaneStyle { get; private set; }

        protected PortalSettings PortalSettings
        {
            get
            {
                return PortalController.Instance.GetCurrentPortalSettings();
            }
        }

        private string ModuleName
        {
            get
            {
                return this.moduleConfiguration.DesktopModule.ModuleName;
            }
        }

        private string FileNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.moduleConfiguration.ModuleControl.ControlSrc);
            }
        }

        public void SetModuleConfiguration(ModuleInfo configuration)
        {
            this.moduleConfiguration = configuration;
            this.ProcessModule();
        }

        private void ProcessModule()
        {
            /*
            if (this.tracelLogger.IsDebugEnabled)
            {
                this.tracelLogger.Debug($"Container.ProcessModule Start (TabId:{this.PortalSettings.ActiveTab.TabID},ModuleID: {this.ModuleConfiguration.ModuleDefinition.DesktopModuleID}): Module FriendlyName: '{this.ModuleConfiguration.ModuleDefinition.FriendlyName}')");
            }
            */
            // Process Content Pane Attributes
            this.ProcessContentPane();

            // always add the actions menu as the first item in the content pane.
            /*
            if (this.InjectActionMenu && !ModuleHost.IsViewMode(this.ModuleConfiguration, this.PortalSettings) && this.Request.QueryString["dnnprintmode"] != "true")
            {
                MvcJavaScript.RequestRegistration(CommonJs.DnnPlugins);
                this.ContentPane.Controls.Add(this.LoadControl(this.PortalSettings.DefaultModuleActionMenu));

                // register admin.css
                MvcClientResourceManager.RegisterAdminStylesheet(this.Page, Globals.HostPath + "admin.css");
            }
            */

            // Process Module Header
            this.ProcessHeader();

            // Try to load the module control
            this.moduleHost = new ModuleHostModel(this.ModuleConfiguration, this.ParentSkin, this);
            this.moduleHost.OnPreRender();

            /*
            if (this.tracelLogger.IsDebugEnabled)
            {
                this.tracelLogger.Debug($"Container.ProcessModule Info (TabId:{this.PortalSettings.ActiveTab.TabID},ModuleID: {this.ModuleConfiguration.ModuleDefinition.DesktopModuleID}): ControlPane.Controls.Add(ModuleHost:{this.moduleHost.ID})");
            }

            this.ContentPane.Controls.Add(this.ModuleHost);
            */

            // Process Module Footer
            this.ProcessFooter();

            /*
            // Process the Action Controls
            if (this.ModuleHost != null && this.ModuleControl != null)
            {
                this.ProcessChildControls(this);
            }
            */

            // Add Module Stylesheets
            this.ProcessStylesheets(this.ModuleHost != null);

            /*
            if (this.tracelLogger.IsDebugEnabled)
            {
                this.tracelLogger.Debug($"Container.ProcessModule End (TabId:{this.PortalSettings.ActiveTab.TabID},ModuleID: {this.ModuleConfiguration.ModuleDefinition.DesktopModuleID}): Module FriendlyName: '{this.ModuleConfiguration.ModuleDefinition.FriendlyName}')");
            }
            */
        }

        private void ProcessContentPane()
        {
            this.SetAlignment();

            this.SetBackground();

            this.SetBorder();

            // display visual indicator if module is only visible to administrators
            string viewRoles = this.ModuleConfiguration.InheritViewPermissions
                                   ? TabPermissionController.GetTabPermissions(this.ModuleConfiguration.TabID, this.ModuleConfiguration.PortalID).ToString("VIEW")
                                   : this.ModuleConfiguration.ModulePermissions.ToString("VIEW");

            string pageEditRoles = TabPermissionController.GetTabPermissions(this.ModuleConfiguration.TabID, this.ModuleConfiguration.PortalID).ToString("EDIT");
            string moduleEditRoles = this.ModuleConfiguration.ModulePermissions.ToString("EDIT");

            viewRoles = viewRoles.Replace(";", string.Empty).Trim().ToLowerInvariant();
            pageEditRoles = pageEditRoles.Replace(";", string.Empty).Trim().ToLowerInvariant();
            moduleEditRoles = moduleEditRoles.Replace(";", string.Empty).Trim().ToLowerInvariant();

            var showMessage = false;
            var adminMessage = Null.NullString;
            if (viewRoles.Equals(this.PortalSettings.AdministratorRoleName, StringComparison.InvariantCultureIgnoreCase)
                            && (moduleEditRoles.Equals(this.PortalSettings.AdministratorRoleName, StringComparison.InvariantCultureIgnoreCase)
                                    || string.IsNullOrEmpty(moduleEditRoles))
                            && pageEditRoles.Equals(this.PortalSettings.AdministratorRoleName, StringComparison.InvariantCultureIgnoreCase))
            {
                adminMessage = Localization.GetString("ModuleVisibleAdministrator.Text");
                showMessage = !this.ModuleConfiguration.HideAdminBorder && !Globals.IsAdminControl();
            }

            if (this.ModuleConfiguration.StartDate >= DateTime.Now)
            {
                adminMessage = string.Format(Localization.GetString("ModuleEffective.Text"), this.ModuleConfiguration.StartDate);
                showMessage = !Globals.IsAdminControl();
            }

            if (this.ModuleConfiguration.EndDate <= DateTime.Now)
            {
                adminMessage = string.Format(Localization.GetString("ModuleExpired.Text"), this.ModuleConfiguration.EndDate);
                showMessage = !Globals.IsAdminControl();
            }

            if (showMessage)
            {
                this.AddAdministratorOnlyHighlighting(adminMessage);
            }
        }

        /// <summary>ProcessFooter adds an optional footer (and an End_Module comment)..</summary>
        private void ProcessFooter()
        {
            // inject the footer
            if (!string.IsNullOrEmpty(this.ModuleConfiguration.Footer))
            {
                this.Footer = this.ModuleConfiguration.Footer;
            }

            // inject an end comment around the module content
            if (!Globals.IsAdminControl())
            {
                // this.ContentPane.Controls.Add(new LiteralControl("<!-- End_Module_" + this.ModuleConfiguration.ModuleID + " -->"));
            }
        }

        /// <summary>ProcessHeader adds an optional header (and a Start_Module_ comment)..</summary>
        private void ProcessHeader()
        {
            if (!Globals.IsAdminControl())
            {
                // inject a start comment around the module content
                // this.ContentPane.Controls.Add(new LiteralControl("<!-- Start_Module_" + this.ModuleConfiguration.ModuleID + " -->"));
            }

            // inject the header
            if (!string.IsNullOrEmpty(this.ModuleConfiguration.Header))
            {
                this.Header = this.ModuleConfiguration.Header;
            }
        }

        /// <summary>
        /// ProcessStylesheets processes the Module and Container stylesheets and adds
        /// them to the Page.
        /// </summary>
        private void ProcessStylesheets(bool includeModuleCss)
        {
            MvcClientResourceManager.RegisterStyleSheet(this.Page.ControllerContext, this.ContainerPath + "container.css", FileOrder.Css.ContainerCss);
            MvcClientResourceManager.RegisterStyleSheet(this.Page.ControllerContext, this.ContainerSrc.Replace(".ascx", ".css"), FileOrder.Css.SpecificContainerCss);

            // process the base class module properties
            if (includeModuleCss)
            {
                string controlSrc = this.ModuleConfiguration.ModuleControl.ControlSrc;
                string folderName = this.ModuleConfiguration.DesktopModule.FolderName;

                string stylesheet = string.Empty;
                if (string.IsNullOrEmpty(folderName) == false)
                {
                    if (controlSrc.EndsWith(".mvc"))
                    {
                        stylesheet = Globals.ApplicationPath + "/DesktopModules/MVC/" + folderName.Replace("\\", "/") + "/module.css";
                    }
                    else
                    {
                        stylesheet = Globals.ApplicationPath + "/DesktopModules/" + folderName.Replace("\\", "/") + "/module.css";
                    }

                    MvcClientResourceManager.RegisterStyleSheet(this.Page.ControllerContext, stylesheet, FileOrder.Css.ModuleCss);
                }

                var ix = controlSrc.LastIndexOf("/", StringComparison.Ordinal);
                if (ix >= 0)
                {
                    stylesheet = Globals.ApplicationPath + "/" + controlSrc.Substring(0, ix + 1) + "module.css";
                    MvcClientResourceManager.RegisterStyleSheet(this.Page.ControllerContext, stylesheet, FileOrder.Css.ModuleCss);
                }
            }
        }

        private void SetAlignment()
        {
            if (!string.IsNullOrEmpty(this.ModuleConfiguration.Alignment))
            {
                this.ContentPaneCssClass += " DNNAlign" + this.ModuleConfiguration.Alignment.ToLowerInvariant();
            }
        }

        private void SetBackground()
        {
            if (!string.IsNullOrEmpty(this.ModuleConfiguration.Color))
            {
                this.ContentPaneStyle += "background-color:" + this.ModuleConfiguration.Color + ";";
            }
        }

        private void SetBorder()
        {
            if (!string.IsNullOrEmpty(this.ModuleConfiguration.Border))
            {
                this.ContentPaneStyle += "border:" + string.Format("{0}px #000000 solid", this.ModuleConfiguration.Border) + ";";
            }
        }

        private void AddAdministratorOnlyHighlighting(string message)
        {
            // this.ContentPane.Controls.Add(new LiteralControl(string.Format("<div class=\"dnnFormMessage dnnFormInfo dnnFormInfoAdminErrMssg\">{0}</div>", message)));
        }
    }
}
