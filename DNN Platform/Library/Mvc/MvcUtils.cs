// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DotNetNuke.Entities.Modules;

    public class MvcUtils
    {
        public static string GetControlViewName(ModuleInfo module)
        {
            return GetControlViewName(module, Path.GetFileNameWithoutExtension(module.ModuleControl.ControlSrc));
        }

        public static string GetControlViewName(ModuleInfo module, string viewName)
        {
            return "~/" + Path.GetDirectoryName(module.ModuleControl.ControlSrc) + "/Views/" + viewName + ".cshtml";
        }
    }
}
