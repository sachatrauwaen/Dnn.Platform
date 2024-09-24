using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString Meta(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name = "", string content = "", string httpEquiv = "", bool insertFirst = false)
        {
            var metaTag = new TagBuilder("meta");

            if (!string.IsNullOrEmpty(name))
            {
                metaTag.Attributes.Add("name", name);
            }

            if (!string.IsNullOrEmpty(content))
            {
                metaTag.Attributes.Add("content", content);
            }

            if (!string.IsNullOrEmpty(httpEquiv))
            {
                metaTag.Attributes.Add("http-equiv", httpEquiv);
            }

            return new MvcHtmlString(metaTag.ToString());
        }
    }
}
