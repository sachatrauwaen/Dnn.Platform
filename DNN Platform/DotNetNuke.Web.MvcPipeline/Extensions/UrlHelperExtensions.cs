using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetNuke.Web.MvcPipeline.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string PageAction(this UrlHelper urlHelper,
        string actionName, string controllerName, object routeValues = null)
        {
            string url = urlHelper.Action(actionName, controllerName, routeValues);
            var navigationManager = Globals.GetCurrentServiceProvider().GetRequiredService<INavigationManager>();

            var pageUrl = navigationManager.NavigateURL();
            var pageRoute = TabController.CurrentPage?.KeyWords;
            if (!string.IsNullOrEmpty(pageRoute))
            {
                // Check if the page route contains "custom-path"
                if (url.Contains(pageRoute))
                {
                    // Replace the URL with the custom path
                    url = url.Trim('/').Replace(pageRoute, pageUrl);
                }
            }

            return url;
        }
    }
}
