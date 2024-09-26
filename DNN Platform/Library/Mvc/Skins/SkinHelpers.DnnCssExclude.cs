// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System.Web;
    using System.Web.Mvc;

    public static partial class SkinHelpers
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
