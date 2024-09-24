using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString CurrentDate(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var lblDate = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblDate.AddCssClass(cssClass);
            }

            lblDate.SetInnerText(DateTime.Now.ToString("D"));

            return new MvcHtmlString(lblDate.ToString());
        }
    }
}
