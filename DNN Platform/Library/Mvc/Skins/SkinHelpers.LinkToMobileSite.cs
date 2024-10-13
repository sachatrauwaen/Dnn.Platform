// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Mobile;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent LinkToMobileSite(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var redirectionController = new RedirectionController();
            var redirectUrl = redirectionController.GetMobileSiteUrl();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                var portalSettings = PortalSettings.Current;
                var link = new TagBuilder("a");

                link.Attributes.Add("href", portalSettings.PortalAlias.HTTPAlias);
                link.InnerHtml.Append(Localization.GetString("lnkPortal.Text", GetSkinsResourceFile("LinkToMobileSite.ascx")));
                return new HtmlString(link.ToString());
            }
            else
            {
                return HtmlString.Empty;
            }
        }
    }
}
