// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;
    using System.Text;

    using Dnn.Migration;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.UI;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent Links(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject", string separator = " ", string level = "same", bool showDisabled = false, bool forceLinks = true, bool includeActiveTab = true)
        {
            var portalSettings = PortalSettings.Current;
            var links = new StringBuilder();

            var tabs = TabController.GetTabsBySortOrder(portalSettings.PortalId);
            foreach (var tab in tabs)
            {
                if (Navigation.CanShowTab(tab, false, showDisabled))
                {
                    if (level == "same" && tab.ParentId == portalSettings.ActiveTab.ParentId)
                    {
                        if (includeActiveTab || tab.TabID != portalSettings.ActiveTab.TabID)
                        {
                            links.Append($"<a class=\"{cssClass}\" href=\"{tab.FullUrl}\">{tab.TabName}</a>{separator}");
                        }
                    }
                    else if (level == "child" && tab.ParentId == portalSettings.ActiveTab.TabID)
                    {
                        links.Append($"<a class=\"{cssClass}\" href=\"{tab.FullUrl}\">{tab.TabName}</a>{separator}");
                    }
                    else if (level == "parent" && tab.TabID == portalSettings.ActiveTab.ParentId)
                    {
                        links.Append($"<a class=\"{cssClass}\" href=\"{tab.FullUrl}\">{tab.TabName}</a>{separator}");
                    }
                    else if (level == "root" && tab.Level == 0)
                    {
                        links.Append($"<a class=\"{cssClass}\" href=\"{tab.FullUrl}\">{tab.TabName}</a>{separator}");
                    }
                }
            }

            if (forceLinks && string.IsNullOrEmpty(links.ToString()))
            {
                foreach (var tab in tabs)
                {
                    if (Navigation.CanShowTab(tab, false, showDisabled))
                    {
                        links.Append($"<a class=\"{cssClass}\" href=\"{tab.FullUrl}\">{tab.TabName}</a>{separator}");
                    }
                }
            }

            return new HtmlString(links.ToString().TrimEnd(separator.ToCharArray()));
        }
    }
}
