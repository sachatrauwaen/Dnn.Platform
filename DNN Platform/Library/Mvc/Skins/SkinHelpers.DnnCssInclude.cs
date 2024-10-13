// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;

    using ClientDependency.Core.Mvc;
    using Dnn.Migration;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent DnnCssInclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string filePath, string pathNameAlias = "", int priority = 100, bool addTag = false, string name = "", string version = "", bool forceVersion = false, string forceProvider = "", bool forceBundle = false, string cssMedia = "")
        {
            // Note: The RequiresCss method needs to be replaced with an equivalent in ASP.NET Core
            helper.RequiresCss(filePath, pathNameAlias, priority);

            if (addTag /*|| helper.ViewContext.HttpContext.IsDebuggingEnabled()*/)
            {
                return new HtmlString($"<!--CDF(Css|{filePath}|{forceProvider}|{priority})-->");
            }

            return HtmlString.Empty;
        }
    }
}
