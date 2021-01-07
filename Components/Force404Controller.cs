using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace FortyFingers.SeoRedirect.Components
{
    public static class Force404Controller
    {
        public static void DoForce404Check()
        {
            // no forced 404 for authenticated users
            if (HttpContext.Current.Request.IsAuthenticated) return;
            // als tabInfo.IsForce404() en de url is "dieper" dan de tab zelf: dan een 404 geven
            var activeTab = Common.CurrentPortalSettings.ActiveTab;
            if (activeTab.IsForce404() && !IsDnnControlUrl())
            {
                Common.Logger.Debug($"Force404-check for tab {Common.CurrentPortalSettings.ActiveTab.TabID}");

                var tabFullUrl = "";
                // activeTab can be a tab when handlers are being called.
                // in those cases, FullUrl throws an exception
                // we're "handling" that here
                try
                {
                    tabFullUrl = activeTab.FullUrl.ToLowerInvariant();
                }
                catch (Exception e)
                {
                }

                if (string.IsNullOrEmpty(tabFullUrl)) return;

                var incoming = Common.IncomingUrl;
                var tabUrl = new Uri(tabFullUrl);
                var incUrl = new Uri(incoming);
                
                // requests for dependancyhandler.axd don't get recognised as a handler and need to be explicitly ignored
                if (incoming.Contains("dependencyhandler.axd"))
                {
                    return;
                }

                if (incUrl.LocalPath.StartsWith(tabUrl.LocalPath) && incUrl.LocalPath.Length > tabUrl.LocalPath.Length)
                {
                    RedirectController.AddRedirectLog(Common.CurrentPortalSettings.PortalId, incoming, "");
                    Common.Handle404Exception(HttpContext.Current.Response, Common.CurrentPortalSettings);
                }
                // also check TabUrls with httpstatus=200
                foreach (var tabUrlInfo in activeTab.TabUrls.Where(tu => tu.HttpStatus == ((int)HttpStatusCode.OK).ToString()))
                {
                    if (incUrl.LocalPath.StartsWith(tabUrlInfo.Url.ToLowerInvariant()) && incUrl.LocalPath.Length > tabUrlInfo.Url.Length)
                    {
                        RedirectController.AddRedirectLog(Common.CurrentPortalSettings.PortalId, incoming, "");
                        Common.Handle404Exception(HttpContext.Current.Response, Common.CurrentPortalSettings);
                    }
                }
            }
        }

        private static bool IsDnnControlUrl()
        {
            var ps = Common.CurrentPortalSettings;
            var req = HttpContext.Current.Request;

            if (ps.LoginTabId <= 0 && req.QueryString["ctl"]?.ToLower() == "login")
            {
                return true;
            }
            if (ps.RegisterTabId <= 0 && req.QueryString["ctl"]?.ToLower() == "register")
            {
                return true;
            }

            return false;
        }
    }
}