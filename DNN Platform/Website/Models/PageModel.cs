// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Framework.Models
{
    using System.Collections.Generic;

    using DotNetNuke.Web.Mvc.Skins;

    public class PageModel
    {
        public int? TabId { get; set; }

        public string Language { get; set; }

        public int? PortalId { get; internal set; }

        public MvcSkin Skin { get; internal set; }

        public string AntiForgery { get; internal set; }

        public Dictionary<string, string> ClientVariables { get; set; }

        public string PageHeadText { get; internal set; }

        public string PortalHeadText { get; internal set; }

        public string Title { get; internal set; }

        public string BackgroundUrl { get; internal set; }

        public string MetaRefresh { get; internal set; }

        public string Description { get; internal set; }

        public string KeyWords { get; internal set; }

        public string Copyright { get; internal set; }

        public string Generator { get; internal set; }

        public string MetaRobots { get; internal set; }

        // public bool EditMode { get; internal set; }
        public Dictionary<string, string> StartupScripts { get; internal set; }

        public bool IsEditMode { get; internal set; }
    }
}
