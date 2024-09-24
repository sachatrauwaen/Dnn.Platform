using System;
using System.Web;
using System.Web.Mvc;


namespace DotNetNuke.Web.Mvc.Skins
{
    public static partial class SkinExtensions
    {
        public static IHtmlString Text(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string showText = "", string cssClass = "", string resourceKey = "", bool replaceTokens = false)
        {
            var portalSettings = PortalSettings.Current;
            var text = showText;

            if (!string.IsNullOrEmpty(resourceKey))
            {
                var file = Path.GetFileName(helper.ViewContext.HttpContext.Server.MapPath(portalSettings.ActiveTab.SkinSrc));
                file = portalSettings.ActiveTab.SkinPath + Localization.LocalResourceDirectory + "/" + file;
                var localization = Localization.GetString(resourceKey, file);
                if (!string.IsNullOrEmpty(localization))
                {
                    text = localization;
                }
            }

            if (replaceTokens)
            {
                var tr = new TokenReplace { AccessingUser = portalSettings.UserInfo };
                text = tr.ReplaceEnvironmentTokens(text);
            }

            var label = new TagBuilder("span");
            label.SetInnerText(text);
            if (!string.IsNullOrEmpty(cssClass))
            {
                label.AddCssClass(cssClass);
            }

            return new MvcHtmlString(label.ToString());
        }
    }
}