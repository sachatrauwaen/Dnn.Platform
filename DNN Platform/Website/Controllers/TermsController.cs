// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;

    using ClientDependency.Core.Mvc;
    using Dnn.EditBar.UI.Mvc;
    using DotNetNuke.Application;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Framework.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Installer.Blocker;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Personalization;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.Utilities;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc.Framework.ActionFilters;
    using DotNetNuke.Web.Mvc.Skins;
    using DotNetNuke.Web.Mvc.Skins.Controllers;

    using Globals = DotNetNuke.Common.Globals;

    public class DefaultController : DnnPageController
    {
        [Authorize]
        public ActionResult Index(ModuleInfo module)
        {
            var terms = Localization.GetSystemMessage(this.PortalSettings, "MESSAGE_PORTAL_TERMS");
            return this.View("Terms", string.Empty, terms);
        }
    }
}
