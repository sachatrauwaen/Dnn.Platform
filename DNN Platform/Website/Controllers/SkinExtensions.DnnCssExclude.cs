using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString DnnCssExclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name)
        {
            var cssExclude = new TagBuilder("dnn:DnnCssExclude");
            cssExclude.Attributes.Add("ID", "ctlExclude");
            cssExclude.Attributes.Add("runat", "server");
            cssExclude.Attributes.Add("Name", name);

            return new MvcHtmlString(cssExclude.ToString());
        }
    }
}
