// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.PageContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DotNetNuke.UI.Skins.Controls;
    using DotNetNuke.Web.Client;

    public interface IPageContext
    {
        string Title { get; set; }

        void AddModuleMessage(string v, ModuleMessage.ModuleMessageType redError);

        void ProcessModuleLoadException(string friendlyMessage, Exception exc);

        void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTag);

        void RegisterScript(string filePath, int jsOrder, string provider = "DnnBodyProvider");

        void RegisterScript(string filePath, FileOrder.Js jsOrder = FileOrder.Js.DefaultPriority, string provider = "DnnBodyProvider");

        void RegisterStartupScript(Type type, string key, string script, bool addScriptTags);

        void RegisterStyleSheet(string v, FileOrder.Css portalCss = FileOrder.Css.DefaultPriority);

        string ResolveUrl(string relativeFilePath);

        void SetPageDescription(string metaDescription);

        void IncludeTextInHeader(string textToInclude);
    }
}
