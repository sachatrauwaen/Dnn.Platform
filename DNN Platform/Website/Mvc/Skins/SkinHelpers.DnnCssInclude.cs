// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Net.NetworkInformation;
    using System.Web;
    using System.Web.Mvc;

    using ClientDependency.Core.Mvc;

    public static partial class SkinHelpers
    {
        public static IHtmlString DnnCssInclude(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string filePath, string pathNameAlias = "", int priority = 100, bool addTag = false, string name = "", string version = "", bool forceVersion = false, string forceProvider = "", bool forceBundle = false, string cssMedia = "")
        {
            helper.RequiresCss(filePath, priority);

            var cssInclude = new TagBuilder("div");
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

            // return new MvcHtmlString(cssInclude.ToString());
            return new MvcHtmlString($"<!-- DnnCssInclude FilePath={filePath}, Priority={priority} -->");
        }
    }
}
