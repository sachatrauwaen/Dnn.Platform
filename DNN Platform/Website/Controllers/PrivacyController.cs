// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System.Web.Mvc;
    using DotNetNuke.Services.Localization;

    public class PrivacyController : DnnPageController
    {
        private readonly ILocalization _localization;

        public PrivacyController(ILocalization localization)
        {
            _localization = localization;
        }

        [Authorize]
        public ActionResult Index()
        {
            var privacy = _localization.GetSystemMessage(this.PortalSettings, "MESSAGE_PORTAL_PRIVACY");
            return this.View("Index", string.Empty, privacy);
        }
    }
}