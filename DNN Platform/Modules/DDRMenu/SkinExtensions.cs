// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.DDRMenu
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Web.WebPages.Html;

    using DotNetNuke.UI;
    using DotNetNuke.Web.DDRMenu;
    using DotNetNuke.Web.DDRMenu.Localisation;
    using DotNetNuke.Web.DDRMenu.TemplateEngine;

    public static class SkinExtensions
    {
        public static string DDRMenu(
                                        string clientID,
                                        string menuStyle,
                                        string nodeXmlPath = "",
                                        string nodeSelector = "*",
                                        bool includeContext = false,
                                        bool includeHidden = false,
                                        string includeNodes = "",
                                        string excludeNodes = "",
                                        string nodeManipulator = "",
                                        List<ClientOption> clientOptions = null,
                                        List<TemplateArgument> templateArguments = null)
        {
            MvcMenuBase menu;
            menu = MvcMenuBase.Instantiate(menuStyle);
            menu.ApplySettings(
                new Settings
                {
                    MenuStyle = menuStyle,
                    NodeXmlPath = nodeXmlPath,
                    NodeSelector = nodeSelector,
                    IncludeContext = includeContext,
                    IncludeHidden = includeHidden,
                    IncludeNodes = includeNodes,
                    ExcludeNodes = excludeNodes,
                    NodeManipulator = nodeManipulator,
                    ClientOptions = clientOptions,
                    TemplateArguments = templateArguments,
                });

            if (string.IsNullOrEmpty(nodeXmlPath))
            {
                menu.RootNode =
                    new MenuNode(
                        Localiser.LocaliseDNNNodeCollection(
                            Navigation.GetNavigationNodes(
                                clientID,
                                Navigation.ToolTipSource.None,
                                -1,
                                -1,
                                DNNAbstract.GetNavNodeOptions(true))));
            }

            menu.PreRender();

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                menu.Render(writer);
            }

            return stringWriter.ToString();
        }
    }
}
