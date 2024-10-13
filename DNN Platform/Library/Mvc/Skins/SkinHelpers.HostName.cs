// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Host;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent HostName(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "")
        {
            var link = new TagBuilder("a");
            link.Attributes.Add("href", Globals.AddHTTP(Host.HostURL));
            link.Attributes.Add("class", cssClass);
            link.InnerHtml.Append(Host.HostTitle);

            return new HtmlString(link.ToString());
        }
    }
}
