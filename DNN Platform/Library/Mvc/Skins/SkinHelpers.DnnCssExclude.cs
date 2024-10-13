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
        public static IHtmlContent DnnCssExclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name)
        {
            var cssExclude = new TagBuilder("dnn:DnnCssExclude");
            cssExclude.Attributes.Add("ID", "ctlExclude");
            cssExclude.Attributes.Add("runat", "server");
            cssExclude.Attributes.Add("Name", name);

            return cssExclude.RenderSelfClosingTag();
        }
    }
}
