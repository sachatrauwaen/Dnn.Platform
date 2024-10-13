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
        public static IHtmlContent Meta(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name = "", string content = "", string httpEquiv = "", bool insertFirst = false)
        {
            var metaTag = new TagBuilder("meta");

            if (!string.IsNullOrEmpty(name))
            {
                metaTag.Attributes.Add("name", name);
            }

            if (!string.IsNullOrEmpty(content))
            {
                metaTag.Attributes.Add("content", content);
            }

            if (!string.IsNullOrEmpty(httpEquiv))
            {
                metaTag.Attributes.Add("http-equiv", httpEquiv);
            }

            return metaTag.RenderSelfClosingTag();
        }
    }
}
