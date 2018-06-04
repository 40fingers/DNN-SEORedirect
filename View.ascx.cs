using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;
using FortyFingers.SeoRedirect.Components;
using FortyFingers.SeoRedirect.Components.Data;
using Globals = DotNetNuke.Common.Globals;
using cfg = FortyFingers.SeoRedirect.Components.Config;

namespace FortyFingers.SeoRedirect
{
    public partial class View : PortalModuleBase, IActionable
    {
        private cfg _config;
        protected cfg Config
        {
            get
            {
                if (_config == null)
                    _config = new cfg(Settings, ModuleId, TabModuleId);

                return _config;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.PreRender += Page_PreRender;
            // as of dnn 7.1, when using Advanced FriendlyURLs, there needs to be a setting in Portalsettings for 404 pages
            // here we're checking that

            var ver = DotNetNuke.Application.DotNetNukeContext.Current.Application.Version;

            if ((UserInfo.IsSuperUser || UserInfo.IsInRole(PortalSettings.AdministratorRoleName)) && (ver.Major == 7 && ver.Minor < 3))
            {
                if(PortalController.GetPortalSettingAsInteger("AUM_ErrorPage404", PortalId, Null.NullInteger) <= 0)
                    Controls.Add(
                        new LiteralControl(String.Format("<p class=\"NormalRed\">{0}</p>",
                            Localization.GetString("Dnn404PortalSettingAbsent.Text", LocalResourceFile))));
            }

            // if it's not a 404, we only want to log when the requested URL is for a deeper level than the page itself is
            //// TODO: This doesn't work as intended, so it's gone
            //// so OnlyLogwhen404 should be true if the request is for the same or higher level
            ////Request.RawUrl.Count(t => t == '/') <= PortalSettings.ActiveTab.Level + 1;
            var onlyLogWhen404 = true;

            Common.Logger.Debug($"Calling DoRedirect From View.ascx OnInit");
            RedirectController.DoRedirect(LoggingPlaceholder.Controls, true, onlyLogWhen404);

            if (!IsPostBack)
            {
                if (TabPermissionController.CanAdminPage())
                {
                    UnhandledUrlsPanel.Visible = true;
                }
            }
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            string incoming = (string)HttpContext.Current.Items["40F_SEO_IncomingUrl"];

            // check if IIS/ASP.NET/DNN already found this to be a 404
            if (Response.Status == "404 Not Found")
            {
                if (Response.StatusCode == (int)HttpStatusCode.NotFound && !string.IsNullOrEmpty(incoming))
                {
                    Common.Logger.Debug($"Logging redirect from Context_EndRequest. incoming:[{incoming}]");
                    RedirectController.AddRedirectLog(PortalId, incoming, "");
                }
            }

            // DNN returns 200 for static files that are actually 404's
            if (RedirectController.RequestHas404Detected() && Response.StatusCode == (int) HttpStatusCode.OK)
            {
                //RedirectController.AddRedirectLog(PortalId, incoming, "");
                RedirectController.SetStatus404();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientResourceManager.RegisterScript(Page, "resources/shared/scripts/knockout.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "resources/shared/scripts/knockout.mapping.js", FileOrder.Js.jQuery + 1);
            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            ServicesFramework.Instance.RequestAjaxScriptSupport();
            ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/40F-Common.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "desktopmodules/40fingers/seoredirect/js/SeoRedirect.js", FileOrder.Js.jQuery);

            UnhandledUrlsPanelHeader.Text = String.Format(Localization.GetString("UnhandledUrlsPanelHeader.Text", LocalResourceFile), Config.NoOfEntries);

        }


        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                actions.Add(GetNextActionID(),
                            Localization.GetString("EditModule.Action", LocalResourceFile),
                            ModuleActionType.EditContent,
                            "",
                            "",
                            EditUrl(),
                            false, DotNetNuke.Security.SecurityAccessLevel.Edit,
                            true,
                            false);

                return actions;
            }
        }
    }
}