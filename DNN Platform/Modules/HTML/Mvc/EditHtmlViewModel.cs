// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Html.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using DotNetNuke.Web.Mvc.Page;

    using static DotNetNuke.Framework.Controllers.HTMLController;

    public class EditHtmlViewModel : ModuleModelBase
    {
        [AllowHtml]
        public string EditorContent { get; set; }

        public bool EditorEnabled { get; set; }

        public bool UseDecorate { get; set; }

       // public List<ToolbarButtonViewModel> ToolbarButtons { get; set; }
        public string CurrentView { get; set; }

        public bool ShowEditView { get; set; }

        public bool ShowMasterContent { get; set; }

        public string MasterContent { get; set; }

        public bool ShowCurrentVersion { get; set; }

        public string CurrentWorkflowState { get; set; }

        public string CurrentVersion { get; set; }

        public string CurrentWorkflowInUse { get; set; }

        public bool ShowPublishOption { get; set; }

        public bool Publish { get; set; }

        public WorkflowType WorkflowType { get; set; }

        public int MaxVersions { get; set; }

        public bool ShowCurrentWorkflowState { get; set; }

        public List<SelectListItem> RenderOptions { get; set; }

        public bool ShowPreviewVersion { get; set; }

        public bool ShowPreviewView { get; set; }

        public string RedirectUrl { get; set; }
    }
}
