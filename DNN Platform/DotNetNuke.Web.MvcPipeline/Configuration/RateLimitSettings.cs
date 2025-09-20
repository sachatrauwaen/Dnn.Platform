// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.Configuration
{
    using DotNetNuke.Common.Utilities;

    /// <summary>
    /// Configuration settings for rate limiting functionality.
    /// </summary>
    public static class RateLimitSettings
    {
        /// <summary>
        /// Gets the maximum number of CSP reports allowed per IP address within the time window.
        /// </summary>
        public static int CspReportMaxRequests
        {
            get
            {
                // Get from web.config appSettings, default to 5
                var webConfigSetting = Config.GetSetting("CSPReportRateLimitMaxRequests");
                if (!string.IsNullOrEmpty(webConfigSetting) && int.TryParse(webConfigSetting, out var webConfigValue))
                {
                    return webConfigValue;
                }

                return 5; // Default value
            }
        }

        /// <summary>
        /// Gets the time window in minutes for CSP report rate limiting.
        /// </summary>
        public static int CspReportTimeWindowMinutes
        {
            get
            {
                // Get from web.config appSettings, default to 1 minute
                var webConfigSetting = Config.GetSetting("CSPReportRateLimitTimeWindowMinutes");
                if (!string.IsNullOrEmpty(webConfigSetting) && int.TryParse(webConfigSetting, out var webConfigValue))
                {
                    return webConfigValue;
                }

                return 1; // Default value
            }
        }

        /// <summary>
        /// Gets a value indicating whether CSP report rate limiting is enabled.
        /// </summary>
        public static bool CspReportRateLimitEnabled
        {
            get
            {
                // Get from web.config appSettings, default to true
                var webConfigSetting = Config.GetSetting("CSPReportRateLimitEnabled");
                if (!string.IsNullOrEmpty(webConfigSetting) && bool.TryParse(webConfigSetting, out var webConfigValue))
                {
                    return webConfigValue;
                }

                return true; // Default value
            }
        }
    }
}
