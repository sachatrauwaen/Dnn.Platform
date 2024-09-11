// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Razor.Parser.SyntaxTree;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.FileSystem;

    using Microsoft.Extensions.DependencyInjection;

    public static class SkinExtensions
    {
        public static System.Web.IHtmlString Logo(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string borderWidth = "", string cssClass = "", string linkCssClass = "")
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();

            TagBuilder tbImage = new TagBuilder("img");
            if (!string.IsNullOrEmpty(borderWidth))
            {
                // this.imgLogo.BorderWidth = Unit.Parse(this.BorderWidth);
            }

            if (!string.IsNullOrEmpty(cssClass))
            {
                // this.imgLogo.CssClass = this.CssClass;
            }

            if (!string.IsNullOrEmpty(linkCssClass))
            {
                // this.hypLogo.CssClass = linkCssClass;
            }

            if (!string.IsNullOrEmpty(portalSettings.LogoFile))
            {
                var fileInfo = GetLogoFileInfo(portalSettings);
                if (fileInfo != null)
                {
                    /*
                    if (this.InjectSvg && "svg".Equals(fileInfo.Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        this.litLogo.Text = this.GetSvgContent(fileInfo);
                        this.litLogo.Visible = !string.IsNullOrEmpty(this.litLogo.Text);
                    }
                    */
                    string imageUrl = FileManager.Instance.GetUrl(fileInfo);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        tbImage.Attributes.Add("src", imageUrl);
                    }
                }
            }

            tbImage.Attributes.Add("alt", portalSettings.PortalName);
            TagBuilder tbLink = new TagBuilder("a");
            tbLink.Attributes.Add("title", portalSettings.PortalName);
            tbLink.Attributes.Add("aria-label", portalSettings.PortalName);

            if (portalSettings.HomeTabId != -1)
            {
                tbLink.Attributes.Add("href", navigationManager.NavigateURL(portalSettings.HomeTabId));
            }
            else
            {
                tbLink.Attributes.Add("href", Globals.AddHTTP(portalSettings.PortalAlias.HTTPAlias));
            }

            tbLink.InnerHtml = tbImage.ToString();
            return new MvcHtmlString(tbLink.ToString());
        }

        private static IFileInfo GetLogoFileInfo(PortalSettings portalSettings)
        {
            string cacheKey = string.Format(DataCache.PortalCacheKey, portalSettings.PortalId, portalSettings.CultureCode) + "LogoFile";
            var file = CBO.GetCachedObject<FileInfo>(
                new CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority),
                (CacheItemArgs itemArgs) =>
                {
                    return FileManager.Instance.GetFile(portalSettings.PortalId, portalSettings.LogoFile);
                });

            return file;
        }
    }
}
