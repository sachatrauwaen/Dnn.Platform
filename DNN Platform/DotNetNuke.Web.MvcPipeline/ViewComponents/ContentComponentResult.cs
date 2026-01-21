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

    // ContentComponentResult.cs
    public class ContentComponentResult : IViewComponentResult
    {
        public string Content { get; set; }

        public void Execute(ViewContext context)
        {
            context.Writer.Write(Content);
        }
    }
}
