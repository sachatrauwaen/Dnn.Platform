// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace Dnn.ContactList.Mvc
{
    using DotNetNuke.Web.MvcPipeline.Routing;

    public class RouteConfig : IMvcRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapRoute(
                "Html",
                "Html",
                "{controller}/{action}",
                new[] { "DotNetNuke.Modules.Html.Controllers" });
        }
    }
}
