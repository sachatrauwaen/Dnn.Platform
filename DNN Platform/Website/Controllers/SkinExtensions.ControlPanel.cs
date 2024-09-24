using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString ControlPanel(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var lblControlPanel = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblControlPanel.AddCssClass(cssClass);
            }

            lblControlPanel.SetInnerText(Localization.GetString("ControlPanel", Localization.GetResourceFile(helper.ViewContext.Controller, "ControlPanel.ascx")));

            return new MvcHtmlString(lblControlPanel.ToString());
        }
    }
}
