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

            // als tabInfo.IsForce404() en de url is "dieper" dan de tab zelf: dan een 404 geven
            var activeTab = Common.CurrentPortalSettings.ActiveTab;
            if (activeTab.IsForce404())
            {
                Common.Logger.Debug($"Force404-check for tab {Common.CurrentPortalSettings.ActiveTab.TabID}");

                var incoming = Common.IncomingUrl;
                var tabUrl = new Uri(activeTab.FullUrl.ToLowerInvariant());
                var incUrl = new Uri(incoming);

                if (incUrl.LocalPath.StartsWith(tabUrl.LocalPath) && incUrl.LocalPath.Length > tabUrl.LocalPath.Length)
                {
                    Common.Handle404Exception(HttpContext.Current.Response, Common.CurrentPortalSettings);
                }
                // also check TabUrls with httpstatus=200
                foreach (var tabUrlInfo in activeTab.TabUrls.Where(tu => tu.HttpStatus == ((int)HttpStatusCode.OK).ToString()))
                {
                    if (incUrl.LocalPath.StartsWith(tabUrlInfo.Url.ToLowerInvariant()) && incUrl.LocalPath.Length > tabUrlInfo.Url.Length)
                    {
                        Common.Handle404Exception(HttpContext.Current.Response, Common.CurrentPortalSettings);
                    }
                }
            }
        }
    }
}