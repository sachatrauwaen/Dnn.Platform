using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString DnnJsExclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name)
        {
            var jsExclude = new TagBuilder("dnn:DnnJsExclude");
            jsExclude.Attributes.Add("ID", "ctlExclude");
            jsExclude.Attributes.Add("runat", "server");
            jsExclude.Attributes.Add("Name", name);

            return new MvcHtmlString(jsExclude.ToString());
        }
    }
}
