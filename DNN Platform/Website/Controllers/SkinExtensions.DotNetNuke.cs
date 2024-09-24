using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    using DotNetNuke.Application;
    using DotNetNuke.Entities.Portals;

    public static partial class SkinExtensions
    {
        public static IHtmlString DotNetNuke(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "")
        {
            var portalSettings = PortalSettings.Current;
            var link = new TagBuilder("a");
            link.Attributes.Add("href", DotNetNukeContext.Current.Application.Url);
            link.Attributes.Add("class", cssClass);
            link.SetInnerText(DotNetNukeContext.Current.Application.LegalCopyright);

            return new MvcHtmlString(link.ToString());
        }
    }
}
