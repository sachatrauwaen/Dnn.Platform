// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using DotNetNuke.Services.Localization;

    public static partial class SkinHelpers
    {
        public static IHtmlContent ControlPanel(this IHtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var lblControlPanel = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblControlPanel.AddCssClass(cssClass);
            }

            // lblControlPanel.InnerHtml.Append(Localization.GetString("ControlPanel", Localization.GetResourceFile(helper.ViewContext.Controller, "ControlPanel.ascx")));
            return new HtmlString(lblControlPanel.ToString());
        }
    }
}
