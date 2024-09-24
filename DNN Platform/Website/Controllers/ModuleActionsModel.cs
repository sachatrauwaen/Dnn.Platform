// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using DotNetNuke.Entities.Modules;

    public class ModuleActionsModel
    {
        // public ModuleInstanceContext ModuleContext { get; internal set; }
        public ModuleInfo ModuleContext { get; set; }

        public bool SupportsQuickSettings { get; set; }

        public bool DisplayQuickSettings { get; set; }

        public object QuickSettingsModel { get; set; }

        public string CustomActionsJSON { get; set; }

        public string AdminActionsJSON { get; set; }

        public string Panes { get; set; }

        public string CustomText { get; set; }

        public string AdminText { get; set; }

        public string MoveText { get; set; }

        public bool SupportsMove { get; set; }

        public bool IsShared { get; set; }

        public string ModuleTitle { get; set; }
    }
}
