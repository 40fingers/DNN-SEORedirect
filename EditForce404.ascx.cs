using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;
using FortyFingers.SeoRedirect.Components;
using Globals = DotNetNuke.Common.Globals;

namespace FortyFingers.SeoRedirect
{
    public partial class EditForce404 : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterResources(Page);

            if (!IsPostBack)
            {
                lnkBack.NavigateUrl = Globals.NavigateURL(TabId);
            }
        }

        private void RegisterResources(Page page)
        {
            jQuery.RequestRegistration();

            ClientResourceManager.RegisterStyleSheet(page, "~/Desktopmodules/40Fingers/SeoRedirect/js/jstree/themes/default/style.css", FileOrder.Css.ModuleCss);
            ClientResourceManager.RegisterScript(page, "~/Desktopmodules/40Fingers/SeoRedirect/js/jstree/jstree.min.js", FileOrder.Js.DefaultPriority);

            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            ServicesFramework.Instance.RequestAjaxScriptSupport();
            //ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/40F-Common.js", FileOrder.Js.jQuery);
            //ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/SeoRedirect.js", FileOrder.Js.jQuery);

            //Page.ClientScript.RegisterClientScriptInclude("jQuery.DataTables.1.8.2", ResolveUrl("js/DataTables-1.8.2/media/js/jquery.dataTables.min.js"));
            //Common.AddCssLink(ResolveUrl("js/DataTables-1.8.2/media/demo_table.css"), Page);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "40Fingers.SeoRedirect.DataTablesStartup", "jQuery(document).ready(function() { jQuery('#SeoRedirectMappingsTable').dataTable(); } );", true);
        }
        protected string TreeData
        {
            get
            {
                // get the tabs
                var allTabs = new TabController().GetTabsByPortal(PortalId);
                // var allTabs = TabController.GetPortalTabs(PortalSettings.Current.PortalId, Null.NullInteger, false, true, false, false);
                // get the locales
                var locales = allTabs.Values.Select(t => t.CultureCode).Distinct();
                // create root nodes for locales on root level
                var nodes = new List<JsTreeNode>();
                foreach (var locale in locales.OrderBy(s => s))
                {
                    nodes.Add(new JsTreeNode() {
                        id = string.IsNullOrEmpty(locale) ? "neutral" : locale, 
                        text = string.IsNullOrEmpty(locale) ? LocalizeString("NeutralLanguage.Text") : locale, 
                        parent = "#",
                        state = new JsTreeNodeState()
                        {
                            opened = false,
                            disabled = true
                        }});
                }

                // now add all the pages
                foreach (var tab in allTabs.Values)
                {
                    nodes.Add(new JsTreeNode()
                    {
                        id = tab.TabID.ToString(),
                        parent = tab.ParentId <= 0 ? String.IsNullOrEmpty(tab.CultureCode) ? "neutral" : tab.CultureCode : tab.ParentId.ToString(),
                        text = tab.TabName,
                        state = new JsTreeNodeState()
                        {
                            opened = false,
                            selected = tab.IsForce404(),
                            disabled = false
                        }
                    });
                }

                var retval = Json.Serialize(nodes);
                return retval;
            }

        }

        protected void lnkSave_OnClick(object sender, EventArgs e)
        {
            var sSelectedTabs = selectedField.Value;
            // when empty: no changes
            if (!String.IsNullOrEmpty(sSelectedTabs))
            {
                var selectedTabIds = new List<int>();
                foreach (var sTabId in sSelectedTabs.Split(','))
                {
                    int i = 0;
                    if(int.TryParse(sTabId, out i)) selectedTabIds.Add(i);
                }
                var allTabs = new TabController().GetTabsByPortal(PortalId);
                foreach (var tabInfo in allTabs.Values)
                {
                    if (tabInfo.IsForce404() != selectedTabIds.Contains(tabInfo.TabID))
                    {
                        // changed:
                        tabInfo.SetForce404(selectedTabIds.Contains(tabInfo.TabID));
                    }
                }
            }

            Response.Redirect(Globals.NavigateURL(TabId));
        }
    }
    /// <summary>
    /// https://www.jstree.com/docs/json/
    /// Alternative format of the node (id & parent are required)
    /// </summary>
    public class JsTreeNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public JsTreeNodeState state { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }
    }

    public class JsTreeNodeState
    {
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}