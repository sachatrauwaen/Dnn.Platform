using System;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;


namespace DotNetNuke.Web.Mvc.Skins
{
    using DotNetNuke.Abstractions;
    using DotNetNuke.Entities.Icons;

    public static partial class SkinExtensions
    {
        public static IHtmlString Search(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string cssClass = "SkinObject", bool showSite = true, bool showWeb = true, bool useWebForSite = false, bool useDropDownList = false, int minCharRequired = 2, int autoSearchDelayInMilliSecond = 400, bool enableWildSearch = true)
        {
            var portalSettings = PortalSettings.Current;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();

            var searchContainer = new TagBuilder("div");
            searchContainer.AddCssClass("SearchContainer");

            var searchBorder = new TagBuilder("div");
            searchBorder.AddCssClass("SearchBorder");

            var searchIcon = new TagBuilder("div");
            searchIcon.AddCssClass("SearchIcon");

            var downArrow = new TagBuilder("img");
            downArrow.Attributes.Add("src", IconController.IconURL("Action"));
            searchIcon.InnerHtml = downArrow.ToString();

            var searchInputContainer = new TagBuilder("span");
            searchInputContainer.AddCssClass("searchInputContainer");
            searchInputContainer.Attributes.Add("data-moreresults", Localization.GetSafeJSString("SeeMoreResults", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchInputContainer.Attributes.Add("data-noresult", Localization.GetSafeJSString("NoResult", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));

            var txtSearchNew = new TagBuilder("input");
            txtSearchNew.Attributes.Add("type", "text");
            txtSearchNew.AddCssClass("SearchTextBox");
            txtSearchNew.Attributes.Add("maxlength", "255");
            txtSearchNew.Attributes.Add("aria-label", "Search");
            txtSearchNew.Attributes.Add("autocomplete", "off");
            txtSearchNew.Attributes.Add("placeholder", Localization.GetSafeJSString("Placeholder", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchInputContainer.InnerHtml = txtSearchNew.ToString();

            var clearText = new TagBuilder("a");
            clearText.AddCssClass("dnnSearchBoxClearText");
            clearText.Attributes.Add("title", Localization.GetSafeJSString("SearchClearQuery", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchInputContainer.InnerHtml += clearText.ToString();

            searchBorder.InnerHtml = searchIcon.ToString() + searchInputContainer.ToString();

            var searchChoices = new TagBuilder("ul");
            searchChoices.AddCssClass("SearchChoices");

            var searchIconSite = new TagBuilder("li");
            searchIconSite.AddCssClass("SearchIconSite");
            searchIconSite.SetInnerText(Localization.GetString("Site", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchChoices.InnerHtml = searchIconSite.ToString();

            var searchIconWeb = new TagBuilder("li");
            searchIconWeb.AddCssClass("SearchIconWeb");
            searchIconWeb.SetInnerText(Localization.GetString("Web", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchChoices.InnerHtml += searchIconWeb.ToString();

            searchBorder.InnerHtml += searchChoices.ToString();

            var cmdSearchNew = new TagBuilder("button");
            cmdSearchNew.AddCssClass("SkinObject SearchButton");
            cmdSearchNew.SetInnerText(Localization.GetString("Search", Localization.GetResourceFile(helper.ViewContext.Controller, "Search.ascx")));
            searchBorder.InnerHtml += cmdSearchNew.ToString();

            searchContainer.InnerHtml = searchBorder.ToString();

            var script = new TagBuilder("script");
            script.Attributes.Add("type", "text/javascript");
            script.InnerHtml = @"
                $(function() {
                    if (typeof dnn != 'undefined' && typeof dnn.searchSkinObject != 'undefined') {
                        var searchSkinObject = new dnn.searchSkinObject({
                            delayTriggerAutoSearch : " + autoSearchDelayInMilliSecond + @",
                            minCharRequiredTriggerAutoSearch : " + minCharRequired + @",
                            searchType: 'S',
                            enableWildSearch: " + enableWildSearch.ToString().ToLowerInvariant() + @",
                            cultureCode: '" + System.Threading.Thread.CurrentThread.CurrentCulture.ToString() + @"',
                            portalId: " + portalSettings.PortalId + @"
                        });
                        searchSkinObject.init();

                        if (!" + useDropDownList.ToString().ToLowerInvariant() + @") {
                            var siteBtn = $('#" + searchIconSite.Attributes["id"] + @"');
                            var webBtn = $('#" + searchIconWeb.Attributes["id"] + @"');
                            var clickHandler = function() {
                                if (siteBtn.is(':checked')) searchSkinObject.settings.searchType = 'S';
                                else searchSkinObject.settings.searchType = 'W';
                            };
                            siteBtn.on('change', clickHandler);
                            webBtn.on('change', clickHandler);
                        } else {
                            if (typeof dnn.initDropdownSearch != 'undefined') {
                                dnn.initDropdownSearch(searchSkinObject);
                            }
                        }
                    }
                });
            ";

            return new MvcHtmlString(searchContainer.ToString() + script.ToString());
        }
    }
}
