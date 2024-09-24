// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Modules.Html;

    public class HTMLController : Controller
    {
        public ActionResult HtmlModule(ModuleInfo module)
        {
            var ctrl = new HtmlTextController();

            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = ctrl.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = ctrl.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = string.Empty;
            if (content != null)
            {
                html = System.Web.HttpUtility.HtmlDecode(content.Content);
            }

            return this.View(new HtmlModuleModel()
            {
                Html = html,
            });
        }

        public ActionResult EditHTML(ModuleInfo module)
        {
            var ctrl = new HtmlTextController();

            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = ctrl.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = ctrl.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = System.Web.HttpUtility.HtmlDecode(content.Content);
            return this.View(new HtmlModuleModel()
            {
                Html = html,
            });
        }

        [HttpPost]
        public ActionResult SaveHTML(ModuleInfo module)
        {
            var input = this.Request.Form["Html"];
            var ctrl = new HtmlTextController();

            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = ctrl.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = ctrl.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = System.Web.HttpUtility.HtmlDecode(content.Content);
            return this.View(new HtmlModuleModel()
            {
                Html = html + "/" + input,
            });
        }
    }
}
