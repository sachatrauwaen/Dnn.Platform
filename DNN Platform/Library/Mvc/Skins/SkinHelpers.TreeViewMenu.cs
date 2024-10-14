// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;

    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static partial class SkinHelpers
    {
        public static IHtmlContent TreeViewMenu(this IHtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var portalSettings = PortalSettings.Current;
            var link = new TagBuilder("a");

            link.Attributes.Add("href", portalSettings.PortalAlias.HTTPAlias);
            link.InnerHtml.Append(Localization.GetString("Title.Text", GetSkinsResourceFile("TreeViewMenu.ascx")));

            return new HtmlString(link.ToString());
        }
    }
}
