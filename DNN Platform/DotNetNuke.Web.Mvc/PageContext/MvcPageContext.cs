// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.PageContext
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.UI.Skins.Controls;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.MvcPipeline.Models;
    using DotNetNuke.Web.MvcPipeline.UI.Utilities;

    internal class MvcPageContext : IPageContext
    {
        private Controller controller;
        private PageModel model;

        public MvcPageContext(Controller controller)
        {
            this.controller = controller;
            this.model = this.controller.ControllerContext.ParentActionViewContext.ViewData.Model as PageModel;
        }

        public string Title
        {
            get
            {
                return this.model.Title;
            }

            set
            {
                this.model.Title = value;
            }
        }

        public void AddModuleMessage(string messsage, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            // DotNetNuke.Web.MvcPipeline.Models.SkinModel.AddModuleMessage(page, messsage, moduleMessageType);
        }

        public void IncludeTextInHeader(string textToInclude)
        {
           this.model.PortalHeadText += textToInclude;
        }

        public void ProcessModuleLoadException(string friendlyMessage, Exception exc)
        {
            // DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(friendlyMessage, page, exc);
        }

        public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTag)
        {
            MvcClientAPI.RegisterStartupScript(key, script);
        }

        public void RegisterClientVariable(string strVar, string strValue, bool overwrite)
        {
            MvcClientAPI.RegisterClientVariable(strVar, strValue, overwrite);
        }

        public void RegisterScript(string filePath, FileOrder.Js priority = FileOrder.Js.DefaultPriority, string provider = "DnnBodyProvider")
        {
            MvcClientResourceManager.RegisterScript(this.controller.ControllerContext, filePath, priority, provider);
        }

        public void RegisterScript(string filePath, int jsOrder, string provider)
        {
            MvcClientResourceManager.RegisterScript(this.controller.ControllerContext, filePath, jsOrder, provider);
        }

        public void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
        {
            MvcClientAPI.RegisterStartupScript(key, script);
        }

        public void RegisterStyleSheet(string filePath, FileOrder.Css order = FileOrder.Css.DefaultPriority)
        {
            MvcClientResourceManager.RegisterStyleSheet(this.controller.ControllerContext, filePath, order);
        }

        public string ResolveUrl(string relativeFilePath)
        {
            return VirtualPathUtility.ToAbsolute(relativeFilePath);
        }

        public void SetPageDescription(string metaDescription)
        {
            this.model.Description = metaDescription;
        }
    }
}
