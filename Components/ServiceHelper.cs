using DotNetNuke.Abstractions.Portals;
using System;
using System.Web;
using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Common.Extensions;

namespace FortyFingers.SeoRedirect.Components
{
    public class ServiceHelper
    {
        private const string ReadyFlag = "SEORedirect_ServiceHelper_Ready";

        public static ServiceHelper Instance => ResolveForRequest(HttpContextSource.Current);
        internal static ServiceHelper I => Instance;

        internal static ServiceHelper ResolveForRequest(HttpContextBase context)
        {
            if (context == null)
                throw new InvalidOperationException("ServiceHelper can only be resolved during an active HTTP request.");

            var provider = context.GetScope()?.ServiceProvider;
            if (provider == null)
                throw new InvalidOperationException("Unable to locate the request service provider.");

            // Prefer resolving the scoped instance registered in DI
            if (provider.GetService(typeof(ServiceHelper)) is ServiceHelper diHelper)
            {
                MarkReady();
                return diHelper;
            }

            // Fallback: resolve dependencies manually if the container has not been configured
            var portalSvc = provider.GetService(typeof(IPortalAliasService)) as IPortalAliasService;
            var navSvc = provider.GetService(typeof(INavigationManager)) as INavigationManager;
            if (portalSvc != null && navSvc != null)
            {
                MarkReady();
                return new ServiceHelper(portalSvc, navSvc);
            }

            throw new InvalidOperationException("ServiceHelper could not be resolved from the current request.");
        }

        private static void MarkReady()
        {
            var app = HttpContext.Current?.Application;
            if (app != null)
            {
                app[ReadyFlag] = true;
            }
        }

        private IPortalAliasService _portalAliasService;
        internal IPortalAliasService PortalAliasService => _portalAliasService;

        private INavigationManager _navigationManager;
        internal INavigationManager NavigationManager => _navigationManager;
        public ServiceHelper(IPortalAliasService portalAliasService, INavigationManager navigationManager)
        {
            _portalAliasService = portalAliasService;
            _navigationManager = navigationManager;
        }

    }
}