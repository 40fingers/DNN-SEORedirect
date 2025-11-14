using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace FortyFingers.SeoRedirect.Components
{
    public class SeoRedirectModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            if (IsUpgrade) return;

            try
            {
                var rsp = HttpContext.Current.Response;
                var ps = Common.CurrentPortalSettings;

                // this needs to be done on EndRequest, to be sure PortalSettings.ActiveTab is correct
                if (rsp.StatusCode == (int) HttpStatusCode.OK)
                {
                    Force404Controller.DoForce404Check();
                }

                string incoming = Common.IncomingUrl;
                if (ps.ActiveTab?.TabID != ps.ErrorPage404 && rsp.StatusCode == (int) HttpStatusCode.NotFound && !string.IsNullOrEmpty(incoming))
                {
                    Common.Logger.Debug($"Logging redirect from Context_EndRequest. incoming:[{incoming}]");
                    RedirectController.AddRedirectLog(ps.PortalId, incoming, "");
                }
            }
            catch (Exception exception)
            {
                // we're not writing in the eventlog, since the amount of log records can really explode
                // we MUST catch every possible exception here, otherwise the website would be completely down in case of a bug
            }
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            if (IsUpgrade) return;
            if (!Common.AreDIServicesReady) return;

            // find incoming URL
            string incoming = "";

            try
            {
                // in case of een upgrade, PortalSettings will be null, nothing to do in such a case:
                // if (PortalSettings.Current == null) return;

                var fake = new ControlCollection(new LiteralControl(""));
                Common.Logger.Debug($"Calling DoRedirect From HttpModule");
                RedirectController.DoRedirect(fake, redirectWhenNo404Detected: true, onlyLogWhen404: true);
            }
            catch (Exception exception)
            {
                // we're not writing in the eventlog, since the amount of log records can really explode
                // we MUST catch every possible exception here, otherwise the website would be completely down in case of a bug
            }


        }

        public void Dispose()
        {

        }

        public bool IsUpgrade
        {
            get
            {
                if (!HttpContext.Current.Items.Contains("40f_seo_isupgrade"))
                {
                    var assemblyVersion = DotNetNukeContext.Current.Application.Version;
                    var databaseVersion = DotNetNuke.Data.DataProvider.Instance().GetVersion();

                    HttpContext.Current.Items["40f_seo_isupgrade"] = !(assemblyVersion.Major == databaseVersion.Major 
                                                                       && assemblyVersion.Minor == databaseVersion.Minor 
                                                                       && assemblyVersion.Build == databaseVersion.Build);
                }

                return (bool)HttpContext.Current.Items["40f_seo_isupgrade"];
            }
        }
    }
}