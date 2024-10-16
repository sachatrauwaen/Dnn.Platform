﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    public static partial class SkinHelpers
    {
        public static IHtmlString JavaScriptLibraryInclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name, string version = "", string specificVersion = "")
        {
            var script = new TagBuilder("script");
            script.Attributes.Add("src", name);
            script.Attributes.Add("type", "text/javascript");

            if (!string.IsNullOrEmpty(version))
            {
                script.Attributes.Add("data-version", version);
            }

            if (!string.IsNullOrEmpty(specificVersion))
            {
                script.Attributes.Add("data-specific-version", specificVersion);
            }

            return new MvcHtmlString(script.ToString());
        }
    }
}
