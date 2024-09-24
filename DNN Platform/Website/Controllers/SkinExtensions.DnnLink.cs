using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString DnnLink(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "", string target = "")
        {
            var link = new TagBuilder("a");
            link.Attributes.Add("href", "http://www.dnnsoftware.com/community?utm_source=dnn-install&utm_medium=web-link&utm_content=gravity-skin-link&utm_campaign=dnn-install");
            link.Attributes.Add("class", cssClass);
            link.Attributes.Add("target", target);
            link.SetInnerText("CMS by DNN");

            return new MvcHtmlString(link.ToString());
        }
    }
}
