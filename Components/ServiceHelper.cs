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
        private static readonly Lazy<ServiceHelper> SttaticInstance = new Lazy<ServiceHelper>(() => new ServiceHelper(), isThreadSafe: true);

        public static ServiceHelper Instance => SttaticInstance.Value;
        internal static ServiceHelper I => Instance;

        private IPortalAliasService _portalAliasService;

        internal IPortalAliasService PortalAliasService => _portalAliasService ?? (_portalAliasService = ResolvePortalAliasService());

        // keep ctor cheap so static init won't hit DI
        public ServiceHelper() { }

        public ServiceHelper(IPortalAliasService portalAliasService)
        {
            _portalAliasService = portalAliasService;
        }

        private static IPortalAliasService ResolvePortalAliasService()
        {
            try
            {
                var lsp = new LazyServiceProvider();
                if (lsp.GetService(typeof(IPortalAliasService)) is IPortalAliasService svc) return svc;
            }
            catch
            {
                // service provider not ready -> fall through to fallback
            }

            // Fallback: use classic controller if available (avoid throwing)
            try
            {
                return PortalAliasController.Instance as IPortalAliasService;
            }
            catch
            {
                return null;
            }
        }
    }
}