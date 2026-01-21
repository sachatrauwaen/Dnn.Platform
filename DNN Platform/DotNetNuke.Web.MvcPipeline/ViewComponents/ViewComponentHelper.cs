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

    // ViewComponentHelper.cs
    public static class ViewComponentHelper
    {
        public static MvcHtmlString RenderViewComponent<T>(this HtmlHelper helper)
            where T : BaseViewComponent
        {
            return RenderViewComponentInternal<T>(helper, null);
        }

        public static MvcHtmlString RenderViewComponent<T>(this HtmlHelper helper, object arguments)
            where T : BaseViewComponent
        {
            return RenderViewComponentInternal<T>(helper, arguments);
        }

        private static MvcHtmlString RenderViewComponentInternal<T>(HtmlHelper helper, object arguments)
            where T : BaseViewComponent
        {
            var component = ViewComponentActivator.Create(typeof(T));

            component.ViewContext = helper.ViewContext;
            component.ViewData = new ViewDataDictionary(arguments);

            var writer = new System.IO.StringWriter();
            var viewContext = new ViewContext(
                helper.ViewContext.Controller.ControllerContext,
                helper.ViewContext.View,
                helper.ViewContext.ViewData,
                helper.ViewContext.TempData,
                writer
            );

            IViewComponentResult result;

            if (arguments != null)
            {
                var cachedMethod = ViewComponentMethodCache.GetInvokeMethod(typeof(T), arguments);

                if (cachedMethod != null && cachedMethod.Parameters.Length > 0)
                {
                    var args = ExtractArgumentValues(cachedMethod.Parameters, arguments);
                    result = (IViewComponentResult)cachedMethod.Method.Invoke(component, args);
                }
                else
                {
                    result = component.Invoke();
                }
            }
            else
            {
                var cachedMethod = ViewComponentMethodCache.GetInvokeMethod(typeof(T), null);

                if (cachedMethod != null && cachedMethod.Parameters.Length == 0)
                {
                    result = (IViewComponentResult)cachedMethod.Method.Invoke(component, null);
                }
                else
                {
                    result = component.Invoke();
                }
            }

            result.Execute(viewContext);

            return MvcHtmlString.Create(writer.ToString());
        }

        private static object[] ExtractArgumentValues(System.Reflection.ParameterInfo[] parameters, object arguments)
        {
            var argumentProperties = ViewComponentMethodCache.GetProperties(arguments.GetType());
            var argumentDict = argumentProperties.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                if (argumentDict.TryGetValue(param.Name, out var prop))
                {
                    values[i] = prop.GetValue(arguments);
                }
                else if (param.IsOptional)
                {
                    values[i] = param.DefaultValue;
                }
                else
                {
                    values[i] = param.ParameterType.IsValueType
                        ? Activator.CreateInstance(param.ParameterType)
                        : null;
                }
            }

            return values;
        }

        // Optional: Clear reflection cache if needed (e.g., during development)
        public static void ClearReflectionCache()
        {
            ViewComponentMethodCache.ClearCache();
        }
    }
}
