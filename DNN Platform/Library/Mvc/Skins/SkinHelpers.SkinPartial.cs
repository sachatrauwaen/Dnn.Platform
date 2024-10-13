// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.IO;
    using System.Web;

    using Dnn.Migration;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static partial class SkinHelpers
    {
        public static IHtmlContent SkinPartial(this HtmlHelper<DotNetNuke.Framework.Models.PageModel> helper, string name = "")
        {
            var model = helper.ViewData.Model;
            if (model == null)
            {
                throw new InvalidOperationException("The model need to be present.");
            }

            var skinPath = Path.GetDirectoryName(model.Skin.SkinSrc);
            return AsyncHelper.RunSync(() => helper.PartialAsync("~" + skinPath + "/Views/" + name + ".cshtml"));
        }
    }
}
