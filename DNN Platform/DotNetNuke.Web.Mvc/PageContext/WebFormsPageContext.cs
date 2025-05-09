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
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using DotNetNuke.UI.Skins.Controls;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;

    public class WebFormsPageContext : IPageContext
    {
        private Page page;
        private Control module;

        public WebFormsPageContext(Page page, Control module = null)
        {
            this.page = page;
            this.module = module;
        }

        public string Title
        {
            get
            {
                return this.page.Title;
            }

            set
            {
                this.page.Title = value;
            }
        }

        public void AddLiteral(string str)
        {
            if (this.module != null)
            {
                var litPartial = new LiteralControl(str);
                this.module.Controls.Add(litPartial);
            }
        }

        public void AddModuleMessage(string messsage, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.module, messsage, moduleMessageType);
        }

        public void ProcessModuleLoadException(string friendlyMessage, Exception exc)
        {
            DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(friendlyMessage, this.module, exc);
        }

        public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTag)
        {
            this.page.ClientScript.RegisterClientScriptBlock(type, key, script, addScriptTag);
        }

        public void RegisterScript(string filePath, FileOrder.Js priority = FileOrder.Js.DefaultPriority, string provider = "DnnBodyProvider")
        {
            ClientResourceManager.RegisterScript(this.page, filePath, priority, provider);
        }

        public void RegisterScript(string filePath, int jsOrder, string provider)
        {
            ClientResourceManager.RegisterScript(this.page, filePath, jsOrder, provider);
        }

        public void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
        {
            ScriptManager.RegisterStartupScript(this.page, this.GetType(), key, script, addScriptTags);
        }

        public void RegisterStyleSheet(string filePath, FileOrder.Css order = FileOrder.Css.DefaultPriority)
        {
            ClientResourceManager.RegisterStyleSheet(this.page, filePath, order);
        }

        public string ResolveUrl(string relativeFilePath)
        {
            return VirtualPathUtility.ToAbsolute(relativeFilePath);
        }

        public void SetPageDescription(string description)
        {
            var dnnpage = this.page as DotNetNuke.Framework.CDefault;
            if (dnnpage != null)
            {
                dnnpage.Header.Description = description;
                dnnpage.Description = description;
                dnnpage.MetaDescription = description;
                var metaDescription = (HtmlMeta)dnnpage.FindControl("Head").FindControl("MetaDescription");
                if (metaDescription != null)
                {
                    metaDescription.Visible = true;
                    metaDescription.Content = description;
                }
            }
        }

        public void IncludeTextInHeader(string textToInclude)
        {
            if (!string.IsNullOrEmpty(textToInclude))
            {
                this.page.Header.Controls.Add(new LiteralControl(textToInclude));
            }
        }
    }
}
