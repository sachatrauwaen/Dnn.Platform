// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;
    using System.Text;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Services.Authentication;
    using DotNetNuke.Services.Social.Messaging.Internal;
    using DotNetNuke.Services.Social.Notifications;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class SkinExtensions
    {
        public static IHtmlString User(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject", bool showUnreadMessages = true, bool showAvatar = true, bool legacyMode = true, string text = "", string url = "", bool showInErrorPage = false)
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();

            if (!showInErrorPage && portalSettings.InErrorPageRequest())
            {
                return MvcHtmlString.Empty;
            }

            var sb = new StringBuilder();

            if (legacyMode)
            {
                sb.Append("<a id=\"registerLink\" class=\"" + cssClass + "\" rel=\"nofollow\"></a>");
            }
            else
            {
                sb.Append("<div class=\"registerGroup\" id=\"registerGroup\">");
                sb.Append("<ul class=\"buttonGroup\">");
                sb.Append("<li class=\"userMessages alpha\" id=\"messageGroup\"><a id=\"messageLink\"></a></li>");
                sb.Append("<li class=\"userNotifications omega\" id=\"notificationGroup\"><a id=\"notificationLink\"></a></li>");
                sb.Append("<li class=\"userDisplayName\"><a id=\"enhancedRegisterLink\" rel=\"nofollow\"></a></li>");
                sb.Append("<li class=\"userProfileImg\" id=\"avatarGroup\"><a id=\"avatar\"></a></li>");
                sb.Append("</ul>");
                sb.Append("</div>");
            }

            var userInfo = UserController.Instance.GetCurrentUserInfo();

            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                if (portalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration)
                {
                    string registerText = !string.IsNullOrEmpty(text) 
                        ? (text.IndexOf("src=") != -1 ? text.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath) : text)
                        : Localization.GetString("Register", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx"));

                    string registerUrl = !string.IsNullOrEmpty(url)
                        ? url
                        : Globals.RegisterURL(HttpUtility.UrlEncode(navigationManager.NavigateURL()), Null.NullString);

                    if (portalSettings.Users < portalSettings.UserQuota || portalSettings.UserQuota == 0)
                    {
                        if (legacyMode)
                        {
                            sb.Replace("id=\"registerLink\"", $"id=\"registerLink\" href=\"{registerUrl}\">{registerText}");
                        }
                        else
                        {
                            sb.Replace("id=\"enhancedRegisterLink\"", $"id=\"enhancedRegisterLink\" href=\"{registerUrl}\">{registerText}");
                        }

                        if (portalSettings.EnablePopUps && portalSettings.RegisterTabId == Null.NullInteger && !AuthenticationController.HasSocialAuthenticationEnabled(portalSettings))
                        {
                            var clickEvent = "return " + UrlUtils.PopUpUrl(registerUrl, helper, portalSettings, true, false, 600, 950);
                            sb.Replace("rel=\"nofollow\"", $"rel=\"nofollow\" onclick=\"{clickEvent}\"");
                        }
                    }
                    else
                    {
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Clear();
                }
            }
            else if (userInfo.UserID != -1)
            {
                string displayName = userInfo.DisplayName;
                string profileUrl = Globals.UserProfileURL(userInfo.UserID);
                string profileTooltip = Localization.GetString("VisitMyProfile", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx"));

                if (showUnreadMessages)
                {
                    int unreadMessages = InternalMessagingController.Instance.CountUnreadMessages(userInfo.UserID, PortalController.GetEffectivePortalId(userInfo.PortalID));
                    int unreadAlerts = NotificationsController.Instance.CountNotifications(userInfo.UserID, PortalController.GetEffectivePortalId(userInfo.PortalID));

                    string messageText = unreadMessages > 0 
                        ? string.Format(Localization.GetString("Messages", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx")), unreadMessages) 
                        : Localization.GetString("NoMessages", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx"));
                    string notificationText = unreadAlerts > 0 
                        ? string.Format(Localization.GetString("Notifications", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx")), unreadAlerts) 
                        : Localization.GetString("NoNotifications", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx"));

                    int messageTabId = GetMessageTab(portalSettings);
                    string messageUrl = navigationManager.NavigateURL(messageTabId, string.Empty, $"userId={userInfo.UserID}");
                    string notificationUrl = navigationManager.NavigateURL(messageTabId, string.Empty, $"userId={userInfo.UserID}", "view=notifications", "action=notifications");

                    sb.Replace("id=\"messageLink\"", $"id=\"messageLink\" href=\"{messageUrl}\">{messageText}");
                    sb.Replace("id=\"notificationLink\"", $"id=\"notificationLink\" href=\"{notificationUrl}\">{notificationText}");

                    if (legacyMode && unreadMessages > 0)
                    {
                        displayName += string.Format(Localization.GetString("NewMessages", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx")), unreadMessages);
                    }
                }
                else
                {
                    sb.Replace("<li class=\"userMessages alpha\" id=\"messageGroup\"><a id=\"messageLink\"></a></li>", string.Empty);
                    sb.Replace("<li class=\"userNotifications omega\" id=\"notificationGroup\"><a id=\"notificationLink\"></a></li>", string.Empty);
                }

                if (showAvatar)
                {
                    string avatarUrl = UserController.Instance.GetUserProfilePictureUrl(userInfo.UserID, 32, 32);
                    sb.Replace("id=\"avatar\"", $"id=\"avatar\" href=\"{profileUrl}\"><img src=\"{avatarUrl}\" alt=\"{Localization.GetString("ProfileAvatar", Localization.GetResourceFile(helper.ViewContext.Controller, "User.ascx"))}\" />");
                }
                else
                {
                    sb.Replace("<li class=\"userProfileImg\" id=\"avatarGroup\"><a id=\"avatar\"></a></li>", string.Empty);
                }

                if (legacyMode)
                {
                    sb.Replace("id=\"registerLink\"", $"id=\"registerLink\" href=\"{profileUrl}\" title=\"{profileTooltip}\">{displayName}");
                }
                else
                {
                    sb.Replace("id=\"enhancedRegisterLink\"", $"id=\"enhancedRegisterLink\" href=\"{profileUrl}\" title=\"{profileTooltip}\">{displayName}");
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        private static int GetMessageTab(PortalSettings portalSettings)
        {
            var cacheKey = $"MessageCenterTab:{portalSettings.PortalId}:{portalSettings.CultureCode}";
            var messageTabId = DataCache.GetCache<int>(cacheKey);
            if (messageTabId > 0)
            {
                return messageTabId;
            }

            messageTabId = FindMessageTab(portalSettings);
            DataCache.SetCache(cacheKey, messageTabId, TimeSpan.FromMinutes(20));

            return messageTabId;
        }

        private static int FindMessageTab(PortalSettings portalSettings)
        {
            var profileTab = TabController.Instance.GetTab(portalSettings.UserTabId, portalSettings.PortalId, false);
            if (profileTab != null)
            {
                var childTabs = TabController.Instance.GetTabsByPortal(profileTab.PortalID).DescendentsOf(profileTab.TabID);
                foreach (TabInfo tab in childTabs)
                {
                    foreach (KeyValuePair<int, ModuleInfo> kvp in ModuleController.Instance.GetTabModules(tab.TabID))
                    {
                        var module = kvp.Value;
                        if (module.DesktopModule.FriendlyName == "Message Center" && !module.IsDeleted)
                        {
                            return tab.TabID;
                        }
                    }
                }
            }

            return portalSettings.UserTabId;
        }
    }
}
