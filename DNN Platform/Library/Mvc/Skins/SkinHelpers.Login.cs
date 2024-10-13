// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;
    using System.Text;

    using Dnn.Migration;
    using DotNetNuke.Abstractions;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Authentication;
    using DotNetNuke.Services.Localization;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class SkinHelpers
    {
        public static IHtmlContent Login(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject", string text = "", string logoffText = "", bool legacyMode = true, bool showInErrorPage = false)
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            var httpContextAccessor = Globals.DependencyProvider.GetRequiredService<IHttpContextAccessor>();
            var sb = new StringBuilder();

            if (portalSettings.InErrorPageRequest() && !portalSettings.HideLoginControl)
            {
                return HtmlString.Empty;
            }

            var loginLink = new TagBuilder("a");
            loginLink.AddCssClass(cssClass);
            loginLink.Attributes.Add("rel", "nofollow");

            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(logoffText))
                {
                    if (logoffText.IndexOf("src=") != -1)
                    {
                        logoffText = logoffText.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath);
                    }

                    loginLink.InnerHtml.AppendHtml(logoffText);
                }
                else
                {
                    loginLink.InnerHtml.Append(Localization.GetString("Logout", GetSkinsResourceFile("Login.ascx")));
                }

                loginLink.Attributes.Add("title", loginLink.InnerHtml.ToString());
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

                    loginLink.InnerHtml.AppendHtml(text);
                }
                else
                {
                    loginLink.InnerHtml.Append(Localization.GetString("Login", GetSkinsResourceFile("Login.ascx")));
                }

                loginLink.Attributes.Add("title", loginLink.InnerHtml.ToString());

                string returnUrl = httpContextAccessor.HttpContext.Request.Path;
                if (returnUrl.IndexOf("?returnurl=") != -1)
                {
                    returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="));
                }

                returnUrl = Uri.EscapeDataString(returnUrl);

                loginLink.Attributes.Add("href", Globals.LoginURL(returnUrl, httpContextAccessor.HttpContext.Request.Query["override"].ToString() != null));

                // Avoid issues caused by multiple clicks of login link
                var oneclick = "this.disabled=true;";
                if (httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString() != null && !httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString().Contains("MSIE 8.0"))
                {
                    loginLink.Attributes.Add("onclick", oneclick);
                }

                if (portalSettings.EnablePopUps && portalSettings.LoginTabId == Null.NullInteger /*&& !AuthenticationController.HasSocialAuthenticationEnabled(portalSettings)*/)
                {
                    var clickEvent = "return " + UrlUtils.PopUpUrl(Uri.UnescapeDataString(loginLink.Attributes["href"]), portalSettings, true, false, 300, 650);
                    loginLink.Attributes["onclick"] = clickEvent;
                }
            }

            if ((!portalSettings.HideLoginControl || httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                && (!portalSettings.InErrorPageRequest() || showInErrorPage))
            {
                if (!legacyMode)
                {
                    sb.Append("<div class=\"loginGroup\" id=\"loginGroup\">");
                    sb.Append(loginLink.ToString());
                    sb.Append("</div>");
                }
                else
                {
                    sb.Append(loginLink.ToString());
                }
            }

            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                sb.Append("<div class=\"loginGroup\" id=\"loginGroup\" style=\"display:none;\">");
                sb.Append("<a id=\"enhancedLoginLink\" class=\"secondaryActionsList\" rel=\"nofollow\"></a>");
                sb.Append("</div>");
            }

            return new HtmlString(sb.ToString());
        }
    }
}
