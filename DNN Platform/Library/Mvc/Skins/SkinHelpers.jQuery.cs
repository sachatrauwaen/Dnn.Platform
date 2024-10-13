// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent JQuery(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, bool dnnjQueryPlugins = false, bool jQueryHoverIntent = false, bool jQueryUI = false)
        {
            var contentBuilder = new HtmlContentBuilder();

            var script = new TagBuilder("script");
            script.Attributes.Add("src", "~/Resources/Shared/Scripts/jquery/jquery.js");
            script.Attributes.Add("type", "text/javascript");
            contentBuilder.AppendHtml(script);

            if (dnnjQueryPlugins)
            {
                var dnnScript = new TagBuilder("script");
                dnnScript.Attributes.Add("src", "~/Resources/Shared/Scripts/dnn.jquery.js");
                dnnScript.Attributes.Add("type", "text/javascript");
                contentBuilder.AppendHtml(dnnScript);
            }

            if (jQueryHoverIntent)
            {
                var hoverScript = new TagBuilder("script");
                hoverScript.Attributes.Add("src", "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.js");
                hoverScript.Attributes.Add("type", "text/javascript");
                contentBuilder.AppendHtml(hoverScript);
            }

            if (jQueryUI)
            {
                var uiScript = new TagBuilder("script");
                uiScript.Attributes.Add("src", "~/Resources/Shared/Scripts/jquery/jquery-ui.js");
                uiScript.Attributes.Add("type", "text/javascript");
                contentBuilder.AppendHtml(uiScript);
            }

            return contentBuilder;
        }
    }
}
