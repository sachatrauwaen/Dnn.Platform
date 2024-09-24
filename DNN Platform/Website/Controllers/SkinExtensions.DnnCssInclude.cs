using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString DnnCssInclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string filePath, string pathNameAlias = "", int priority = 100, bool addTag = false, string name = "", string version = "", bool forceVersion = false, string forceProvider = "", bool forceBundle = false, string cssMedia = "")
        {
            var cssInclude = new TagBuilder("dnn:DnnCssInclude");
            cssInclude.Attributes.Add("ID", "ctlInclude");
            cssInclude.Attributes.Add("runat", "server");
            cssInclude.Attributes.Add("FilePath", filePath);
            cssInclude.Attributes.Add("PathNameAlias", pathNameAlias);
            cssInclude.Attributes.Add("Priority", priority.ToString());
            cssInclude.Attributes.Add("AddTag", addTag.ToString());
            cssInclude.Attributes.Add("Name", name);
            cssInclude.Attributes.Add("Version", version);
            cssInclude.Attributes.Add("ForceVersion", forceVersion.ToString());
            cssInclude.Attributes.Add("ForceProvider", forceProvider);
            cssInclude.Attributes.Add("ForceBundle", forceBundle.ToString());
            cssInclude.Attributes.Add("CssMedia", cssMedia);

            return new MvcHtmlString(cssInclude.ToString());
        }
    }
}
