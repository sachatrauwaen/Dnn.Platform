#region Copyright
// 
// DotNetNukeŽ - http://www.dotnetnuke.com
// Copyright (c) 2002-2013
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs.Internal;
using DotNetNuke.Entities.Urls;
using DotNetNuke.Entities.Users;
using DotNetNuke.HttpModules.UrlRewrite;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Tests.Utilities;

using NUnit.Framework;

namespace DotNetNuke.Tests.Urls
{
    [TestFixture]
    public class UrlRewriteTests : UrlTestBase 
    {
        private const string _defaultPage = Globals.glbDefaultPage;
        private int _tabId;
        private const string _testPage = "Test Page";
        private const string _aboutUsPageName = "About Us";
        private string _redirectMode;
        private Locale _customLocale;
        private string _securePageName;
        private PortalAliasInfo _primaryAlias;
        private bool _sslEnforced;
        private bool _sslEnabled;

        public UrlRewriteTests() : base(0) { }

        #region Private Methods

        private void CreateSimulatedRequest(Uri url)
        {
            var simulator = new Instance.Utilities.HttpSimulator.HttpSimulator();
            simulator.SimulateRequest(url);

            var browserCaps = new HttpBrowserCapabilities { Capabilities = new Hashtable() };
            HttpContext.Current.Request.Browser = browserCaps;
        }

        private void ProcessRequest(FriendlyUrlSettings settings, UrlTestHelper testHelper)
        {
            var provider = new AdvancedUrlRewriter();

            provider.ProcessTestRequestWithContext(HttpContext.Current,
                                    HttpContext.Current.Request.Url,
                                    true,
                                    testHelper.Result,
                                    settings);
            testHelper.Response = HttpContext.Current.Response;
        }

        private string ReplaceTokens(string url, string httpAlias, string defaultAlias, string tabName, string tabId, string portalId, string vanityUrl, string userId)
        {
            return url.Replace("{alias}", httpAlias)
                            .Replace("{usealias}", defaultAlias)
                            .Replace("{tabName}", tabName)
                            .Replace("{tabId}", tabId)
                            .Replace("{portalId}", portalId)
                            .Replace("{vanityUrl}", vanityUrl)
                            .Replace("{userId}", userId)
                            .Replace("{defaultPage}", _defaultPage);
        }

        #endregion

        #region SetUp and TearDown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DeleteTab(_testPage);
            CreateTab(_testPage);
            UpdateTabName(_tabId, "About Us");
            UpdateTabSkin(_tabId,  "");
            CacheController.FlushPageIndexFromCache();
            GetDefaultAlias();
            _redirectMode = PortalController.GetPortalSetting("PortalAliasMapping", PortalId, "CANONICALURL");
            _sslEnforced = PortalController.GetPortalSettingAsBoolean("SSLEnforced", PortalId, false);
            _sslEnabled = PortalController.GetPortalSettingAsBoolean("SSLEnabled", PortalId, false);
            _primaryAlias = null;
            _customLocale = null;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            DeleteTab(_testPage);
            UpdateTabName(_tabId, "About Us");
            UpdateTabSkin(_tabId, "[G]Skins/Gravity/2-Col.ascx");

            if (!String.IsNullOrEmpty(_securePageName))
            {
                var tc = new TabController();
                var tab = tc.GetTabByName(_securePageName, PortalId);
                if (tab != null)
                {
                    tab.IsSecure = false;

                    UpdateTab(tab);
                }
            }

            if (_customLocale != null)
            {
                Localization.RemoveLanguageFromPortals(_customLocale.LanguageId);
                Localization.DeleteLanguage(_customLocale);
            }
            if (_primaryAlias != null)
            {
                TestablePortalAliasController.Instance.DeletePortalAlias(_primaryAlias);
            }

            SetDefaultAlias(DefaultAlias);
            PortalController.UpdatePortalSetting(PortalId, "PortalAliasMapping", _redirectMode);
            PortalController.UpdatePortalSetting(PortalId, "SSLEnforced", _sslEnforced.ToString());
            PortalController.UpdatePortalSetting(PortalId, "SSLEnabled", _sslEnabled.ToString());

