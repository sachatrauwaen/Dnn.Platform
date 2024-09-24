// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Authentication;
    using DotNetNuke.Services.Localization;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class SkinExtensions
    {
        public static IHtmlString Login(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject", string text = "", string logoffText = "")
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            var sb = new System.Text.StringBuilder();

            if (portalSettings.InErrorPageRequest() && !portalSettings.HideLoginControl)
            {
                return MvcHtmlString.Empty;
            }

            var loginLink = new TagBuilder("a");
            loginLink.AddCssClass(cssClass);
            loginLink.Attributes.Add("rel", "nofollow");

            if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(logoffText))
                {
                    if (logoffText.IndexOf("src=") != -1)
                    {
                        logoffText = logoffText.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath);
                    }
                    loginLink.InnerHtml = logoffText;
                }
                else
                {
                    loginLink.InnerHtml = Localization.GetString("Logout", Localization.GetResourceFile(helper.ViewContext.Controller, "Login.ascx"));
                }
                loginLink.Attributes.Add("title", loginLink.InnerHtml);
                loginLink.Attributes.Add("href", navigationManager.NavigateURL(portalSettings.ActiveTab.TabID, "Logoff"));
            }
            else
            {
                if (!string.IsNullOrEmpty(text))
                {
                    if (text.IndexOf("src=") != -1)
                    {
                        text = text.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath);
                    }
                    loginLink.InnerHtml = text;
                }
                else
                {
                    loginLink.InnerHtml = Localization.GetString("Login", Localization.GetResourceFile(helper.ViewContext.Controller, "Login.ascx"));
                }
                loginLink.Attributes.Add("title", loginLink.InnerHtml);

                string returnUrl = HttpContext.Current.Request.RawUrl;
                if (returnUrl.IndexOf("?returnurl=") != -1)
                {
                    returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="));
                }
                returnUrl = HttpUtility.UrlEncode(returnUrl);

                loginLink.Attributes.Add("href", Globals.LoginURL(returnUrl, HttpContext.Current.Request.QueryString["override"] != null));

                // Avoid issues caused by multiple clicks of login link
                var oneclick = "this.disabled=true;";
                if (HttpContext.Current.Request.UserAgent != null && !HttpContext.Current.Request.UserAgent.Contains("MSIE 8.0"))
                {
                    loginLink.Attributes.Add("onclick", oneclick);
                }

                if (portalSettings.EnablePopUps && portalSettings.LoginTabId == Null.NullInteger && !AuthenticationController.HasSocialAuthenticationEnabled(portalSettings))
                {
                    var clickEvent = "return " + UrlUtils.PopUpUrl(HttpUtility.UrlDecode(loginLink.Attributes["href"]), helper, portalSettings, true, false, 300, 650);
                    loginLink.Attributes["onclick"] = clickEvent;
                }
            }

            if (!portalSettings.HideLoginControl || HttpContext.Current.Request.IsAuthenticated)
            {
                sb.Append(loginLink.ToString());
            }

            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                sb.Append("<div class=\"loginGroup\" id=\"loginGroup\" style=\"display:none;\">");
                sb.Append("<a id=\"enhancedLoginLink\" class=\"secondaryActionsList\" rel=\"nofollow\"></a>");
                sb.Append("</div>");
            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}
