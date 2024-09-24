// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Services.Localization;

    public static partial class SkinExtensions
    {
        public static IHtmlString ControlPanel(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var lblControlPanel = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblControlPanel.AddCssClass(cssClass);
            }

            // lblControlPanel.SetInnerText(Localization.GetString("ControlPanel", Localization.GetResourceFile(helper.ViewContext.Controller, "ControlPanel.ascx")));
            return new MvcHtmlString(lblControlPanel.ToString());
        }
    }
}
