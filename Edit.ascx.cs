using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;
using FortyFingers.SeoRedirect.Components;
using Globals = DotNetNuke.Common.Globals;

namespace FortyFingers.SeoRedirect
{
    public partial class Edit : PortalModuleBase
    {
        private Config _config;
        protected Config Config
        {
            get
            {
                if (_config == null)
                    _config = new Config(Settings, ModuleId, TabModuleId);

                return _config;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterResources(Page);

            if (!IsPostBack)
            {
                lnkBack.NavigateUrl = ServiceHelper.I.NavigationManager.NavigateURL(TabId);
                lnkBackTop.NavigateUrl = lnkBack.NavigateUrl;
            }
        }

        private void RegisterResources(Page page)
        {
            ClientResourceManager.RegisterStyleSheet(page, "~/Resources/Shared/components/DropDownList/dnn.DropDownList.css", FileOrder.Css.ModuleCss);
            ClientResourceManager.RegisterStyleSheet(page, "~/Resources/Shared/scripts/jquery/dnn.jScrollBar.css", FileOrder.Css.ModuleCss);
            
            if (Config.RegisterJquery)
            {
                ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/jquery/jquery.min.js");
            }
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/dnn.extensions.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/dnn.DataStructures.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/jquery/jquery.mousewheel.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/jquery/dnn.jScrollBar.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/TreeView/dnn.TreeView.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/scripts/TreeView/dnn.DynamicTreeView.js");
            ClientResourceManager.RegisterScript(page, "~/Resources/Shared/Components/DropDownList/dnn.DropDownList.js");

            ClientResourceManager.RegisterScript(Page, "resources/shared/scripts/knockout.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "resources/shared/scripts/knockout.mapping.js", FileOrder.Js.jQuery + 1);
            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            ServicesFramework.Instance.RequestAjaxScriptSupport();
            ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/40F-Common.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/SeoRedirect.js", FileOrder.Js.jQuery);

            //Page.ClientScript.RegisterClientScriptInclude("jQuery.DataTables.1.8.2", ResolveUrl("js/DataTables-1.8.2/media/js/jquery.dataTables.min.js"));
            //Common.AddCssLink(ResolveUrl("js/DataTables-1.8.2/media/demo_table.css"), Page);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "40Fingers.SeoRedirect.DataTablesStartup", "jQuery(document).ready(function() { jQuery('#SeoRedirectMappingsTable').dataTable(); } );", true);
        }
        internal static void RegisterClientScript(Page page, string skin)
        {

        }

        //protected void MappingsRepeater_ItemDatabound(object sender, RepeaterItemEventArgs e)
        //{
        //    switch (e.Item.ItemType)
        //    {
        //        case ListItemType.Header:
        //            BindMappingsRepeaterHeader(e);
        //            break;
        //        case ListItemType.Footer:
        //            BindMappingsRepeaterFooter(e);
        //            break;
        //        case ListItemType.Item:
        //        case ListItemType.AlternatingItem:
        //            BindMappingsRepeaterItem(e);
        //            break;
        //    }
        //}

        //private void BindMappingsRepeaterFooter(RepeaterItemEventArgs e)
        //{
        //}

        //private void BindMappingsRepeaterHeader(RepeaterItemEventArgs e)
        //{
        //    var btn = (ImageButton) e.Item.FindControl("AddButton");
        //    btn.ImageUrl = IconTypes.Add.GetUrl();
        //}

        //private List<TabInfo> _PortalTabs = null;
        //private List<TabInfo> PortalTabs
        //{
        //    get
        //    {
        //        if(_PortalTabs == null)
        //        {
        //            _PortalTabs = TabController.GetPortalTabs(PortalId, -1, false, true, true, true);
        //        }

        //        return _PortalTabs;
        //    }
        //}

        //private void BindMappingsRepeaterItem(RepeaterItemEventArgs e)
        //{
        //    var btn = (ImageButton)e.Item.FindControl("DeleteButton");
        //    btn.CommandName = "DeleteRow";
        //    btn.ImageUrl = IconTypes.Delete.GetUrl();

        //    var map = (Mapping) e.Item.DataItem;

        //    var txtSource = (TextBox) e.Item.FindControl("SourceUrlTextBox");
        //    var txtTarget = (TextBox) e.Item.FindControl("TargetUrlTextBox");
        //    var cboTarget = (DnnPageDropDownList) e.Item.FindControl("TargetTabId");
        //    cboTarget.IncludeDisabledTabs = false;
        //    cboTarget.OnClientSelectionChanged.Add("ff_seo_selectedPageChanged");

        //    var radUrl = (RadioButton) e.Item.FindControl("UseUrlRadio");
        //    var radTab = (RadioButton) e.Item.FindControl("UseTabRadio");

        //    radUrl.GroupName = txtSource.ClientID;
        //    radTab.GroupName = txtSource.ClientID;

        //    if (map.TargetTabId > 0)
        //    {
        //        txtTarget.Text = "";

        //        radUrl.Checked = false;
        //        radTab.Checked = true;
        //        cboTarget.SelectedPage = PortalTabs.FirstOrDefault(t => t.TabID == map.TargetTabId);
        //        cboTarget.PortalId = PortalId;
        //    }
        //    else
        //    {
        //        radUrl.Checked = true;
        //        radTab.Checked = false;
        //        txtTarget.Text = map.TargetUrl;
        //        cboTarget.SelectedPage = null;
        //    }
        //}

        //protected void MappingsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    switch (e.CommandName)
        //    {
        //        case "StartEdit":
        //            break;
        //        case "CancelEdit":
        //            break;
        //        case "EndEdit":
        //            break;
        //        case "NewRow":
        //            break;
        //        case "DeleteRow":
        //            // read from controls
        //            var config = ReadConfigData();
        //            // remove the one
        //            config.Mappings.RemoveAt(e.Item.ItemIndex);
        //            // rebind
        //            FillForm(config);
        //            break;
        //    }
        //}

        //protected void SaveMappingsButton_Click(object sender, EventArgs e)
        //{
        //    // read from controls
        //    var config = ReadConfigData();
        //    // serialize to file
        //    config.ToFile(Common.RedirectConfigFile());
        //    RedirectConfig.Reload(PortalId);
        //    FillForm(RedirectConfig.Instance);
        //}

        //protected void AddButton_Click(object sender, ImageClickEventArgs e)
        //{
        //    // read from controls
        //    var config = ReadConfigData();
        //    // add 1 object
        //    config.Mappings.Insert(0, new Mapping());
        //    // rebind to repeater
        //    FillForm(config);
        //}

        //private RedirectConfig ReadConfigData()
        //{
        //    var retval = new RedirectConfig();

        //    foreach (RepeaterItem repeaterItem in MappingsRepeater.Items)
        //    {
        //        var source = ((TextBox)repeaterItem.FindControl("SourceUrlTextBox")).Text;
        //        var txtTarget = ((TextBox)repeaterItem.FindControl("TargetUrlTextBox")).Text;
        //        var tabTarget = ((DnnPageDropDownList)repeaterItem.FindControl("TargetTabId")).SelectedPage;
        //        var useRegex = ((CheckBox) repeaterItem.FindControl("UseRegexCheckBox")).Checked;

        //        var radUrl = (RadioButton)repeaterItem.FindControl("UseUrlRadio");
        //        var radTab = (RadioButton)repeaterItem.FindControl("UseTabRadio");

        //        string targetUrl = "";
        //        int targetTabId = 0;
        //        if (radTab.Checked)
        //        {
        //            targetTabId = tabTarget.TabID;
        //            targetUrl = Globals.NavigateURL(targetTabId, PortalSettings.Current, "");
        //        }
        //        else if(radUrl.Checked)
        //        {
        //            targetUrl = txtTarget;
        //            targetTabId = -1;
        //        }
        //        retval.Mappings.Add(new Mapping()
        //                                {
        //                                    SourceUrl = source,
        //                                    TargetUrl =  targetUrl,
        //                                    TargetTabId = targetTabId,
        //                                    UseRegex = useRegex
        //                                });
        //    }

        //    return retval;
        //}

        //private void FillForm(RedirectConfig redirectConfig)
        //{
        //    if(redirectConfig.Mappings.Count == 0)
        //        redirectConfig.Mappings.Add(new Mapping());

        //    MappingsRepeater.DataSource = redirectConfig.Mappings;
        //    MappingsRepeater.DataBind();
        //}

    }
}