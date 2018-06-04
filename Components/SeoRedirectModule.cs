using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

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
            try
            {
                var rsp = HttpContext.Current.Response;
                var ps = Common.CurrentPortalSettings;
                string incoming = (string) HttpContext.Current.Items["40F_SEO_IncomingUrl"];
                if (rsp.StatusCode == (int) HttpStatusCode.NotFound && !string.IsNullOrEmpty(incoming))
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
    }
}