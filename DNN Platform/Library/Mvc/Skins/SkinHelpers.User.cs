﻿// Licensed to the .NET Foundation under one or more agreements.
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
        public static IHtmlContent User(this IHtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, IHttpContextAccessor httpContextAccessor, string cssClass = "SkinObject", string text = "", string url = "", bool showUnreadMessages = true, bool showAvatar = true, bool legacyMode = true, bool showInErrorPage = false)
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            var userInfo = UserController.Instance.GetCurrentUserInfo();

            if (portalSettings.InErrorPageRequest() && !showInErrorPage)
            {
                return Microsoft.AspNetCore.Html.HtmlString.Empty;
            }

            var userPropertiesDiv = new TagBuilder("div");
            userPropertiesDiv.AddCssClass("userProperties");

            var ul = new TagBuilder("ul");

            var httpContext = httpContextAccessor.HttpContext;
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                // Logic for unauthenticated user
                if (portalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration &&
                    (portalSettings.Users < portalSettings.UserQuota || portalSettings.UserQuota == 0))
                {
                    var registerLi = new TagBuilder("li");
                    registerLi.AddCssClass("userRegister");

                    var registerLink = new TagBuilder("a");
                    registerLink.AddCssClass(cssClass);
                    registerLink.Attributes.Add("rel", "nofollow");
                    registerLink.InnerHtml.Append(!string.IsNullOrEmpty(text) ? text.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath) : Localization.GetString("Register", GetSkinsResourceFile("UserAndLogin.ascx")));
                    registerLink.Attributes.Add("href", !string.IsNullOrEmpty(url) ? url : Globals.RegisterURL(HttpUtility.UrlEncode(navigationManager.NavigateURL()), Null.NullString));

                    if (portalSettings.EnablePopUps && portalSettings.RegisterTabId == Null.NullInteger)
                    {
                        var clickEvent = "return " + UrlUtils.PopUpUrl(registerLink.Attributes["href"], portalSettings, true, false, 600, 950);
                        registerLink.Attributes.Add("onclick", clickEvent);
                    }

                    registerLi.InnerHtml.AppendHtml(registerLink);
                    ul.InnerHtml.AppendHtml(registerLi);
                }

                if (!portalSettings.HideLoginControl)
                {
                    var loginLi = new TagBuilder("li");
                    loginLi.AddCssClass("userLogin");

                    var loginLink = new TagBuilder("a");
                    loginLink.AddCssClass(cssClass);
                    loginLink.Attributes.Add("rel", "nofollow");
                    loginLink.InnerHtml.Append(Localization.GetString("Login", GetSkinsResourceFile("UserAndLogin.ascx")));
                    loginLink.Attributes.Add("href", Globals.LoginURL(HttpUtility.UrlEncode(httpContext.Request.Path), httpContext.Request.Query["override"] != QueryString.Empty));

                    if (portalSettings.EnablePopUps && portalSettings.LoginTabId == Null.NullInteger)
                    {
                        var clickEvent = "return " + UrlUtils.PopUpUrl(loginLink.Attributes["href"], portalSettings, true, false, 300, 650);
                        loginLink.Attributes.Add("onclick", clickEvent);
                    }

                    loginLi.InnerHtml.AppendHtml(loginLink);
                    ul.InnerHtml.AppendHtml(loginLi);
                }
            }
            else if (userInfo.UserID != -1)
            {
                // Logic for authenticated user
                var userNameLi = new TagBuilder("li");
                userNameLi.AddCssClass("userName");

                var userNameLink = new TagBuilder("a");
                userNameLink.Attributes.Add("id", "dnn_dnnUser_userNameLink");
                userNameLink.Attributes.Add("href", "#");
                userNameLink.InnerHtml.Append(userInfo.DisplayName);

                var userMenu = new TagBuilder("ul");
                userMenu.AddCssClass("userMenu");

                // Add various menu items (viewProfile, userMessages, userNotifications, etc.)
                userMenu.InnerHtml.AppendHtml(CreateMenuItem("viewProfile", Globals.UserProfileURL(userInfo.UserID), "Profile"));

                if (showUnreadMessages)
                {
                    var unreadMessages = InternalMessagingController.Instance.CountUnreadMessages(userInfo.UserID, PortalController.GetEffectivePortalId(userInfo.PortalID));
                    var unreadAlerts = NotificationsController.Instance.CountNotifications(userInfo.UserID, PortalController.GetEffectivePortalId(userInfo.PortalID));

                    userMenu.InnerHtml.AppendHtml(CreateMessageMenuItem("userMessages", navigationManager.NavigateURL(GetMessageTab(portalSettings), string.Empty, $"userId={userInfo.UserID}"), "Messages", unreadMessages));
                    userMenu.InnerHtml.AppendHtml(CreateMessageMenuItem("userNotifications", navigationManager.NavigateURL(GetMessageTab(portalSettings), string.Empty, $"userId={userInfo.UserID}", "view=notifications", "action=notifications"), "Notifications", unreadAlerts));
                }

                userMenu.InnerHtml.AppendHtml(CreateMenuItem("userSettings", navigationManager.NavigateURL(portalSettings.UserTabId, "Profile", $"userId={userInfo.UserID}", "pageno=1"), "Account"));
                userMenu.InnerHtml.AppendHtml(CreateMenuItem("userProfilename", navigationManager.NavigateURL(portalSettings.UserTabId, "Profile", $"userId={userInfo.UserID}", "pageno=2"), "EditProfile"));
                userMenu.InnerHtml.AppendHtml(CreateMenuItem("userLogout", navigationManager.NavigateURL(portalSettings.ActiveTab.TabID, "Logoff"), "Logout", true));

                userNameLi.InnerHtml.AppendHtml(userNameLink);
                userNameLi.InnerHtml.AppendHtml(userMenu);
                ul.InnerHtml.AppendHtml(userNameLi);

                if (showAvatar)
                {
                    var userProfileLi = new TagBuilder("li");
                    userProfileLi.AddCssClass("userProfile");

                    var profileLink = new TagBuilder("a");
                    profileLink.Attributes.Add("href", Globals.UserProfileURL(userInfo.UserID));

                    var profileImg = new TagBuilder("img");
                    profileImg.Attributes.Add("src", UserController.Instance.GetUserProfilePictureUrl(userInfo.UserID, 32, 32));
                    profileImg.Attributes.Add("alt", Localization.GetString("ProfilePicture", GetSkinsResourceFile("UserAndLogin.ascx")));

                    var profileImgSpan = new TagBuilder("span");
                    profileImgSpan.AddCssClass("userProfileImg");
                    profileImgSpan.InnerHtml.AppendHtml(profileImg);

                    profileLink.InnerHtml.AppendHtml(profileImgSpan);
                    userProfileLi.InnerHtml.AppendHtml(profileLink);

                    ul.InnerHtml.AppendHtml(userProfileLi);
                }
            }

            userPropertiesDiv.InnerHtml.AppendHtml(ul);
            return new Microsoft.AspNetCore.Html.HtmlString(userPropertiesDiv.ToString());
        }

        private static string CreateMenuItem(string cssClass, string href, string resourceKey, bool isStrong = false)
        {
            var li = new TagBuilder("li");
            li.AddCssClass(cssClass);

            var a = new TagBuilder("a");
            a.Attributes.Add("href", href);
            var text = Localization.GetString(resourceKey, Localization.GetResourceFile(null, "UserAndLogin.ascx"));
            a.InnerHtml.AppendHtml(isStrong ? $"<strong>{text}</strong>" : text);

            li.InnerHtml.AppendHtml(a);
            return li.ToString();
        }

        private static string CreateMessageMenuItem(string cssClass, string href, string resourceKey, int count)
        {
            var li = new TagBuilder("li");
            li.AddCssClass(cssClass);

            var a = new TagBuilder("a");
            a.Attributes.Add("href", href);

            if (count > 0 || AlwaysShowCount(PortalSettings.Current))
            {
                var span = new TagBuilder("span");
                span.AddCssClass(cssClass == "userMessages" ? "messageCount" : "notificationCount");
                span.InnerHtml.Append(count.ToString());
                a.InnerHtml.AppendHtml(span);
            }

            a.InnerHtml.AppendHtml(Localization.GetString(resourceKey, Localization.GetResourceFile(null, "UserAndLogin.ascx")));

            li.InnerHtml.AppendHtml(a);
            return li.ToString();
        }

        private static int GetMessageTab(PortalSettings portalSettings)
        {
            var cacheKey = string.Format("MessageCenterTab:{0}:{1}", portalSettings.PortalId, portalSettings.CultureCode);
            var messageTabId = DataCache.GetCache<int>(cacheKey);
            if (messageTabId > 0)
            {
                return messageTabId;
            }

            // Find the Message Tab
            messageTabId = FindMessageTab(portalSettings);

            // save in cache
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

            // default to User Profile Page
            return portalSettings.UserTabId;
        }

        private static bool AlwaysShowCount(PortalSettings portalSettings)
        {
            const string SettingKey = "UserAndLogin_AlwaysShowCount";
            var alwaysShowCount = false;

            var portalSetting = PortalController.GetPortalSetting(SettingKey, portalSettings.PortalId, string.Empty);
            if (!string.IsNullOrEmpty(portalSetting) && bool.TryParse(portalSetting, out alwaysShowCount))
            {
                return alwaysShowCount;
            }

            var hostSetting = HostController.Instance.GetString(SettingKey, string.Empty);
            if (!string.IsNullOrEmpty(hostSetting) && bool.TryParse(hostSetting, out alwaysShowCount))
            {
                return alwaysShowCount;
            }

            return false;
        }
    }
}
