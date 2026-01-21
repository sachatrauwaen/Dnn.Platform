// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DotNetNuke.Services.Search.Entities;
    using DotNetNuke.Web.MvcPipeline.ViewComponents;
    using Microsoft.Extensions.DependencyInjection;

    // ViewComponentResult.cs
    public class ViewComponentResult : IViewComponentResult
    {
        public string ViewName { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public ViewContext ViewContext { get; set; }

        public void Execute(ViewContext context)
        {
            var viewPath = $"~/Views/Shared/Components/{ViewName}.cshtml";
            var viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);

            if (viewEngineResult.View == null)
            {
                viewPath = $"~/Views/Shared/Components/{ViewName}/Default.cshtml";
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            }

            if (viewEngineResult.View != null)
            {
                var newViewContext = new ViewContext(
                    context.Controller.ControllerContext,
                    viewEngineResult.View,
                    ViewData,
                    context.TempData,
                    context.Writer
                );

                viewEngineResult.View.Render(newViewContext, context.Writer);
            }
        }
    }
}
