// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Containers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework.Models;
    using DotNetNuke.Web.Mvc.Skins;

    public static partial class SkinHelpers
    {
        public static IHtmlString Content(this HtmlHelper<MvcContainer> htmlHelper)
        {
            var model = htmlHelper.ViewData.Model;
            if (model == null)
            {
                throw new InvalidOperationException("The model need to be present.");
            }

            var moduleDiv = new TagBuilder("div");
            try
            {
                moduleDiv.InnerHtml += htmlHelper.Action(model.ActionName, model.ControllerName, model.ModuleConfiguration);
            }
            catch (Exception ex)
            {
                moduleDiv.InnerHtml += $"Error : {ex.Message} (Controller : {model.ControllerName}, Action : {model.ActionName}, module : {model.ModuleConfiguration.ModuleTitle})";
            }

            return MvcHtmlString.Create(moduleDiv.InnerHtml);
        }
    }
}
