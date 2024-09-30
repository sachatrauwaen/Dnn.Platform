// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Entities.Tabs.TabVersions;
    using DotNetNuke.Framework;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Mvc;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI;
    using DotNetNuke.UI.ControlPanels;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.Skins.Controls;
    using DotNetNuke.UI.Skins.EventListeners;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc.Skins.Controllers;

    public class SkinModel
    {
        private Dictionary<string, PaneModel> panes;

        public SkinModel(DnnPageController page)
        {
            this.Page = page;
        }

        public DnnPageController Page { get; private set; }

        public string SkinSrc { get; set; }

        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.Instance.GetCurrentPortalSettings();
            }
        }

        public Dictionary<string, PaneModel> Panes
        {
            get
            {
                return this.panes ?? (this.panes = new Dictionary<string, PaneModel>());
            }
        }

        public string RazorFile
        {
            get
            {
                return "~" + Path.GetDirectoryName(this.SkinSrc) + "/Views/" + Path.GetFileName(this.SkinSrc).Replace(".ascx", ".cshtml");
            }
        }

        public string SkinPath
        {
            get
            {
                return Path.GetDirectoryName(this.SkinSrc).Replace("\\", "/") + "/";
            }
        }

        public string ControlPanelRazor { get; set; }

        public string PaneCssClass
        {
            get
            {
                if (Globals.IsEditMode())
                {
                    return "dnnSortable";
                }

                return string.Empty;
            }
        }

        public string BodyCssClass
        {
            get
            {
                if (Globals.IsEditMode())
                {
                    return "dnnEditState";
                }

                return string.Empty;
            }
        }

        public static SkinModel GetSkin(DnnPageController page)
        {
            SkinModel skin = null;
            string skinSource = Null.NullString;

            // skin preview
            if (page.Request.QueryString["SkinSrc"] != null)
            {
                skinSource = SkinController.FormatSkinSrc(Globals.QueryStringDecode(page.Request.QueryString["SkinSrc"]) + ".ascx", page.PortalSettings);
                skin = LoadSkin(page, skinSource);
            }

            // load user skin ( based on cookie )
            if (skin == null)
            {
                HttpCookie skinCookie = page.Request.Cookies["_SkinSrc" + page.PortalSettings.PortalId];
                if (skinCookie != null)
                {
                    if (!string.IsNullOrEmpty(skinCookie.Value))
                    {
                        skinSource = SkinController.FormatSkinSrc(skinCookie.Value + ".ascx", page.PortalSettings);
                        skin = LoadSkin(page, skinSource);
                    }
                }
            }

            // load assigned skin
            if (skin == null)
            {
                // DNN-6170 ensure skin value is culture specific
                // skinSource = Globals.IsAdminSkin() ? SkinController.FormatSkinSrc(page.PortalSettings.DefaultAdminSkin, page.PortalSettings) : page.PortalSettings.ActiveTab.SkinSrc;
                skinSource = Globals.IsAdminSkin() ? PortalController.GetPortalSetting("DefaultAdminSkin", page.PortalSettings.PortalId, Host.DefaultPortalSkin, page.PortalSettings.CultureCode) : page.PortalSettings.ActiveTab.SkinSrc;
                if (!string.IsNullOrEmpty(skinSource))
                {
                    skinSource = SkinController.FormatSkinSrc(skinSource, page.PortalSettings);
                    skin = LoadSkin(page, skinSource);
                }
            }

            // error loading skin - load default
            if (skin == null)
            {
                skinSource = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalSkin(), page.PortalSettings);
                skin = LoadSkin(page, skinSource);
            }

            // set skin path
            page.PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath(skinSource);

            // set skin id to an explicit short name to reduce page payload and make it standards compliant
            /*
            skin.ID = "dnn";
            */
            return skin;
        }

        /// <summary>GetPopUpSkin gets the Skin that is used in modal popup.</summary>
        /// <param name="page">The Page.</param>
        /// <returns>A <see cref="Skin"/> instance.</returns>
        public static SkinModel GetPopUpSkin(DnnPageController page)
        {
            SkinModel skin = null;

            // attempt to find and load a popup skin from the assigned skinned source
            string skinSource = Globals.IsAdminSkin() ? SkinController.FormatSkinSrc(page.PortalSettings.DefaultAdminSkin, page.PortalSettings) : page.PortalSettings.ActiveTab.SkinSrc;
            if (!string.IsNullOrEmpty(skinSource))
            {
                skinSource = SkinController.FormatSkinSrc(SkinController.FormatSkinPath(skinSource) + "popUpSkin.ascx", page.PortalSettings);

                if (File.Exists(HttpContext.Current.Server.MapPath(SkinController.FormatSkinSrc(skinSource, page.PortalSettings))))
                {
                    skin = LoadSkin(page, skinSource);
                }
            }

            // error loading popup skin - load default popup skin
            if (skin == null)
            {
                skinSource = Globals.HostPath + "Skins/_default/popUpSkin.ascx";
                skin = LoadSkin(page, skinSource);
            }

            // set skin path
            page.PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath(skinSource);

            // set skin id to an explicit short name to reduce page payload and make it standards compliant
            /*
            skin.ID = "dnn";
            */
            return skin;
        }

        public bool InjectModule(PaneModel pane, ModuleInfo module)
        {
            bool bSuccess = true;

            // try to inject the module into the pane
            try
            {
                if (this.PortalSettings.ActiveTab.TabID == this.PortalSettings.UserTabId || this.PortalSettings.ActiveTab.ParentId == this.PortalSettings.UserTabId)
                {
                    /*
                    var profileModule = this.ModuleControlPipeline.LoadModuleControl(this.Page, module) as IProfileModule;
                    if (profileModule == null || profileModule.DisplayModule)
                    {
                        pane.InjectModule(module);
                    }
                    */
                }
                else
                {
                    pane.InjectModule(module);
                }
            }
            catch (ThreadAbortException)
            {
                // Response.Redirect may called in module control's OnInit method, so it will cause ThreadAbortException, no need any action here.
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        protected void OnInit(DnnPageController page)
        {
            /*
            base.OnInit(e);
            */

            // Load the Panes
            this.LoadPanes();

            // Load the Module Control(s)
            bool success = Globals.IsAdminControl() ? this.ProcessSlaveModule(page) : this.ProcessMasterModules();
            /*
            this.ProcessMasterModules();
            */

            // Load the Control Panel
            this.InjectControlPanel(page);

            /*
            // Register any error messages on the Skin
            if (this.Request.QueryString["error"] != null && Host.ShowCriticalErrors)
            {
                AddPageMessage(this, Localization.GetString("CriticalError.Error"), " ", ModuleMessage.ModuleMessageType.RedError);

                if (UserController.Instance.GetCurrentUserInfo().IsSuperUser)
                {
                    ServicesFramework.Instance.RequestAjaxScriptSupport();
                    ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                    JavaScript.RequestRegistration(CommonJs.jQueryUI);
                    JavaScript.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom);
                    ClientResourceManager.RegisterScript(this.Page, "~/resources/shared/scripts/dnn.logViewer.js");
                }
            }

            if (!TabPermissionController.CanAdminPage() && !success)
            {
                // only display the warning to non-administrators (administrators will see the errors)
                AddPageMessage(this, Localization.GetString("ModuleLoadWarning.Error"), string.Format(Localization.GetString("ModuleLoadWarning.Text"), this.PortalSettings.Email), ModuleMessage.ModuleMessageType.YellowWarning);
            }

            this.InvokeSkinEvents(SkinEventType.OnSkinInit);

            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(OnInitMessage))
            {
                var messageType = ModuleMessage.ModuleMessageType.YellowWarning;
                if (HttpContext.Current.Items.Contains(OnInitMessageType))
                {
                    messageType = (ModuleMessage.ModuleMessageType)Enum.Parse(typeof(ModuleMessage.ModuleMessageType), HttpContext.Current.Items[OnInitMessageType].ToString(), true);
                }

                AddPageMessage(this, string.Empty, HttpContext.Current.Items[OnInitMessage].ToString(), messageType);

                JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            */

            // Process the Panes attributes
            this.ProcessPanes();
        }

        protected void OnLoad()
        {
            // this.InvokeSkinEvents(SkinEventType.OnSkinLoad);
        }

        protected void OnPreRender(DnnPageController page)
        {
            // this.InvokeSkinEvents(SkinEventType.OnSkinPreRender);
            var isSpecialPageMode = UrlUtils.InPopUp() || page.Request.QueryString["dnnprintmode"] == "true";
            if (TabPermissionController.CanAddContentToPage() && Globals.IsEditMode() && !isSpecialPageMode)
            {
                // Register Drag and Drop plugin
                MvcJavaScript.RequestRegistration(CommonJs.DnnPlugins);
                MvcClientResourceManager.RegisterStyleSheet(page.ControllerContext, "~/resources/shared/stylesheets/dnn.dragDrop.css", FileOrder.Css.FeatureCss);
                MvcClientResourceManager.RegisterScript(page.ControllerContext, "~/resources/shared/scripts/dnn.dragDrop.js");

                // Register Client Script
                var sb = new StringBuilder();
                sb.AppendLine(" (function ($) {");
                sb.AppendLine("     $(document).ready(function () {");
                sb.AppendLine("         $('.dnnSortable').dnnModuleDragDrop({");
                sb.AppendLine("             tabId: " + this.PortalSettings.ActiveTab.TabID + ",");
                sb.AppendLine("             draggingHintText: '" + Localization.GetSafeJSString("DraggingHintText", Localization.GlobalResourceFile) + "',");
                sb.AppendLine("             dragHintText: '" + Localization.GetSafeJSString("DragModuleHint", Localization.GlobalResourceFile) + "',");
                sb.AppendLine("             dropHintText: '" + Localization.GetSafeJSString("DropModuleHint", Localization.GlobalResourceFile) + "',");
                sb.AppendLine("             dropTargetText: '" + Localization.GetSafeJSString("DropModuleTarget", Localization.GlobalResourceFile) + "'");
                sb.AppendLine("         });");
                sb.AppendLine("     });");
                sb.AppendLine(" } (jQuery));");

                var script = sb.ToString();
                /*
                if (ScriptManager.GetCurrent(this.Page) != null)
                {
                    // respect MS AJAX
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DragAndDrop", script, true);
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "DragAndDrop", script, true);
                }
                */
                MvcClientAPI.RegisterStartupScript("DragAndDrop", script);
            }
        }

        private static SkinModel LoadSkin(DnnPageController page, string skinPath)
        {
            SkinModel ctlSkin = null;
            try
            {
                string skinSrc = skinPath;
                if (skinPath.IndexOf(Globals.ApplicationPath, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    skinPath = skinPath.Remove(0, Globals.ApplicationPath.Length);
                }

                /*
                ctlSkin = ControlUtilities.LoadControl<Skin>(page, skinPath);
                */
                ctlSkin = new SkinModel(page);

                ctlSkin.SkinSrc = skinSrc;

                // call databind so that any server logic in the skin is executed
                /*
                ctlSkin.DataBind();
                */

                ctlSkin.OnInit(page); // new
                ctlSkin.OnPreRender(page); // new
            }
            catch (Exception exc)
            {
                // could not load user control
                var lex = new PageLoadException("Unhandled error loading page.", exc);
                if (TabPermissionController.CanAdminPage())
                {
                    // only display the error to administrators
                    /*
                    var skinError = (Label)page.FindControl("SkinError");
                    skinError.Text = string.Format(Localization.GetString("SkinLoadError", Localization.GlobalResourceFile), skinPath, page.Server.HtmlEncode(exc.Message));
                    skinError.Visible = true;
                    */
                }

                Exceptions.LogException(lex);
            }

            return ctlSkin;
        }

        private void ProcessPanes()
        {
            foreach (KeyValuePair<string, PaneModel> kvp in this.Panes)
            {
                kvp.Value.ProcessPane();
            }
        }

        private void LoadPanes()
        {
            this.PortalSettings.ActiveTab.Panes.Add("HeaderPane");
            this.PortalSettings.ActiveTab.Panes.Add("ContentPane");
            this.PortalSettings.ActiveTab.Panes.Add("ContentPaneLower");

            /*
            // iterate page controls
            foreach (Control ctlControl in this.Controls)
            {
                var objPaneControl = ctlControl as HtmlContainerControl;

                // Panes must be runat=server controls so they have to have an ID
                if (objPaneControl != null && !string.IsNullOrEmpty(objPaneControl.ID))
                {
                    // load the skin panes
                    switch (objPaneControl.TagName.ToLowerInvariant())
                    {
                        case "td":
                        case "div":
                        case "span":
                        case "p":
                        case "section":
                        case "header":
                        case "footer":
                        case "main":
                        case "article":
                        case "aside":
                            // content pane
                            if (!objPaneControl.ID.Equals("controlpanel", StringComparison.InvariantCultureIgnoreCase))
                            {
                                // Add to the PortalSettings (for use in the Control Panel)
                                this.PortalSettings.ActiveTab.Panes.Add(objPaneControl.ID);

                                // Add to the Panes collection
                                this.Panes.Add(objPaneControl.ID.ToLowerInvariant(), new Pane(objPaneControl));
                            }
                            else
                            {
                                // Control Panel pane
                                this.controlPanel = objPaneControl;
                            }

                            break;
                    }
                }
            }
            */
        }

        private bool ProcessMasterModules()
        {
            bool success = true;
            if (TabPermissionController.CanViewPage())
            {
                // We need to ensure that Content Item exists since in old versions Content Items are not needed for tabs
                this.EnsureContentItemForTab(this.PortalSettings.ActiveTab);

                // Versioning checks.
                if (!TabController.CurrentPage.HasAVisibleVersion)
                {
                    this.HandleAccesDenied(true);
                }

                int urlVersion;
                if (TabVersionUtils.TryGetUrlVersion(out urlVersion))
                {
                    if (!TabVersionUtils.CanSeeVersionedPages())
                    {
                        this.HandleAccesDenied(false);
                        return true;
                    }

                    if (TabVersionController.Instance.GetTabVersions(TabController.CurrentPage.TabID).All(tabVersion => tabVersion.Version != urlVersion))
                    {
                        /*
                        this.Response.Redirect(this.NavigationManager.NavigateURL(this.PortalSettings.ErrorPage404, string.Empty, "status=404"));
                        */
                    }
                }

                // check portal expiry date
                if (!this.CheckExpired())
                {
                    if ((this.PortalSettings.ActiveTab.StartDate < DateTime.Now && this.PortalSettings.ActiveTab.EndDate > DateTime.Now) || TabPermissionController.CanAdminPage() || Globals.IsLayoutMode())
                    {
                        foreach (var objModule in PortalSettingsController.Instance().GetTabModules(this.PortalSettings))
                        {
                            success = this.ProcessModule(objModule);
                        }
                    }
                    else
                    {
                        this.HandleAccesDenied(false);
                    }
                }
                else
                {
                    /*
                    AddPageMessage(
                        this,
                        string.Empty,
                        string.Format(Localization.GetString("ContractExpired.Error"), this.PortalSettings.PortalName, Globals.GetMediumDate(this.PortalSettings.ExpiryDate.ToString(CultureInfo.InvariantCulture)), this.PortalSettings.Email),
                        ModuleMessage.ModuleMessageType.RedError);
                    */
                }
            }
            else
            {
                // If request localized page which haven't complete translate yet, redirect to default language version.
                var redirectUrl = Globals.AccessDeniedURL(Localization.GetString("TabAccess.Error"));

                // Current locale will use default if did'nt find any
                Locale currentLocale = LocaleController.Instance.GetCurrentLocale(this.PortalSettings.PortalId);
                if (this.PortalSettings.ContentLocalizationEnabled &&
                    TabController.CurrentPage.CultureCode != currentLocale.Code)
                {
                    redirectUrl = new LanguageTokenReplace { Language = currentLocale.Code }.ReplaceEnvironmentTokens("[URL]");
                }

                /*
                this.Response.Redirect(redirectUrl, true);
                */
            }

            return success;
        }

        private bool ProcessSlaveModule(DnnPageController page)
        {
            var success = true;
            var key = UIUtilities.GetControlKey();
            var moduleId = UIUtilities.GetModuleId(key);
            var slaveModule = UIUtilities.GetSlaveModule(moduleId, key, this.PortalSettings.ActiveTab.TabID);

            PaneModel pane;
            this.Panes.TryGetValue(Globals.glbDefaultPane.ToLowerInvariant(), out pane);
            if (pane == null)
            {
                this.Panes.Add(Globals.glbDefaultPane.ToLowerInvariant(), new PaneModel(Globals.glbDefaultPane.ToLowerInvariant(), page, this));
                this.Panes.TryGetValue(Globals.glbDefaultPane.ToLowerInvariant(), out pane);
            }

            slaveModule.PaneName = Globals.glbDefaultPane;
            slaveModule.ContainerSrc = this.PortalSettings.ActiveTab.ContainerSrc;
            if (string.IsNullOrEmpty(slaveModule.ContainerSrc))
            {
                slaveModule.ContainerSrc = this.PortalSettings.DefaultPortalContainer;
            }

            slaveModule.ContainerSrc = SkinController.FormatSkinSrc(slaveModule.ContainerSrc, this.PortalSettings);
            slaveModule.ContainerPath = SkinController.FormatSkinPath(slaveModule.ContainerSrc);

            var moduleControl = ModuleControlController.GetModuleControlByControlKey(key, slaveModule.ModuleDefID);
            if (moduleControl != null)
            {
                slaveModule.ModuleControlId = moduleControl.ModuleControlID;
                slaveModule.IconFile = moduleControl.IconFile;

                string permissionKey;
                switch (slaveModule.ModuleControl.ControlSrc)
                {
                    case "Admin/Modules/ModuleSettings.ascx":
                        permissionKey = "MANAGE";
                        break;
                    case "Admin/Modules/Import.ascx":
                        permissionKey = "IMPORT";
                        break;
                    case "Admin/Modules/Export.ascx":
                        permissionKey = "EXPORT";
                        break;
                    default:
                        permissionKey = "CONTENT";
                        break;
                }

                if (ModulePermissionController.HasModuleAccess(slaveModule.ModuleControl.ControlType, permissionKey, slaveModule))
                {
                    success = this.InjectModule(pane, slaveModule);
                }
                else
                {
                    /*
                    this.Response.Redirect(Globals.AccessDeniedURL(Localization.GetString("ModuleAccess.Error")), true);
                    */
                }
            }

            return success;
        }

        private bool ProcessModule(ModuleInfo module)
        {
            var success = true;
            if (ModuleInjectionManager.CanInjectModule(module, this.PortalSettings))
            {
                // We need to ensure that Content Item exists since in old versions Content Items are not needed for modules
                this.EnsureContentItemForModule(module);

                PaneModel pane = this.GetPane(module);

                if (pane != null)
                {
                    success = this.InjectModule(pane, module);
                }
                else
                {
                    var lex = new ModuleLoadException(Localization.GetString("PaneNotFound.Error"));

                    // this.Controls.Add(new ErrorContainer(this.PortalSettings, MODULELOAD_ERROR, lex).Container);
                    Exceptions.LogException(lex);
                }
            }

            return success;
        }

        private PaneModel GetPane(ModuleInfo module)
        {
            PaneModel pane;
            bool found = this.Panes.TryGetValue(module.PaneName.ToLowerInvariant(), out pane);

            if (!found)
            {
                // this.Panes.TryGetValue(Globals.glbDefaultPane.ToLowerInvariant(), out pane);
                this.Panes.Add(module.PaneName.ToLowerInvariant(), new PaneModel(module.PaneName.ToLowerInvariant(), this.Page, this));
                found = this.Panes.TryGetValue(module.PaneName.ToLowerInvariant(), out pane);
            }

            return pane;
        }

        private void AddPageMessage(SkinModel mvcSkin, string empty, string v, object redError)
        {
            throw new NotImplementedException();
        }

        private void HandleAccesDenied(bool v)
        {
            throw new NotImplementedException();
        }

        private void EnsureContentItemForTab(Entities.Tabs.TabInfo tabInfo)
        {
            // If tab exists but ContentItem not, then we create it
            if (tabInfo.ContentItemId == Null.NullInteger && tabInfo.TabID != Null.NullInteger)
            {
                TabController.Instance.CreateContentItem(tabInfo);
                TabController.Instance.UpdateTab(tabInfo);
            }
        }

        private bool CheckExpired()
        {
            bool blnExpired = false;
            if (this.PortalSettings.ExpiryDate != Null.NullDate)
            {
                if (Convert.ToDateTime(this.PortalSettings.ExpiryDate) < DateTime.Now && !Globals.IsHostTab(this.PortalSettings.ActiveTab.TabID))
                {
                    blnExpired = true;
                }
            }

            return blnExpired;
        }

        private void EnsureContentItemForModule(ModuleInfo module)
        {
            // If module exists but ContentItem not, then we create it
            if (module.ContentItemId == Null.NullInteger && module.ModuleID != Null.NullInteger)
            {
                ModuleController.Instance.CreateContentItem(module);
                ModuleController.Instance.UpdateModule(module);
            }
        }

        private void InjectControlPanel(DnnPageController page)
        {
            // if querystring dnnprintmode=true, controlpanel will not be shown
            if (page.Request.QueryString["dnnprintmode"] != "true" && !UrlUtils.InPopUp() && page.Request.QueryString["hidecommandbar"] != "true")
            {
                // if (Host.AllowControlPanelToDetermineVisibility || (ControlPanelBase.IsPageAdminInternal() || ControlPanelBase.IsModuleAdminInternal()))
                if (ControlPanelBase.IsPageAdminInternal() || ControlPanelBase.IsModuleAdminInternal())
                {
                    // ControlPanel processing
                    this.ControlPanelRazor = Path.GetFileNameWithoutExtension(Host.ControlPanel);

                    /*
                    var controlPanel = ControlUtilities.LoadControl<ControlPanelBase>(this, Host.ControlPanel);
                    var form = (HtmlForm)this.Parent.FindControl("Form");

                    if (controlPanel.IncludeInControlHierarchy)
                    {
                        // inject ControlPanel control into skin
                        if (this.ControlPanel == null || HostController.Instance.GetBoolean("IgnoreControlPanelWrapper", false))
                        {
                            if (form != null)
                            {
                                form.Controls.AddAt(0, controlPanel);
                            }
                            else
                            {
                                this.Page.Controls.AddAt(0, controlPanel);
                            }
                        }
                        else
                        {
                            this.ControlPanel.Controls.Add(controlPanel);
                        }

                        // register admin.css
                        ClientResourceManager.RegisterAdminStylesheet(this.Page, Globals.HostPath + "admin.css");
                    }
                    */
                }
            }
        }
    }
}
