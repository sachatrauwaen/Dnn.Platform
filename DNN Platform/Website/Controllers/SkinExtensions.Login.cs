using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Mvc.Skins
{
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;

    public static partial class SkinExtensions
    {
        public static IHtmlString Login(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject")
        {
            var portalSettings = PortalSettings.Current;
            var link = new TagBuilder("a");

            link.Attributes.Add("href", portalSettings.PortalAlias.HTTPAlias);
            link.SetInnerText(Localization.GetString("Login.Text", Localization.GetResourceFile(helper.ViewContext.Controller, "Login.ascx")));

            return new MvcHtmlString(link.ToString());
        }
    }
}
