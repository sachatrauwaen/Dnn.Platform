// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using DotNetNuke.Application;
    using DotNetNuke.Entities.Portals;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent DotNetNuke(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "")
        {
            var portalSettings = PortalSettings.Current;
            var link = new TagBuilder("a");
            link.Attributes.Add("href", DotNetNukeContext.Current.Application.Url);
            link.Attributes.Add("class", cssClass);
            link.InnerHtml.Append(DotNetNukeContext.Current.Application.LegalCopyright);

            return new HtmlString(link.ToString());
        }
    }
}
