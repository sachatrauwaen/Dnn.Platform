// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Website.Areas.Shop
{
    using System.Web.Mvc;

    using DotNetNuke.Web.MvcPipeline.Routing;

    public class ShopAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Shop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var route = context.MapRoute(
                "Shop_default",
                "API/Shop/{controller}/{action}/{id}",
                new { controller= "Default", action = "Index", id = UrlParameter.Optional });

            route.RouteHandler = new DnnMvcPageRouteHandler();
        }
    }
}
