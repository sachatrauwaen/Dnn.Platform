// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetNuke.Web.MvcPipeline.ViewComponents
{
    public class DIViewComponentActivator : IViewComponentActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public DIViewComponentActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BaseViewComponent Create(Type componentType)
        {
            return (BaseViewComponent)_serviceProvider.GetRequiredService(componentType);
        }
    }
}
