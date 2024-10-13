// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Containers
{
    using System;
    using System.IO;

    using Dnn.Migration;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework.Models;
    using DotNetNuke.Web.Mvc.Skins;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent Title(this HtmlHelper<ContainerModel> htmlHelper, string cssClass)
        {
            var model = htmlHelper.ViewData.Model;
            if (model == null)
            {
                throw new InvalidOperationException("The model needs to be present.");
            }

            var labelDiv = new TagBuilder("div");
            labelDiv.InnerHtml.Append(model.ModuleConfiguration.ModuleTitle);
            if (!string.IsNullOrEmpty(cssClass))
            {
                labelDiv.AddCssClass(cssClass);
            }

            return labelDiv;
        }
    }
}
