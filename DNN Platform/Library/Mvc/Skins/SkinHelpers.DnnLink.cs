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
        public static IHtmlContent DnnLink(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "", string target = "")
        {
            var link = new TagBuilder("a");
            link.Attributes.Add("href", "http://www.dnnsoftware.com/community?utm_source=dnn-install&utm_medium=web-link&utm_content=gravity-skin-link&utm_campaign=dnn-install");
            link.Attributes.Add("class", cssClass);
            link.Attributes.Add("target", target);
            link.InnerHtml.Append("CMS by DNN");

            return new HtmlString(link.ToString());
        }
    }
}
