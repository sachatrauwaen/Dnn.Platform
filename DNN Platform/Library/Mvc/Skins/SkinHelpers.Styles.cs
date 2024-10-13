// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using DotNetNuke.UI.Skins;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent Styles(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string styleSheet, string condition = "", bool isFirst = false, bool useSkinPath = true, string media = "")
        {
            var skinPath = useSkinPath ? ((Skin)helper.ViewData["Skin"]).SkinPath : string.Empty;
            var link = new TagBuilder("link");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("href", skinPath + styleSheet);
            if (!string.IsNullOrEmpty(media))
            {
                link.Attributes.Add("media", media);
            }

            if (string.IsNullOrEmpty(condition))
            {
                return new HtmlString(link.ToString());
            }
            else
            {
                var openIf = new TagBuilder("span");
                openIf.InnerHtml.AppendHtml($"<!--[if {condition}]>");
                var closeIf = new TagBuilder("span");
                closeIf.InnerHtml.AppendHtml("<![endif]-->");
                return new HtmlString(openIf.ToString() + link.ToString() + closeIf.ToString());
            }
        }
    }
}
