// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;

    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;

    public static partial class SkinHelpers
    {
        public static IHtmlString Copyright(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var portalSettings = PortalSettings.Current;
            var lblCopyright = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblCopyright.AddCssClass(cssClass);
            }

            if (!string.IsNullOrEmpty(portalSettings.FooterText))
            {
                lblCopyright.SetInnerText(portalSettings.FooterText.Replace("[year]", DateTime.Now.ToString("yyyy")));
            }
            else
            {
                lblCopyright.SetInnerText(string.Format(Localization.GetString("Copyright", GetSkinsResourceFile("Copyright.ascx")), DateTime.Now.Year, portalSettings.PortalName));
            }

            return new MvcHtmlString(lblCopyright.ToString());
        }
    }
}