            foreach (var tabUrl in CBO.FillCollection<TabUrlInfo>(DataProvider.Instance().GetTabUrls(PortalId)))
            {
                TestableTabController.Instance.DeleteTabUrl(tabUrl, PortalId, true);
            }

        }

        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            var tc = new TabController();
            var tab = tc.GetTabByName(_aboutUsPageName, PortalId);
            _tabId = tab.TabID;

            //Add Portal Aliases
            var aliasController = new PortalAliasController();
            TestUtil.ReadStream(String.Format("{0}", "Aliases"), (line, header) =>
                            {
                                string[] fields = line.Split(',');
                                var alias = aliasController.GetPortalAlias(fields[0], PortalId);
                                if (alias == null)
                                {
                                    alias = new PortalAliasInfo
                                    {
                                        HTTPAlias = fields[0],
                                        PortalID = PortalId
                                    };
                                    TestablePortalAliasController.Instance.AddPortalAlias(alias);
                                }
                            });
            TestUtil.ReadStream(String.Format("{0}", "Users"), (line, header) =>
                                {
                                    string[] fields = line.Split(',');

                                    TestUtil.AddUser(PortalId, fields[0].Trim(), fields[1].Trim(), fields[2].Trim());
                                });
        }

        [TestFixtureTearDown]
        public override void TestFixtureTearDown()
        {
            base.TestFixtureTearDown();

            var aliasController = new PortalAliasController();
            TestUtil.ReadStream(String.Format("{0}", "Aliases"), (line, header) =>
                            {
                                string[] fields = line.Split(',');
                                var alias = aliasController.GetPortalAlias(fields[0], PortalId);
                                TestablePortalAliasController.Instance.DeletePortalAlias(alias);
                            });
            TestUtil.ReadStream(String.Format("{0}", "Users"), (line, header) =>
                            {
                                string[] fields = line.Split(',');

                                TestUtil.DeleteUser(PortalId, fields[0]);
                            });

        }

        #endregion

        #region Private Methods

        private TabInfo CreateTab(string tabName)
        {
            var tc = new TabController();
            var tab = new TabInfo { PortalID = PortalId, TabName = tabName };

            tc.AddTab(tab);

            return tab;
        }

        private void DeleteTab(string tabName)
        {
            var tc = new TabController();
            var tab = tc.GetTabByName(tabName, PortalId);

            if (tab != null)
            {
                tc.DeleteTab(tab.TabID, PortalId);
            }
        }

        private void ExecuteTestForTab(TabInfo tab, FriendlyUrlSettings settings, Dictionary<string, string> testFields)
        {
            var defaultAlias = testFields.GetValue("DefaultAlias", String.Empty);
            var httpAlias = testFields.GetValue("Alias", String.Empty);
            var tabName = testFields["Page Name"];
            var scheme = testFields["Scheme"];
            var url = testFields["Test Url"];
            var result = testFields["Expected Url"];
            var expectedStatus = Int32.Parse(testFields["Status"]);
            var redirectUrl = testFields.GetValue("Final Url");
            var redirectReason = testFields.GetValue("RedirectReason");
            var vanityUrl = testFields.GetValue("VanityUrl", String.Empty);

            var tabID = (tab == null) ? "-1" : tab.TabID.ToString();

            var userName = testFields.GetValue("UserName", String.Empty);
            string userId = String.Empty;
            if (!String.IsNullOrEmpty(userName))
            {
                var user = UserController.GetUserByName(PortalId, userName);
                if (user != null)
                {
                    userId = user.UserID.ToString();
                }
            }

            var expectedResult = ReplaceTokens(result, httpAlias, defaultAlias, tabName, tabID, PortalId.ToString(), vanityUrl, userId);
            var testurl = ReplaceTokens(url, httpAlias, defaultAlias, tabName, tabID, PortalId.ToString(), vanityUrl, userId);
            var expectedRedirectUrl = ReplaceTokens(redirectUrl, httpAlias, defaultAlias, tabName, tabID, PortalId.ToString(), vanityUrl, userId);

            CreateSimulatedRequest(new Uri(testurl));

            var request = HttpContext.Current.Request;
            var testHelper = new UrlTestHelper
                    {
                        HttpAliasFull = scheme + httpAlias + "/",
                        //Result = new UrlAction(scheme, scheme + httpAlias, Globals.ApplicationMapPath)
                        //            {
                        //                IsSecureConnection = HttpContext.Current.Request.IsSecureConnection,
                        //                RawUrl = HttpContext.Current.Request.RawUrl
                        //            },
                        Result = new UrlAction(request)
                                        {
                                            IsSecureConnection = request.IsSecureConnection,
                                            RawUrl = request.RawUrl
                                        },
                        RequestUri = new Uri(testurl),
                        QueryStringCol = new NameValueCollection()
                    };

            ProcessRequest(settings, testHelper);

            //Test expected response status
            Assert.AreEqual(expectedStatus, testHelper.Response.StatusCode);

            switch (expectedStatus)
            {
                case 200:
                    //Test expected rewrite path
                    if (!String.IsNullOrEmpty(expectedResult))
                    {
                        Assert.AreEqual(expectedResult, testHelper.Result.RewritePath);
                    }
                    break;
                case 301:
                case 302:
                    //Test for final Url if redirected
                    Assert.AreEqual(expectedRedirectUrl, testHelper.Result.FinalUrl);
                    Assert.AreEqual(redirectReason, testHelper.Result.Reason.ToString(), "Redirect reason incorrect");
                    break;
            }
        }

        private void ExecuteTest(FriendlyUrlSettings settings, Dictionary<string, string> testFields, bool setDefaultAlias)
        {
            var tabName = testFields["Page Name"];
            var tc = new TabController();
            var tab = tc.GetTabByName(tabName, PortalId);

            if (setDefaultAlias)
            {
                SetDefaultAlias(testFields);
            }

            ExecuteTestForTab(tab, settings, testFields);
        }

        private void UpdateTab(TabInfo tab)
        {
            var tc = new TabController();
            if (tab != null)
            {
                tc.UpdateTab(tab);
            }
            
        }

        private void UpdateTabName(int tabId, string newName)
        {
            var tc = new TabController();
            var tab = tc.GetTab(tabId, PortalId, false);
            tab.TabName = newName;
            tc.UpdateTab(tab);
        }

        private void UpdateTabSkin(int tabId, string newSkin)
        {
            var tc = new TabController();
            var tab = tc.GetTab(tabId, PortalId, false);
            tab.SkinSrc = newSkin;
            tc.UpdateTab(tab);
        }

        #endregion

        #region Tests

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_BasicTestCases")]
        public void AdvancedUrlRewriter_BasicTest(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            settings.PortalId = PortalId;

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_DeletedTabHandlingTestCases")]
        public void AdvancedUrlRewriter_DeletedTabHandling(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            settings.PortalId = PortalId;

            var tc = new TabController();
            var tab = tc.GetTabByName(_testPage, PortalId);
            if (Convert.ToBoolean(testFields["HardDeleted"]))
            {
                DeleteTab(_testPage);
                CacheController.FlushPageIndexFromCache();
            }
            else
            {
                tab.IsDeleted = Convert.ToBoolean(testFields["SoftDeleted"]);
                tab.DisableLink = Convert.ToBoolean(testFields["Disabled"]);
                if (Convert.ToBoolean(testFields["Expired"]))
                {
                    tab.EndDate = DateTime.Now - TimeSpan.FromDays(1);
                }
                UpdateTab(tab);
            }

            string deletedTabHandling = testFields.GetValue("DeletedTabHandling");

            if (!String.IsNullOrEmpty(deletedTabHandling))
            {
                switch (deletedTabHandling)
                {
                    case "Do404Error":
                        settings.DeletedTabHandlingType = DeletedTabHandlingType.Do404Error;
                        break;
                    default:
                        settings.DeletedTabHandlingType = DeletedTabHandlingType.Do301RedirectToPortalHome;
                        break;
                }
            }

            SetDefaultAlias(testFields);

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_ForwardExternalUrlTestCases")]
        public void AdvancedUrlRewriter_ForwardExternalUrls(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            settings.PortalId = PortalId;

            var tc = new TabController();
            var tab = tc.GetTabByName(_testPage, PortalId);
            tab.Url = testFields["ExternalUrl"];
            tc.UpdateTab(tab);

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_ForceLowerCaseTestCases")]
        public void AdvancedUrlRewriter_ForceLowerCase(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            string forceLowerCaseRegex = testFields.GetValue("ForceLowerCaseRegex");

            if (!String.IsNullOrEmpty(forceLowerCaseRegex))
            {
                settings.ForceLowerCaseRegex = forceLowerCaseRegex;
            }

            settings.PortalId = PortalId;

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_RegexTestCases")]
        public void AdvancedUrlRewriter_Regex(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            string regexSetting = testFields["Setting"];
            string regexValue = testFields["Value"];
            if (!String.IsNullOrEmpty(regexValue))
            {
                switch (regexSetting)
                {
                    case "IgnoreRegex":
                        settings.IgnoreRegex = regexValue;
                        break;
                    case "DoNotRewriteRegex":
                        settings.DoNotRewriteRegex = regexValue;
                        break;
                    case "UseSiteUrlsRegex":
                        settings.UseSiteUrlsRegex = regexValue;
                        break;
                    case "DoNotRedirectRegex":
                        settings.DoNotRedirectRegex = regexValue;
                        break;
                    case "DoNotRedirectSecureRegex":
                        settings.DoNotRedirectSecureRegex = regexValue;
                        break;
                    case "ForceLowerCaseRegex":
                        settings.ForceLowerCaseRegex = regexValue;
                        break;
                    case "NoFriendlyUrlRegex":
                        settings.NoFriendlyUrlRegex = regexValue;
                        break;
                    case "DoNotIncludeInPathRegex":
                        settings.DoNotIncludeInPathRegex = regexValue;
                        break;
                    case "ValidExtensionlessUrlsRegex":
                        settings.ValidExtensionlessUrlsRegex = regexValue;
                        break;
                    case "RegexMatch":
                        settings.RegexMatch = regexValue;
                        break;
                }
            }

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_ReplaceCharsTestCases")]
        public void AdvancedUrlRewriter_ReplaceChars(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            string testPageName = testFields.GetValue("TestPageName");
            TabInfo tab = null;
            if (!String.IsNullOrEmpty(testPageName))
            {
                var tabName = testFields["Page Name"];
                var tc = new TabController();
                tab = tc.GetTabByName(tabName, PortalId);
                tab.TabName = testPageName;
                tc.UpdateTab(tab);

                //Refetch tab from DB
                tab = tc.GetTab(tab.TabID, tab.PortalID, false);
            }

            settings.PortalId = PortalId;

            string autoAscii = testFields.GetValue("AutoAscii");

            if (!String.IsNullOrEmpty(autoAscii))
            {
                settings.AutoAsciiConvert = Convert.ToBoolean(autoAscii);
            }

            TestUtil.GetReplaceCharDictionary(testFields, settings.ReplaceCharacterDictionary);

            SetDefaultAlias(testFields);

            ExecuteTestForTab(tab, settings, testFields);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_ReplaceSpaceTestCases")]
        public void AdvancedUrlRewriter_ReplaceSpace(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            string replaceSpaceWith = testFields.GetValue("ReplaceSpaceWith");
            if (!String.IsNullOrEmpty(replaceSpaceWith))
            {
                settings.ReplaceSpaceWith = replaceSpaceWith;
            }

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_SiteRootRedirectTestCases")]
        public void AdvancedUrlRewriter_SiteRootRedirect(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", "SiteRootRedirect");

            string scheme = testFields["Scheme"];
            settings.PortalId = PortalId;

            ExecuteTest(settings, testFields, true);
            if (testFields["TestName"].Contains("Resubmit"))
            {
                var httpAlias = testFields["Alias"];
                settings.DoNotRedirectRegex = scheme + httpAlias;

                ExecuteTest(settings, testFields, true);
            }
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_PrimaryPortalAliasTestCases")]
        public void AdvancedUrlRewriter_PrimaryPortalAlias(Dictionary<string, string> testFields)
        {
            string defaultAlias = testFields["DefaultAlias"];

            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);

            settings.PortalId = PortalId;

            string language = testFields["Language"].Trim();
            string skin = testFields["Skin"].Trim();
            if (!String.IsNullOrEmpty(language))
            {
                _customLocale = new Locale { Code = language, Fallback = "en-US" };
                _customLocale.Text = CultureInfo.CreateSpecificCulture(_customLocale.Code).NativeName;
                Localization.SaveLanguage(_customLocale);
                Localization.AddLanguageToPortals(_customLocale.LanguageId);

            }

            if (testFields.ContainsKey("Final Url"))
            {
                testFields["Final Url"] = testFields["Final Url"].Replace("{useAlias}", defaultAlias);
            }

            if (!(String.IsNullOrEmpty(language) && String.IsNullOrEmpty(skin)))
            {
                //add new primary alias
                _primaryAlias = new PortalAliasInfo
                                    {
                                        PortalID = PortalId,
                                        HTTPAlias = defaultAlias,
                                        CultureCode = language,
                                        Skin = skin,
                                        IsPrimary = true
                                    };
                _primaryAlias.PortalAliasID = TestablePortalAliasController.Instance.AddPortalAlias(_primaryAlias);
                ExecuteTest(settings, testFields, true);
            }
            else
            {
                SetDefaultAlias(defaultAlias);
                ExecuteTest(settings, testFields, false);
            }

        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_VanityUrlTestCases")]
        public void AdvancedUrlRewriter_VanityUrl(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);
            settings.DeletedTabHandlingType = DeletedTabHandlingType.Do301RedirectToPortalHome;

            var vanityUrl = testFields.GetValue("VanityUrl", String.Empty);
            var userName = testFields.GetValue("UserName", String.Empty);
            var redirectOld = testFields.GetValue("RedirectOldProfileUrl", String.Empty);

            if (!String.IsNullOrEmpty(userName))
            {
                var user = UserController.GetUserByName(PortalId, userName);
                if (user != null)
                {
                    user.VanityUrl = vanityUrl;
                    UserController.UpdateUser(PortalId, user);
                }
            }

            if (!String.IsNullOrEmpty(redirectOld))
            {
                settings.RedirectOldProfileUrl = Convert.ToBoolean(redirectOld);
            }
            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_SecureRedirectTestCases")]
        public void AdvancedUrlRewriter_SecureRedirect(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", testFields["TestName"]);
            var isClient = Convert.ToBoolean(testFields["Client"]);

            _securePageName = testFields["Page Name"].Trim();

            PortalController.UpdatePortalSetting(PortalId, "SSLEnforced", testFields["Enforced"].Trim());
            PortalController.UpdatePortalSetting(PortalId, "SSLEnabled", testFields["Enabled"].Trim());

            var isSecure = Convert.ToBoolean(testFields["IsSecure"].Trim());

            if (isSecure)
            {
                var tc = new TabController();
                var tab = tc.GetTabByName(_securePageName, PortalId);
                tab.IsSecure = true;

                UpdateTab(tab);
            }

            settings.SSLClientRedirect = isClient;

            ExecuteTest(settings, testFields, true);
        }

        [Test]
        [TestCaseSource(typeof(UrlTestFactoryClass), "UrlRewrite_JiraTests")]
        public void AdvancedUrlRewriter_JiraTests(Dictionary<string, string> testFields)
        {
            var settings = UrlTestFactoryClass.GetSettings("UrlRewrite", "Jira_Tests", testFields["SettingsFile"]);
            var dictionary = UrlTestFactoryClass.GetDictionary("UrlRewrite", "Jira_Tests", testFields["DictionaryFile"]);

            int homeTabId = -1;
            bool homeTabChanged = false;
            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Key == "HomeTabId")
                {
                    homeTabId = UpdateHomeTab(Int32.Parse(keyValuePair.Value));
                    homeTabChanged = true;
                }
            }

            ExecuteTest(settings, testFields, true);

            if (homeTabChanged)
            {
                UpdateHomeTab(homeTabId);
            }
        }

        private int UpdateHomeTab(int homeTabId)
        {
            var portalController = new PortalController();
            var portalInfo = portalController.GetPortal(PortalId);
            int oldHomeTabId = portalInfo.HomeTabId;
            portalInfo.HomeTabId = homeTabId;

            return oldHomeTabId;
        }

        #endregion
    }
}
