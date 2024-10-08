// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;

    using ClientDependency.Core.Mvc;
    using Dnn.EditBar.UI.Mvc;
    using DotNetNuke.Application;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Framework.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Installer.Blocker;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Personalization;
    using DotNetNuke.UI.Internals;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.Utilities;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc.Framework.ActionFilters;
    using DotNetNuke.Web.Mvc.Skins;
    using DotNetNuke.Web.Mvc.Skins.Controllers;

    using Globals = DotNetNuke.Common.Globals;

    public class DefaultController : DnnPageController
    {
        private static readonly Regex HeaderTextRegex = new Regex(
            "<meta([^>])+name=('|\")robots('|\")",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public ActionResult Page(int tabid, string language)
        {
            var antiForgery = string.Empty;
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            if (ServicesFrameworkInternal.Instance.IsAjaxScriptSupportRequired)
            {
                antiForgery = AntiForgery.GetHtml().ToHtmlString();
            }

            JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.jQuery);
            ServicesFrameworkInternal.Instance.RegisterAjaxScript(this.ControllerContext);
            var user = this.PortalSettings.UserInfo;

            MvcContentEditorManager.CreateManager(this);

            var renderer = this.ControllerContext.GetLoader();

            // renderer.RegisterDependency("/Resources/libraries/jQuery/03_07_01/jquery.js", ClientDependency.Core.ClientDependencyType.Javascript);
            var model = new PageModel()
            {
                // EditMode = Personalization.GetUserMode() == PortalSettings.Mode.Edit,
                IsEditMode = Globals.IsEditMode(),
                AntiForgery = antiForgery,
                PortalId = this.PortalSettings?.PortalId,
                TabId = this.PortalSettings?.ActiveTab?.TabID,
                Language = language,
            };
            model.Skin = this.OnInit(model);

            // DotNetNuke.Framework.JavaScriptLibraries.MvcJavaScript.Register(this.ControllerContext);
            model.ClientVariables = MvcClientAPI.GetClientVariableList();
            model.StartupScripts = MvcClientAPI.GetClientStartupScriptList();

            return this.View(model.Skin.RazorFile, "Layout", model);
        }

        /// <summary>Contains the functionality to populate the Root aspx page with controls.</summary>
        /// <param name="page"></param>
        /// <remarks>
        /// - obtain PortalSettings from Current Context
        /// - set global page settings.
        /// - initialise reference paths to load the cascading style sheets
        /// - add skin control placeholder.  This holds all the modules and content of the page.
        /// </remarks>
        /// <returns>skin.</returns>
        protected SkinModel OnInit(PageModel page)
        {
            /*
            base.OnInit(e);
            */

            // set global page settings
            this.InitializePage(page);

            // load skin control and register UI js
            SkinModel ctlSkin;
            if (this.PortalSettings.EnablePopUps)
            {
                ctlSkin = UrlUtils.InPopUp() ? SkinModel.GetPopUpSkin(this) : SkinModel.GetSkin(this);

                // register popup js
                JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.jQueryUI);

                var popupFilePath = System.Web.HttpContext.Current.IsDebuggingEnabled
                                   ? "~/js/Debug/dnn.modalpopup.js"
                                   : "~/js/dnn.modalpopup.js";

                MvcClientResourceManager.RegisterScript(this.ControllerContext, popupFilePath, FileOrder.Js.DnnModalPopup);
            }
            else
            {
                ctlSkin = SkinModel.GetSkin(this);
            }

            /*
            // DataBind common paths for the client resource loader
            this.ClientResourceLoader.DataBind();
            this.ClientResourceLoader.PreRender += (sender, args) => JavaScript.Register(this.Page);

            // check for and read skin package level doctype
            this.SetSkinDoctype();
            */

            /*
            // Manage disabled pages
            if (this.PortalSettings.ActiveTab.DisableLink)
            {
                if (TabPermissionController.CanAdminPage())
                {
                    var heading = Localization.GetString("PageDisabled.Header");
                    var message = Localization.GetString("PageDisabled.Text");

                    UI.Skins.Skin.AddPageMessage(
                        ctlSkin,
                        heading,
                        message,
                        ModuleMessage.ModuleMessageType.YellowWarning);
                }
                else
                {
                    if (this.PortalSettings.HomeTabId > 0)
                    {
                        this.Response.Redirect(this.NavigationManager.NavigateURL(this.PortalSettings.HomeTabId), true);
                    }
                    else
                    {
                        this.Response.Redirect(Globals.GetPortalDomainName(this.PortalSettings.PortalAlias.HTTPAlias, this.Request, true), true);
                    }
                }
            }

            // Manage canonical urls
            if (this.PortalSettings.PortalAliasMappingMode == PortalSettings.PortalAliasMapping.CanonicalUrl)
            {
                string primaryHttpAlias = null;
                if (Config.GetFriendlyUrlProvider() == "advanced")
                {
                    // advanced mode compares on the primary alias as set during alias identification
                    if (this.PortalSettings.PrimaryAlias != null && this.PortalSettings.PortalAlias != null)
                    {
                        if (string.Compare(this.PortalSettings.PrimaryAlias.HTTPAlias, this.PortalSettings.PortalAlias.HTTPAlias, StringComparison.InvariantCulture) != 0)
                        {
                            primaryHttpAlias = this.PortalSettings.PrimaryAlias.HTTPAlias;
                        }
                    }
                }
                else
                {
                    // other modes just depend on the default alias
                    if (string.Compare(this.PortalSettings.PortalAlias.HTTPAlias, this.PortalSettings.DefaultPortalAlias, StringComparison.InvariantCulture) != 0)
                    {
                        primaryHttpAlias = this.PortalSettings.DefaultPortalAlias;
                    }
                }

                if (primaryHttpAlias != null && string.IsNullOrEmpty(this.CanonicalLinkUrl))
                {
                    // a primary http alias was identified
                    var originalurl = this.Context.Items["UrlRewrite:OriginalUrl"].ToString();
                    this.CanonicalLinkUrl = originalurl.Replace(this.PortalSettings.PortalAlias.HTTPAlias, primaryHttpAlias);

                    if (UrlUtils.IsSecureConnectionOrSslOffload(this.Request))
                    {
                        this.CanonicalLinkUrl = this.CanonicalLinkUrl.Replace("http://", "https://");
                    }
                }
            }
            */

            // add CSS links
            MvcClientResourceManager.RegisterDefaultStylesheet(this.ControllerContext, string.Concat(Globals.ApplicationPath, "/Resources/Shared/stylesheets/dnndefault/7.0.0/default.css"));
            MvcClientResourceManager.RegisterIEStylesheet(this.ControllerContext, string.Concat(Globals.HostPath, "ie.css"));

            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, string.Concat(ctlSkin.SkinPath, "skin.css"), FileOrder.Css.SkinCss);
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, ctlSkin.SkinSrc.Replace(".ascx", ".css"), FileOrder.Css.SpecificSkinCss);

            /*
            // add skin to page
            this.SkinPlaceHolder.Controls.Add(ctlSkin);
            */

            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, string.Concat(this.PortalSettings.HomeDirectory, "portal.css"), FileOrder.Css.PortalCss);

            // add Favicon
            this.ManageFavicon(page);
            /*
            // ClientCallback Logic
            ClientAPI.HandleClientAPICallbackEvent(this);

            // add viewstateuserkey to protect against CSRF attacks
            if (this.User.Identity.IsAuthenticated)
            {
                this.ViewStateUserKey = this.User.Identity.Name;
            }

            // set the async postback timeout.
            if (AJAX.IsEnabled())
            {
                AJAX.GetScriptManager(this).AsyncPostBackTimeout = Host.AsyncTimeout;
            }
            */

            return ctlSkin;
        }

        protected bool NonProductionVersion()
        {
            return DotNetNukeContext.Current.Application.Status != ReleaseMode.Stable;
        }

        private void ManageFavicon(PageModel page)
        {
            string headerLink = FavIcon.GetHeaderLink(this.PortalSettings.PortalId);
            page.FavIconLink = headerLink;
            /*
            if (!string.IsNullOrEmpty(headerLink))
            {
                this.Page.Header.Controls.Add(new Literal { Text = headerLink });
            }
            */
        }

        private void InitializePage(PageModel page)
        {
            // There could be a pending installation/upgrade process
            if (InstallBlocker.Instance.IsInstallInProgress())
            {
                Exceptions.ProcessHttpException(new HttpException(503, Localization.GetString("SiteAccessedWhileInstallationWasInProgress.Error", Localization.GlobalResourceFile)));
            }

            // Configure the ActiveTab with Skin/Container information
            PortalSettingsController.Instance().ConfigureActiveTab(this.PortalSettings);
            /*
            // redirect to a specific tab based on name
            if (!string.IsNullOrEmpty(this.Request.QueryString["tabname"]))
            {
                TabInfo tab = TabController.Instance.GetTabByName(this.Request.QueryString["TabName"], this.PortalSettings.PortalId);
                if (tab != null)
                {
                    var parameters = new List<string>(); // maximum number of elements
                    for (int intParam = 0; intParam <= this.Request.QueryString.Count - 1; intParam++)
                    {
                        switch (this.Request.QueryString.Keys[intParam].ToLowerInvariant())
                        {
                            case "tabid":
                            case "tabname":
                                break;
                            default:
                                parameters.Add(
                                    this.Request.QueryString.Keys[intParam] + "=" + this.Request.QueryString[intParam]);
                                break;
                        }
                    }

                    this.Response.Redirect(this.NavigationManager.NavigateURL(tab.TabID, Null.NullString, parameters.ToArray()), true);
                }
                else
                {
                    // 404 Error - Redirect to ErrorPage
                    Exceptions.ProcessHttpException(this.Request);
                }
            }
            */
            string cacheability = this.Request.IsAuthenticated ? Host.AuthenticatedCacheability : Host.UnauthenticatedCacheability;

            switch (cacheability)
            {
                case "0":
                    this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    break;
                case "1":
                    this.Response.Cache.SetCacheability(HttpCacheability.Private);
                    break;
                case "2":
                    this.Response.Cache.SetCacheability(HttpCacheability.Public);
                    break;
                case "3":
                    this.Response.Cache.SetCacheability(HttpCacheability.Server);
                    break;
                case "4":
                    this.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                    break;
                case "5":
                    this.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                    break;
            }

            /*
            // Only insert the header control if a comment is needed
            if (!string.IsNullOrWhiteSpace(this.Comment))
            {
                page.Header.Controls.AddAt(0, new LiteralControl(this.Comment));
            }
            */
            if (this.PortalSettings.ActiveTab.PageHeadText != Null.NullString && !Globals.IsAdminControl())
            {
                page.PageHeadText = this.PortalSettings.ActiveTab.PageHeadText;
            }

            if (!string.IsNullOrEmpty(this.PortalSettings.PageHeadText))
            {
                page.PortalHeadText = this.PortalSettings.PageHeadText;
            }

            // set page title
            if (UrlUtils.InPopUp())
            {
                var strTitle = new StringBuilder(this.PortalSettings.PortalName);
                var slaveModule = UI.UIUtilities.GetSlaveModule(this.PortalSettings.ActiveTab.TabID);

                // Skip is popup is just a tab (no slave module)
                if (slaveModule.DesktopModuleID != Null.NullInteger)
                {
                    var control = ModuleControlFactory.CreateModuleControl(slaveModule) as IModuleControl;
                    string extension = Path.GetExtension(slaveModule.ModuleControl.ControlSrc.ToLowerInvariant());
                    switch (extension)
                    {
                        case ".mvc":
                            var segments = slaveModule.ModuleControl.ControlSrc.Replace(".mvc", string.Empty).Split('/');

                            control.LocalResourceFile = string.Format(
                                "~/DesktopModules/MVC/{0}/{1}/{2}.resx",
                                slaveModule.DesktopModule.FolderName,
                                Localization.LocalResourceDirectory,
                                segments[0]);
                            break;
                        default:
                            control.LocalResourceFile = string.Concat(
                                slaveModule.ModuleControl.ControlSrc.Replace(
                                    Path.GetFileName(slaveModule.ModuleControl.ControlSrc),
                                    string.Empty),
                                Localization.LocalResourceDirectory,
                                "/",
                                Path.GetFileName(slaveModule.ModuleControl.ControlSrc));
                            break;
                    }

                    var title = Localization.LocalizeControlTitle(control);

                    strTitle.Append(string.Concat(" > ", this.PortalSettings.ActiveTab.LocalizedTabName));
                    strTitle.Append(string.Concat(" > ", title));
                }
                else
                {
                    strTitle.Append(string.Concat(" > ", this.PortalSettings.ActiveTab.LocalizedTabName));
                }

                // Set to page
                page.Title = strTitle.ToString();
            }
            else
            {
                // If tab is named, use that title, otherwise build it out via breadcrumbs
                if (!string.IsNullOrEmpty(this.PortalSettings.ActiveTab.Title))
                {
                    page.Title = this.PortalSettings.ActiveTab.Title;
                }
                else
                {
                    // Elected for SB over true concatenation here due to potential for long nesting depth
                    var strTitle = new StringBuilder(this.PortalSettings.PortalName);
                    foreach (TabInfo tab in this.PortalSettings.ActiveTab.BreadCrumbs)
                    {
                        strTitle.Append(string.Concat(" > ", tab.TabName));
                    }

                    page.Title = strTitle.ToString();
                }
            }

            // set the background image if there is one selected
            if (!UrlUtils.InPopUp())
            {
                if (!string.IsNullOrEmpty(this.PortalSettings.BackgroundFile))
                {
                    var fileInfo = this.GetBackgroundFileInfo();
                    page.BackgroundUrl = FileManager.Instance.GetUrl(fileInfo);

                    // ((HtmlGenericControl)this.FindControl("Body")).Attributes["style"] = string.Concat("background-image: url('", url, "')");
                }
            }

            // META Refresh
            // Only autorefresh the page if we are in VIEW-mode and if we aren't displaying some module's subcontrol.
            if (this.PortalSettings.ActiveTab.RefreshInterval > 0 && Personalization.GetUserMode() == PortalSettings.Mode.View && this.Request.QueryString["ctl"] == null)
            {
                page.MetaRefresh = this.PortalSettings.ActiveTab.RefreshInterval.ToString();
            }

            // META description
            if (!string.IsNullOrEmpty(this.PortalSettings.ActiveTab.Description))
            {
                page.Description = this.PortalSettings.ActiveTab.Description;
            }
            else
            {
                page.Description = this.PortalSettings.Description;
            }

            // META keywords
            if (!string.IsNullOrEmpty(this.PortalSettings.ActiveTab.KeyWords))
            {
                page.KeyWords = this.PortalSettings.ActiveTab.KeyWords;
            }
            else
            {
                page.KeyWords = this.PortalSettings.KeyWords;
            }

            // META copyright
            if (!string.IsNullOrEmpty(this.PortalSettings.FooterText))
            {
                page.Copyright = this.PortalSettings.FooterText.Replace("[year]", DateTime.Now.Year.ToString());
            }
            else
            {
                page.Copyright = string.Concat("Copyright (c) ", DateTime.Now.Year, " by ", this.PortalSettings.PortalName);
            }

            // META generator
            page.Generator = string.Empty;

            // META Robots - hide it inside popups and if PageHeadText of current tab already contains a robots meta tag
            if (!UrlUtils.InPopUp() &&
                !(HeaderTextRegex.IsMatch(this.PortalSettings.ActiveTab.PageHeadText) ||
                  HeaderTextRegex.IsMatch(this.PortalSettings.PageHeadText)))
            {
                var allowIndex = true;
                if ((this.PortalSettings.ActiveTab.TabSettings.ContainsKey("AllowIndex") &&
                     bool.TryParse(this.PortalSettings.ActiveTab.TabSettings["AllowIndex"].ToString(), out allowIndex) &&
                     !allowIndex)
                    ||
                    (this.Request.QueryString["ctl"] != null &&
                     (this.Request.QueryString["ctl"] == "Login" || this.Request.QueryString["ctl"] == "Register")))
                {
                    page.MetaRobots = "NOINDEX, NOFOLLOW";
                }
                else
                {
                    page.MetaRobots = "INDEX, FOLLOW";
                }
            }

            // NonProduction Label Injection
            if (this.NonProductionVersion() && Host.DisplayBetaNotice && !UrlUtils.InPopUp())
            {
                string versionString =
                    $" ({DotNetNukeContext.Current.Application.Status} Version: {DotNetNukeContext.Current.Application.Version})";
                page.Title += versionString;
            }

            // register the custom stylesheet of current page
            if (this.PortalSettings.ActiveTab.TabSettings.ContainsKey("CustomStylesheet") && !string.IsNullOrEmpty(this.PortalSettings.ActiveTab.TabSettings["CustomStylesheet"].ToString()))
            {
                var styleSheet = this.PortalSettings.ActiveTab.TabSettings["CustomStylesheet"].ToString();

                // Try and go through the FolderProvider first
                var stylesheetFile = this.GetPageStylesheetFileInfo(styleSheet);
                if (stylesheetFile != null)
                {
                    MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, FileManager.Instance.GetUrl(stylesheetFile));
                }
                else
                {
                    MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, styleSheet);
                }
            }

            // Cookie Consent
            if (this.PortalSettings.ShowCookieConsent)
            {
                JavaScriptLibraries.MvcJavaScript.RegisterClientReference(this.ControllerContext, ClientAPI.ClientNamespaceReferences.dnn);
                MvcClientAPI.RegisterClientVariable("cc_morelink", this.PortalSettings.CookieMoreLink, true);
                MvcClientAPI.RegisterClientVariable("cc_message", Localization.GetString("cc_message", Localization.GlobalResourceFile), true);
                MvcClientAPI.RegisterClientVariable("cc_dismiss", Localization.GetString("cc_dismiss", Localization.GlobalResourceFile), true);
                MvcClientAPI.RegisterClientVariable("cc_link", Localization.GetString("cc_link", Localization.GlobalResourceFile), true);
                MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/Resources/Shared/Components/CookieConsent/cookieconsent.min.js", FileOrder.Js.DnnControls);
                MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/Resources/Shared/Components/CookieConsent/cookieconsent.min.css", FileOrder.Css.ResourceCss);
                MvcClientResourceManager.RegisterScript(this.ControllerContext, "~/js/dnn.cookieconsent.js", FileOrder.Js.DefaultPriority);
            }
        }

        private IFileInfo GetBackgroundFileInfo()
        {
            string cacheKey = string.Format(Common.Utilities.DataCache.PortalCacheKey, this.PortalSettings.PortalId, "BackgroundFile");
            var file = CBO.GetCachedObject<Services.FileSystem.FileInfo>(
                new CacheItemArgs(cacheKey, Common.Utilities.DataCache.PortalCacheTimeOut, Common.Utilities.DataCache.PortalCachePriority),
                this.GetBackgroundFileInfoCallBack);

            return file;
        }

        private IFileInfo GetBackgroundFileInfoCallBack(CacheItemArgs itemArgs)
        {
            return FileManager.Instance.GetFile(this.PortalSettings.PortalId, this.PortalSettings.BackgroundFile);
        }

        private IFileInfo GetPageStylesheetFileInfo(string styleSheet)
        {
            string cacheKey = string.Format(Common.Utilities.DataCache.PortalCacheKey, this.PortalSettings.PortalId, "PageStylesheet" + styleSheet);
            var file = CBO.GetCachedObject<Services.FileSystem.FileInfo>(
                new CacheItemArgs(cacheKey, Common.Utilities.DataCache.PortalCacheTimeOut, Common.Utilities.DataCache.PortalCachePriority, styleSheet),
                this.GetPageStylesheetInfoCallBack);

            return file;
        }

        private IFileInfo GetPageStylesheetInfoCallBack(CacheItemArgs itemArgs)
        {
            var styleSheet = itemArgs.Params[0].ToString();
            return FileManager.Instance.GetFile(this.PortalSettings.PortalId, styleSheet);
        }
    }
}
