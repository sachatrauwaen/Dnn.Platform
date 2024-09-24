// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    public static partial class SkinExtensions
    {
        public static IHtmlString LeftMenu(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper)
        {
            return new MvcHtmlString(string.Empty); // LeftMenu is deprecated and should return an empty string.
        }
    }
}
