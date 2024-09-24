using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString LeftMenu(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper)
        {
            return new MvcHtmlString(string.Empty); // LeftMenu is deprecated and should return an empty string.
        }
    }
}
