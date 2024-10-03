// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Containers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Framework.Models;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc.Skins;

    public static partial class SkinHelpers
    {
        public static IHtmlString Content(this HtmlHelper<ContainerModel> htmlHelper)
        {
            var model = htmlHelper.ViewData.Model;
            if (model == null)
            {
                throw new InvalidOperationException("The model need to be present.");
            }

            var moduleContentPaneDiv = new TagBuilder("div");
            if (!string.IsNullOrEmpty(model.ContentPaneCssClass))
            {
                moduleContentPaneDiv.AddCssClass(model.ContentPaneCssClass);
            }

            if (!ModuleHostModel.IsViewMode(model.ModuleConfiguration, model.ModuleHost.PortalSettings) && htmlHelper.ViewContext.HttpContext.Request.QueryString["dnnprintmode"] != "true")
            {
                MvcJavaScript.RequestRegistration(CommonJs.DnnPlugins);
                if (model.EditMode && model.ModuleConfiguration.ModuleID > 0)
                {
                    moduleContentPaneDiv.InnerHtml += htmlHelper.Action("Index", "ModuleActions", model.ModuleConfiguration);
                }

                // register admin.css
                MvcClientResourceManager.RegisterAdminStylesheet(htmlHelper.ViewContext, Globals.HostPath + "admin.css");
            }

            if (!string.IsNullOrEmpty(model.ContentPaneStyle))
            {
                moduleContentPaneDiv.Attributes["style"] = model.ContentPaneStyle;
            }

            if (!string.IsNullOrEmpty(model.Header))
            {
                moduleContentPaneDiv.InnerHtml += model.Header;
            }

            var moduleDiv = new TagBuilder("div");
            moduleDiv.AddCssClass(model.ModuleHost.CssClass);

            try
            {
                moduleDiv.InnerHtml += htmlHelper.Action(model.ActionName, model.ControllerName, model.ModuleConfiguration);
            }
            catch (HttpException ex)
            {
                var scriptFolder = Path.GetDirectoryName(model.ModuleConfiguration.ModuleControl.ControlSrc);
                var fileRoot = Path.GetFileNameWithoutExtension(model.ModuleConfiguration.ModuleControl.ControlSrc);
                var srcPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptFolder, "_" + fileRoot + ".cshtml");
                var scriptFile = Path.Combine("~/" + scriptFolder, "Views/", "_" + fileRoot + ".cshtml");
                if (File.Exists(srcPhysicalPath))
                {
                    try
                    {
                        moduleDiv.InnerHtml += htmlHelper.Partial(scriptFile, model.ModuleConfiguration);
                    }
                    catch (Exception ex2)
                    {
                        throw new Exception($"Error : {ex2.Message} ( razor : {scriptFile}, module : {model.ModuleConfiguration.ModuleID})", ex2);
                    }
                }
                else
                {
                    // moduleDiv.InnerHtml += $"Error : {ex.Message} (Controller : {model.ControllerName}, Action : {model.ActionName}, module : {model.ModuleConfiguration.ModuleTitle}) {ex.StackTrace}";
                    throw new Exception($"Error : {ex.Message} (Controller : {model.ControllerName}, Action : {model.ActionName}, module : {model.ModuleConfiguration.ModuleID})", ex);
                }
            }
            catch (Exception ex)
            {
                    // moduleDiv.InnerHtml += $"Error : {ex.Message} (Controller : {model.ControllerName}, Action : {model.ActionName}, module : {model.ModuleConfiguration.ModuleTitle}) {ex.StackTrace}";
                    throw new Exception($"Error : {ex.Message} (Controller : {model.ControllerName}, Action : {model.ActionName}, module : {model.ModuleConfiguration.ModuleID})", ex);
            }

            moduleContentPaneDiv.InnerHtml += moduleDiv.ToString();
            if (!string.IsNullOrEmpty(model.Footer))
            {
                moduleContentPaneDiv.InnerHtml += model.Footer;
            }

            return MvcHtmlString.Create(moduleContentPaneDiv.InnerHtml);
        }
    }
}
