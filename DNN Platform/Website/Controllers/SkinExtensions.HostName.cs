using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    using DotNetNuke.Entities.Host;

    public static partial class SkinExtensions
    {
        public static IHtmlString HostName(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "")
        {
            var link = new TagBuilder("a");
            link.Attributes.Add("href", Globals.AddHTTP(Host.HostURL));
            link.Attributes.Add("class", cssClass);
            link.SetInnerText(Host.HostTitle);

            return new MvcHtmlString(link.ToString());
        }
    }
}
