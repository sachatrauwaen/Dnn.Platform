using System;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;

public class ModuleSettingsMvcController : DnnController
{
    private readonly INavigationManager _navigationManager;

    public ModuleSettingsMvcController()
    {
        _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
    }

    [ModuleAction(ControlKey = "Edit", TitleKey = "EditModule")]
    public ActionResult Index(int moduleId)
    {
        if (!ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", Module))
        {
            return AccessDeniedView();
        }

        var module = ModuleController.Instance.GetModule(moduleId, TabId, false);
        var viewModel = new ModuleSettingsMvcViewModel();
        viewModel.LoadSettings(module);
        viewModel.AvailableTabs = TabController.GetPortalTabs(PortalId, -1, false, null, true, false, true, false, true);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateModel]
    public ActionResult Update(ModuleSettingsMvcViewModel model)
    {
        if (!ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", Module))
        {
            return AccessDeniedView();
        }

        if (ModelState.IsValid)
        {
            var module = ModuleController.Instance.GetModule(model.ModuleId, TabId, false);

            module.ModuleTitle = model.ModuleTitle;
            module.Alignment = model.Alignment;
            module.AllTabs = model.AllTabs;
            module.Color = model.Color;
            module.Border = model.Border;
            module.IconFile = model.IconFile;
            module.CacheTime = model.CacheDuration;
            module.CacheMethod = model.CacheProvider;
            module.TabID = model.TabId;
            module.Visibility = model.Visibility;
            module.IsDeleted = false;
            module.Header = model.Header;
            module.Footer = model.Footer;
            module.StartDate = model.StartDate ?? Null.NullDate;
            module.EndDate = model.EndDate ?? Null.NullDate;
            module.ContainerSrc = model.ContainerSrc;
            module.DisplayTitle = model.DisplayTitle;
            module.DisplayPrint = model.DisplayPrint;
            module.DisplaySyndicate = model.DisplaySyndicate;
            module.IsWebSlice = model.IsWebSlice;
            module.WebSliceTitle = model.WebSliceTitle;
            module.WebSliceExpiryDate = model.WebSliceExpiryDate ?? Null.NullDate;
            module.WebSliceTTL = model.WebSliceTTL ?? Null.NullInteger;
            module.IsDefaultModule = model.IsDefaultModule;
            module.AllModules = model.AllModules;

            ModuleController.Instance.UpdateModule(module);

            ModuleController.Instance.UpdateModuleSetting(module.ModuleID, "AllowIndex", model.AllowIndex.ToString());
            ModuleController.Instance.UpdateModuleSetting(module.ModuleID, "hideadminborder", model.AdminBorder.ToString());
            ModuleController.Instance.UpdateModuleSetting(module.ModuleID, "Moniker", model.Moniker);

            if (!module.IsShared)
            {
                module.InheritViewPermissions = model.InheritViewPermissions;
                module.IsShareable = model.IsShareable;
                module.IsShareableViewOnly = model.IsShareableViewOnly;
            }

            ModulePermissionController.SaveModulePermissions(module);

            ModuleController.Instance.UpdateModuleTerms(module, model.Terms);

            if (!model.AllTabs)
            {
                var newTabId = model.TabId;
                if (module.TabID != newTabId)
                {
                    ModuleController.Instance.MoveModule(module.ModuleID, module.TabID, newTabId, Globals.glbDefaultPane);
                }
            }

            if (model.AllTabsChanged)
            {
                var listTabs = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, false, true);
                if (model.AllTabs)
                {
                    if (!model.NewTabs)
                    {
                        foreach (var destinationTab in listTabs)
                        {
                            var existingModule = ModuleController.Instance.GetModule(module.ModuleID, destinationTab.TabID, false);
                            if (existingModule != null)
                            {
                                if (existingModule.IsDeleted)
                                {
                                    ModuleController.Instance.RestoreModule(existingModule);
                                }
                            }
                            else
                            {
                                if (!PortalSettings.ContentLocalizationEnabled || (module.CultureCode == destinationTab.CultureCode))
                                {
                                    ModuleController.Instance.CopyModule(module, destinationTab, module.PaneName, true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ModuleController.Instance.DeleteAllModules(module.ModuleID, module.TabID, listTabs, true, false, false);
                }
            }

            return RedirectToDefaultRoute();
        }

        // Als we hier komen, was er een validatiefout
        var module = ModuleController.Instance.GetModule(model.ModuleId, TabId, false);
        model.LoadSettings(module); // Dit zal AvailableCacheProviders opnieuw vullen
        model.AvailableTabs = TabController.GetPortalTabs(PortalId, -1, false, null, true, false, true, false, true);
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int moduleId)
    {
        if (!ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", Module))
        {
            return AccessDeniedView();
        }

        ModuleController.Instance.DeleteTabModule(TabId, moduleId, true);
        return RedirectToDefaultRoute();
    }
}