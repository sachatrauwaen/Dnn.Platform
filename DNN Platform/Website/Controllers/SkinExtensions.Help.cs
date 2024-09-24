using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    using DotNetNuke.Entities.Portals;

    public static partial class SkinExtensions
    {
        public static IHtmlString Help(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "")
        {
            var portalSettings = PortalSettings.Current;
            var link = new TagBuilder("a");
            link.Attributes.Add("href", "mailto:" + portalSettings.Email + "?subject=" + portalSettings.PortalName + " Support Request");
            link.Attributes.Add("class", cssClass);
            link.SetInnerText("Help");

            return new MvcHtmlString(link.ToString());
        }
    }
}
