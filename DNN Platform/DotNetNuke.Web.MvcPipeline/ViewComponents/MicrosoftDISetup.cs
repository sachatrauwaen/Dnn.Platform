using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetNuke.Web.MvcPipeline.ViewComponents
{
    // Helper for setting up Microsoft DI in MVC5
    public static class MicrosoftDISetup
    {
        public static void AddViewComponenServices(this IServiceCollection services)
        {

            // Register all ViewComponents
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(BaseViewComponent)) && !t.IsAbstract);

            foreach (var type in componentTypes)
            {
                services.AddTransient(type);
            }
        }
    }
}
