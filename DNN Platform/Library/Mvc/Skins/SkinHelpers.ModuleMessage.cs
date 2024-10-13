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
        public static IHtmlContent ModuleMessage(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string heading = "", string message = "", string cssClass = "dnnModuleMessage", string headingCssClass = "dnnModMessageHeading")
        {
            var panel = new TagBuilder("div");
            panel.AddCssClass(cssClass);

            if (!string.IsNullOrEmpty(heading))
            {
                var headingLabel = new TagBuilder("span");
                headingLabel.AddCssClass(headingCssClass);
                headingLabel.InnerHtml.Append(heading);
                panel.InnerHtml.AppendHtml(headingLabel);
            }

            var messageLabel = new TagBuilder("span");
            messageLabel.InnerHtml.Append(message);
            panel.InnerHtml.AppendHtml(messageLabel);

            var script = new TagBuilder("script");
            script.Attributes.Add("type", "text/javascript");
            script.InnerHtml.AppendHtml(@"
                jQuery(document).ready(function ($) {
                    var $body = window.opera ? (document.compatMode == 'CSS1Compat' ? $('html') : $('body')) : $('html,body');
                    var scrollTop = $('#" + panel.Attributes["id"] + @"').offset().top - parseInt($(document.body).css('margin-top'));
                    $body.animate({ scrollTop: scrollTop }, 'fast');
                });
            ");

            var result = new HtmlContentBuilder();
            result.AppendHtml(panel);
            result.AppendHtml(script);

            return result;
        }
    }
}
