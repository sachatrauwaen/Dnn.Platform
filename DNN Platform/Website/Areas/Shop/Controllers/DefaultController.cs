// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Website.Areas.Shop.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Web.MvcPipeline.Framework;
    using DotNetNuke.Website.Areas.Shop.Models;

    public class DefaultController : DotNetNuke.Web.MvcWebsite.Controllers.DefaultController
    {
        public DefaultController(
            DotNetNuke.ContentSecurityPolicy.IContentSecurityPolicy contentSecurityPolicy,
            INavigationManager navigationManager,
            IPageModelFactory pageModelFactory)
            : base(contentSecurityPolicy, navigationManager, pageModelFactory)
        {
        }

        // GET: Shop/Default
        public ActionResult Index()
        {
            var tabid = this.PortalSettings.ActiveTab.TabID;
            var language = this.PortalSettings.CultureCode;
            this.BeforeCreateModel();
            var model = this.pageModelFactory.CreatePageModel<ShopPageModel>(this);
            model.Text = "Index";

            return this.Page(tabid, language, "Index", model);
        }

        // GET: Shop/Default/Details/5
        public ActionResult Details(int id)
        {
            return this.View();
        }

        // GET: Shop/Default/Create
        public ActionResult Create()
        {
            var tabid = this.PortalSettings.ActiveTab.TabID;
            var language = this.PortalSettings.CultureCode;
            this.BeforeCreateModel();
            var model = this.pageModelFactory.CreatePageModel<ShopPageModel>(this);
            model.Text = "Create";

            return this.Page(tabid, language, "Create", model);
        }

        // POST: Shop/Default/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }

        // GET: Shop/Default/Edit/5
        public ActionResult Edit(int id)
        {
            return this.View();
        }

        // POST: Shop/Default/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }

        // GET: Shop/Default/Delete/5
        public ActionResult Delete(int id)
        {
            return this.View();
        }

        // POST: Shop/Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }
    }
}
