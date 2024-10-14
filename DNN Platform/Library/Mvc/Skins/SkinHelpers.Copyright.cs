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
        public static IHtmlContent Copyright(this IHtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var portalSettings = PortalSettings.Current;
            var lblCopyright = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblCopyright.AddCssClass(cssClass);
            }

            if (!string.IsNullOrEmpty(portalSettings.FooterText))
            {
                lblCopyright.InnerHtml.Append(portalSettings.FooterText.Replace("[year]", DateTime.Now.ToString("yyyy")));
            }
            else
            {
                lblCopyright.InnerHtml.Append(string.Format(Localization.GetString("Copyright", GetSkinsResourceFile("Copyright.ascx")), DateTime.Now.Year, portalSettings.PortalName));
            }

            return new HtmlString(lblCopyright.ToString());
        }
    }
}
