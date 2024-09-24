﻿using System;
using System.ComponentModel.DataAnnotations;
using DotNetNuke.Services.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.ModuleCache;
using DotNetNuke.UI.WebControls;
using System.ComponentModel.DataAnnotations;
using DotNetNuke.Services.Localization;
using System.Collections.Generic;
using System.Web.Mvc;

public class ModuleSettingsMvcViewModel
{
    public int ModuleId { get; set; }
    [Required(ErrorMessage = "ModuleTitle is required.")]
    [Display(Name = "ModuleTitle", ResourceType = typeof(LocalResourceFile))]
    public string ModuleTitle { get; set; }
    [Display(Name = "Alignment", ResourceType = typeof(LocalResourceFile))]
    public string Alignment { get; set; }
    public bool AllTabs { get; set; }
    public bool NewTabs { get; set; }
    public bool AllowIndex { get; set; }
    public bool IsShareable { get; set; }
    public bool IsShareableViewOnly { get; set; }
    public bool AdminBorder { get; set; }
    public string Header { get; set; }
    public string Footer { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Moniker { get; set; }
    public string CacheProvider { get; set; }
    public int CacheDuration { get; set; }
    public string Color { get; set; }
    public string Border { get; set; }
    public string IconFile { get; set; }
    public VisibilityState Visibility { get; set; }
    public bool DisplayTitle { get; set; }
    public bool DisplayPrint { get; set; }
    public bool DisplaySyndicate { get; set; }
    public bool IsWebSlice { get; set; }
    public string WebSliceTitle { get; set; }
    public DateTime? WebSliceExpiryDate { get; set; }
    public int? WebSliceTTL { get; set; }
    public string ContainerSrc { get; set; }
    public bool IsDefaultModule { get; set; }
    public bool AllModules { get; set; }
    public int TabId { get; set; }
    public List<TabInfo> AvailableTabs { get; set; }
    public List<ModulePermissionInfo> Permissions { get; set; }
    public List<Term> Terms { get; set; }
    public bool InheritViewPermissions { get; set; }
    public bool AllTabsChanged { get; set; }
    public List<SelectListItem> AvailableCacheProviders { get; set; }

    public ModuleSettingsMvcViewModel()
    {
        AvailableTabs = new List<TabInfo>();
        Permissions = new List<ModulePermissionInfo>();
        Terms = new List<Term>();
        AvailableCacheProviders = new List<SelectListItem>();
    }

    public void LoadSettings(ModuleInfo module)
    {
        ModuleId = module.ModuleID;
        ModuleTitle = module.ModuleTitle;
        Alignment = module.Alignment;
        AllTabs = module.AllTabs;
        Color = module.Color;
        Border = module.Border;
        IconFile = module.IconFile;
        CacheDuration = module.CacheTime;
        CacheProvider = module.CacheMethod;
        TabId = module.TabID;
        Visibility = module.Visibility;
        Header = module.Header;
        Footer = module.Footer;
        StartDate = module.StartDate;
        EndDate = module.EndDate;
        ContainerSrc = module.ContainerSrc;
        DisplayTitle = module.DisplayTitle;
        DisplayPrint = module.DisplayPrint;
        DisplaySyndicate = module.DisplaySyndicate;
        IsWebSlice = module.IsWebSlice;
        WebSliceTitle = module.WebSliceTitle;
        WebSliceExpiryDate = module.WebSliceExpiryDate;
        WebSliceTTL = module.WebSliceTTL;
        IsDefaultModule = module.IsDefaultModule;
        AllModules = module.AllModules;

        AllowIndex = bool.Parse(module.ModuleSettings["AllowIndex"] ?? "true");
        AdminBorder = bool.Parse(module.ModuleSettings["hideadminborder"] ?? "false");
        Moniker = module.ModuleSettings["Moniker"] as string ?? string.Empty;

        if (!module.IsShared)
        {
            InheritViewPermissions = module.InheritViewPermissions;
            IsShareable = module.IsShareable;
            IsShareableViewOnly = module.IsShareableViewOnly;
        }

        Permissions = new List<ModulePermissionInfo>(module.ModulePermissions);
        Terms = new List<Term>(module.Terms);

        // Laad beschikbare cache providers
        var cacheProviders = ModuleCachingProvider.GetProviderList();
        AvailableCacheProviders = cacheProviders.Select(p => new SelectListItem
        {
            Text = p.Key.Replace("ModuleCachingProvider", ""),
            Value = p.Key
        }).ToList();
        AvailableCacheProviders.Insert(0, new SelectListItem { Text = "None Specified", Value = "" });
    }
}
