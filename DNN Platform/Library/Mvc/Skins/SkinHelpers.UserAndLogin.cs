// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Services.Authentication;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Social.Messaging.Internal;
    using DotNetNuke.Services.Social.Notifications;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class SkinHelpers
    {
        public static IHtmlContent UserAndLogin(this IHtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, IHttpContextAccessor httpContextAccessor, bool showInErrorPage = false)
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();

            if (!showInErrorPage && portalSettings.InErrorPageRequest())
            {
                return Microsoft.AspNetCore.Html.HtmlString.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("<div class=\"userProperties\"><ul>");

            var httpContext = httpContextAccessor.HttpContext;
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                if (CanRegister(portalSettings))
                {
                    sb.Append($"<li class=\"userRegister\"><a id=\"registerLink\" href=\"{RegisterUrl(navigationManager)}\">{LocalizeString(helper, "Register")}</a></li>");
                }

                if (!portalSettings.HideLoginControl)
                {
                    sb.Append($"<li class=\"userLogin\"><a id=\"loginLink\" href=\"{LoginUrl(httpContext)}\">{LocalizeString(helper, "Login")}</a></li>");
                }
            }
            else
            {
                var userInfo = portalSettings.UserInfo;
                sb.Append($"<li class=\"userName\"><a id=\"dnn_dnnUser_userNameLink\" href=\"#\">{userInfo.DisplayName}</a>");
                sb.Append("<ul class=\"userMenu\">");
                sb.Append($"<li class=\"viewProfile\"><a id=\"viewProfileLink\" href=\"{Globals.UserProfileURL(userInfo.UserID)}\">{LocalizeString(helper, "Profile")}</a></li>");

                var unreadMessages = InternalMessagingController.Instance.CountUnreadMessages(userInfo.UserID, portalSettings.PortalId);
                var unreadAlerts = NotificationsController.Instance.CountNotifications(userInfo.UserID, portalSettings.PortalId);
                var messageTabId = GetMessageTab(portalSettings);

                sb.Append($"<li class=\"userMessages\"><a id=\"messagesLink\" href=\"{navigationManager.NavigateURL(messageTabId, string.Empty, $"userId={userInfo.UserID}")}\"><span id=\"messageCount\" {(unreadMessages > 0 ? string.Empty : "style=\"display:none;\"")}>{unreadMessages}</span>{LocalizeString(helper, "Messages")}</a></li>");
                sb.Append($"<li class=\"userNotifications\"><a id=\"notificationsLink\" href=\"{navigationManager.NavigateURL(messageTabId, string.Empty, $"userId={userInfo.UserID}", "view=notifications", "action=notifications")}\"><span id=\"notificationCount\" {(unreadAlerts > 0 ? string.Empty : "style=\"display:none;\"")}>{unreadAlerts}</span>{LocalizeString(helper, "Notifications")}</a></li>");
                sb.Append($"<li class=\"userSettings\"><a id=\"accountLink\" href=\"{navigationManager.NavigateURL(portalSettings.UserTabId, "Profile", $"userId={userInfo.UserID}", "pageno=1")}\">{LocalizeString(helper, "Account")}</a></li>");
                sb.Append($"<li class=\"userProfilename\"><a id=\"editProfileLink\" href=\"{navigationManager.NavigateURL(portalSettings.UserTabId, "Profile", $"userId={userInfo.UserID}", "pageno=2")}\">{LocalizeString(helper, "EditProfile")}</a></li>");
                sb.Append($"<li class=\"userLogout\"><a id=\"logoffLink\" href=\"{navigationManager.NavigateURL(portalSettings.ActiveTab.TabID, "Logoff")}\"><strong>{LocalizeString(helper, "Logout")}</strong></a></li>");
                sb.Append("</ul></li>");

                sb.Append("<li class=\"userProfile\">");
                sb.Append($"<a id=\"viewProfileImageLink\" href=\"{Globals.UserProfileURL(userInfo.UserID)}\"><span class=\"userProfileImg\"><img id=\"profilePicture\" src=\"{UserController.Instance.GetUserProfilePictureUrl(userInfo.UserID, 32, 32)}\" alt=\"{LocalizeString(helper, "ProfilePicture")}\" /></span></a>");
                if (unreadMessages > 0)
                {
                    sb.Append($"<span id=\"messages\" class=\"userMessages\" title=\"{(unreadMessages == 1 ? LocalizeString(helper, "OneMessage") : string.Format(LocalizeString(helper, "MessageCount"), unreadMessages))}\">{unreadMessages}</span>");
                }

                sb.Append("</li>");
            }

            sb.Append("</ul></div>");

            var result = sb.ToString();

            if (UsePopUp(portalSettings))
            {
                result = result.Replace("id=\"registerLink\"", $"id=\"registerLink\" onclick=\"{RegisterUrlForClickEvent(navigationManager, portalSettings)}\"");
                result = result.Replace("id=\"loginLink\"", $"id=\"loginLink\" onclick=\"{LoginUrlForClickEvent(portalSettings, httpContext)}\"");
            }

            return new Microsoft.AspNetCore.Html.HtmlString(result);
        }

        private static bool CanRegister(PortalSettings portalSettings)
        {
            return (portalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration)
                && (portalSettings.Users < portalSettings.UserQuota || portalSettings.UserQuota == 0);
        }

        private static string RegisterUrl(INavigationManager navigationManager)
        {
            return Globals.RegisterURL(HttpUtility.UrlEncode(navigationManager.NavigateURL()), Null.NullString);
        }

        private static string LoginUrl(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            string returnUrl = httpContext.Request.Path;
            if (returnUrl.IndexOf("?returnurl=", StringComparison.OrdinalIgnoreCase) != -1)
            {
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl=", StringComparison.OrdinalIgnoreCase));
            }

            returnUrl = HttpUtility.UrlEncode(returnUrl);

            return Globals.LoginURL(returnUrl, httpContext.Request.Query["override"] != QueryString.Empty);
        }

        private static bool UsePopUp(PortalSettings portalSettings)
        {
            return portalSettings.EnablePopUps
                && portalSettings.LoginTabId == Null.NullInteger;
        }

        private static string RegisterUrlForClickEvent(INavigationManager navigationManager, PortalSettings portalSettings)
        {
            return "return " + UrlUtils.PopUpUrl(HttpUtility.UrlDecode(RegisterUrl(navigationManager)), portalSettings, true, false, 600, 950);
        }

        private static string LoginUrlForClickEvent(PortalSettings portalSettings, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return "return " + UrlUtils.PopUpUrl(HttpUtility.UrlDecode(LoginUrl(httpContext)), portalSettings, true, false, 300, 650);
        }

        private static string LocalizeString(IHtmlHelper helper, string key)
        {
            return Localization.GetString(key, GetSkinsResourceFile("UserAndLogin.ascx"));
        }
    }
}
