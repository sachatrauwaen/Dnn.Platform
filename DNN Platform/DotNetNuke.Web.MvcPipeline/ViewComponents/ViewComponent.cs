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

    public abstract class BaseViewComponent
    {
        public ViewContext ViewContext { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        protected BaseViewComponent()
        {
            this.ViewData = new ViewDataDictionary();
        }

        // Default Invoke with no parameters (for backwards compatibility)
        public virtual IViewComponentResult Invoke()
        {
            throw new NotImplementedException("You must implement either Invoke() or InvokeAsync()");
        }

        protected ViewComponentResult View()
        {
            return View(null, null);
        }

        protected ViewComponentResult View(string viewName)
        {
            return View(viewName, null);
        }

        protected ViewComponentResult View(object model)
        {
            return View(null, model);
        }

        protected ViewComponentResult View(string viewName, object model)
        {
            return new ViewComponentResult
            {
                ViewName = viewName ?? GetDefaultViewName(),
                ViewData = new ViewDataDictionary(model),
                ViewContext = ViewContext
            };
        }

        protected ContentComponentResult Content(string content)
        {
            return new ContentComponentResult { Content = content };
        }

        private string GetDefaultViewName()
        {
            return GetType().Name.Replace("ViewComponent", "");
        }
    }
}
