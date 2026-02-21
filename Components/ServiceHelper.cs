using DotNetNuke.Abstractions.Portals;
using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FortyFingers.SeoRedirect.Components
{
    public class ServiceHelper
    {
        private const string ReadyFlag = "SEORedirect_ServiceHelper_Ready";

        public static ServiceHelper Instance => ResolveForRequest(HttpContextSource.Current);
        internal static ServiceHelper I => Instance;

        internal static ServiceHelper ResolveForRequest(HttpContextBase context)
        {
            if (context == null && HttpContext.Current == null)
                throw new InvalidOperationException("ServiceHelper can only be resolved during an active HTTP request.");

            IServiceProvider provider = null;

            try
            {
                // Try DNN 10+ with scoped provider first
                if (context != null)
                {
                    provider = context.GetScope()?.ServiceProvider;
                }

                // In DNN 9, GetScope() returns null even when context exists
                // Fallback: access Globals.DependencyProvider via reflection
                if (provider == null)
                {
                    var globalsType = typeof(Globals);
                    var dependencyProviderProperty = globalsType.GetProperty("DependencyProvider", 
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (dependencyProviderProperty != null)
                    {
                        provider = dependencyProviderProperty.GetValue(null) as IServiceProvider;
                    }
                }
            }
            catch
            {
                // GetScope might throw during initialization
            }

            if (provider == null)
                throw new InvalidOperationException("Unable to locate the request service provider. DI may not be initialized yet.");

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

            throw new InvalidOperationException("ServiceHelper could not be resolved. Required services (IPortalAliasService, INavigationManager) are not available.");
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