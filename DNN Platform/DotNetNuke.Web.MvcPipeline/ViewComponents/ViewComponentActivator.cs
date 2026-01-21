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

    // ViewComponentActivator.cs (static accessor for DI container)
    public static class ViewComponentActivator
    {
        private static IViewComponentActivator _activator = new DefaultViewComponentActivator();

        public static void SetActivator(IViewComponentActivator activator)
        {
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        public static BaseViewComponent Create(Type componentType)
        {
            return _activator.Create(componentType);
        }
    }
}
