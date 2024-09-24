using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString JavaScriptLibraryInclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name, string version = "", string specificVersion = "")
        {
            var script = new TagBuilder("script");
            script.Attributes.Add("src", name);
            script.Attributes.Add("type", "text/javascript");

            if (!string.IsNullOrEmpty(version))
            {
                script.Attributes.Add("data-version", version);
            }

            if (!string.IsNullOrEmpty(specificVersion))
            {
                script.Attributes.Add("data-specific-version", specificVersion);
            }

            return new MvcHtmlString(script.ToString());
        }
    }
}
