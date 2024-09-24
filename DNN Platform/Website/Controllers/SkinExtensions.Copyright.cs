using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;

    public static partial class SkinExtensions
    {
        public static IHtmlString Copyright(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var portalSettings = PortalSettings.Current;
            var lblCopyright = new TagBuilder("span");

            if (!string.IsNullOrEmpty(cssClass))
            {
                lblCopyright.AddCssClass(cssClass);
            }

            if (!string.IsNullOrEmpty(portalSettings.FooterText))
            {
                lblCopyright.SetInnerText(portalSettings.FooterText.Replace("[year]", DateTime.Now.ToString("yyyy")));
            }
            else
            {
                lblCopyright.SetInnerText(string.Format(Localization.GetString("Copyright", Localization.GetResourceFile(helper.ViewContext.Controller, "Copyright.ascx")), DateTime.Now.Year, portalSettings.PortalName));
            }

            return new MvcHtmlString(lblCopyright.ToString());
        }
    }
}
