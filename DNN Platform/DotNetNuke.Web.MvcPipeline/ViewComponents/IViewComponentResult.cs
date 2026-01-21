// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.ViewComponents
{
    using System.Web.Mvc;

    // IViewComponentResult.cs
    public interface IViewComponentResult
    {
        void Execute(ViewContext context);
    }
}

