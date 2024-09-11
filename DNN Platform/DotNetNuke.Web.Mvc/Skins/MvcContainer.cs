// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;
    using System.Web;

    using DotNetNuke.Entities.Modules;

    public class MvcContainer
    {
        private ModuleInfo moduleConfiguration;

        public string ID { get; internal set; }

        public string ContainerSrc { get; internal set; }

        public string ActionName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.moduleConfiguration.ModuleControl.ControlSrc);
            }
        }

        public string ControllerName
        {
            get
            {
                return this.moduleConfiguration.DesktopModule.FolderName;
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
                return this.ContainerSrc.Replace(".ascx", string.Empty);
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

            if (this.ContentPane != null)
            {
                // Process Content Pane Attributes
                this.ProcessContentPane();

                // always add the actions menu as the first item in the content pane.
                if (this.InjectActionMenu && !ModuleHost.IsViewMode(this.ModuleConfiguration, this.PortalSettings) && this.Request.QueryString["dnnprintmode"] != "true")
                {
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    this.ContentPane.Controls.Add(this.LoadControl(this.PortalSettings.DefaultModuleActionMenu));

                    // register admin.css
                    ClientResourceManager.RegisterAdminStylesheet(this.Page, Globals.HostPath + "admin.css");
                }

                // Process Module Header
                this.ProcessHeader();

                // Try to load the module control
                this.moduleHost = new ModuleHost(this.ModuleConfiguration, this.ParentSkin, this);
                if (this.tracelLogger.IsDebugEnabled)
                {
                    this.tracelLogger.Debug($"Container.ProcessModule Info (TabId:{this.PortalSettings.ActiveTab.TabID},ModuleID: {this.ModuleConfiguration.ModuleDefinition.DesktopModuleID}): ControlPane.Controls.Add(ModuleHost:{this.moduleHost.ID})");
                }

                this.ContentPane.Controls.Add(this.ModuleHost);

                // Process Module Footer
                this.ProcessFooter();

                // Process the Action Controls
                if (this.ModuleHost != null && this.ModuleControl != null)
                {
                    this.ProcessChildControls(this);
                }

                // Add Module Stylesheets
                this.ProcessStylesheets(this.ModuleHost != null);
            }

            if (this.tracelLogger.IsDebugEnabled)
            {
                this.tracelLogger.Debug($"Container.ProcessModule End (TabId:{this.PortalSettings.ActiveTab.TabID},ModuleID: {this.ModuleConfiguration.ModuleDefinition.DesktopModuleID}): Module FriendlyName: '{this.ModuleConfiguration.ModuleDefinition.FriendlyName}')");
            }
            */
        }
    }
}
