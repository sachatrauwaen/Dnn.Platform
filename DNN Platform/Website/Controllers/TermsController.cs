﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System.Web.Mvc;
    using DotNetNuke.Services.Localization;

    public class TermsController : DnnPageController
    {
        private readonly ILocalization _localization;

        public TermsController(ILocalization localization)
        {
            _localization = localization;
        }

        [Authorize]
        public ActionResult Index()
        {
            var terms = _localization.GetSystemMessage(this.PortalSettings, "MESSAGE_PORTAL_TERMS");
            return this.View("Index", string.Empty, terms);
        }
    }
}
