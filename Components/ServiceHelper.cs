using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortyFingers.SeoRedirect.Components
{
    public class ServiceHelper
    {
        private static readonly Lazy<ServiceHelper> StaticInstance = new Lazy<ServiceHelper>(() => 
        {
            try
            {
                var lsp = new LazyServiceProvider();

                // Prefer a ServiceHelper registered in DI
                if (lsp.GetService(typeof(ServiceHelper)) is ServiceHelper diHelper)
                    return diHelper;

                // Otherwise resolve the dependency and construct
                var portalSvc = lsp.GetService(typeof(IPortalAliasService)) as IPortalAliasService;
                if (portalSvc != null)
                    return new ServiceHelper(portalSvc);
            }
            catch
            {
                // service provider not ready -> fall through to fallback
            }

            // Final fallback: classic controller
            return new ServiceHelper(PortalAliasController.Instance as IPortalAliasService);
        }, isThreadSafe: true);

        public static ServiceHelper Instance => StaticInstance.Value;
        internal static ServiceHelper I => Instance;

        private IPortalAliasService _portalAliasService;

        internal IPortalAliasService PortalAliasService => _portalAliasService;

        public ServiceHelper(IPortalAliasService portalAliasService)
        {
            _portalAliasService = portalAliasService;
        }

    }
}