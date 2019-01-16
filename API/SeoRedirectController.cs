using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Api;
using FortyFingers.SeoRedirect.API.Models;
using FortyFingers.SeoRedirect.Components;

namespace FortyFingers.SeoRedirect.API
{

    [SupportedModules("40Fingers.SeoRedirect")] // can be comma separated list of supported module
    public class SeoRedirectController : DnnApiController
    {
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [HttpGet]
        public HttpResponseMessage GetUnhandledUrls()
        {
            var _config = new Components.Config(ActiveModule.ModuleSettings, ActiveModule.ModuleID, ActiveModule.TabModuleID);
            var data = RedirectController.GetTopUnhandledUrls(PortalSettings.PortalId, Constants.UnhandledUrlsMaxDays, _config.NoOfEntries);

            var retval = new UnhandledUrlsModel();
            data.ForEach(u => retval.Urls.Add(new UnhandledUrlModel(u)));

            return Request.CreateResponse(HttpStatusCode.OK, retval);
        }

        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [HttpPost]
        public HttpResponseMessage SaveRedirect(MappingModel model)
        {
            var map = new Mapping();
            map.SourceUrl = model.SourceUrl;
            if (!string.IsNullOrEmpty(model.TargetUrl))
            {
                map.TargetUrl = model.TargetUrl;
            }
            else if (model.TargetTabId > 0)
            {
                map.TargetTabId = model.TargetTabId;
                map.TargetUrl = DotNetNuke.Common.Globals.NavigateURL(map.TargetTabId, PortalSettings, "");
            }
            else
            {
                // keep giving 404
                map = null;
            }
            if (map != null)
            {
                map.UseRegex = false;
                var cfg = RedirectConfig.Instance;
                cfg.Mappings.Add(map);
                cfg.ToFile(Common.RedirectConfigFile());
                RedirectConfig.Reload(PortalSettings.PortalId);
            }

            // set handledon/handledby in table
            RedirectController.SetHandledUrl(model.SourceUrl);

            return Request.CreateResponse(HttpStatusCode.OK, new {});

        }

        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [HttpGet]
        public HttpResponseMessage GetMappings()
        {
            var data = RedirectConfig.Instance.Mappings;



            var retval = new MappingsModel();
            data.ForEach(u => retval.Mappings.Add(new MappingModel(u)));

            return Request.CreateResponse(HttpStatusCode.OK, retval);
        }

        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [HttpPost]
        public HttpResponseMessage SaveMapping(MappingModel model) // string id, bool useRegex, string sourceUrl, string targetUrl, int targetTabId)
        {
            var map = RedirectConfig.Instance.Mappings.FirstOrDefault(m => m.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Id) && map == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            if (map == null && string.IsNullOrEmpty(model.Id))
            {
                map = new Mapping();
                map.Id = Guid.NewGuid().ToString();
            }

            map.SourceUrl = model.SourceUrl;
            map.UseRegex = model.UseRegex;
            if (!string.IsNullOrEmpty(model.TargetUrl))
            {
                map.TargetTabId = Null.NullInteger;
                map.TargetUrl = model.TargetUrl;
            }
            else if (model.TargetTabId > 0)
            {
                map.TargetTabId = model.TargetTabId;
                map.TargetUrl = DotNetNuke.Common.Globals.NavigateURL(map.TargetTabId, PortalSettings, "");
            }
            else
            {
                // remove mapping
                RedirectConfig.Instance.Mappings.Remove(map);
                map = null;
            }
            if (map != null)
            {
                RedirectConfig.Instance.Mappings.Add(map);
            }

            RedirectConfig.Instance.ToFile(Common.RedirectConfigFile());
            RedirectConfig.Reload(PortalSettings.PortalId);

            // set handledon/handledby in table
            if(!model.UseRegex)
                RedirectController.SetHandledUrl(model.SourceUrl);

            return Request.CreateResponse(HttpStatusCode.OK, map == null ? null : new MappingModel(map));
        }

    }
}