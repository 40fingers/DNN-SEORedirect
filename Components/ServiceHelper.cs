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
        private const string CachedInstanceKey = "SEORedirect_ServiceHelper_Instance";
        private const string CachedProviderKey = "SEORedirect_ServiceProvider_Instance";

        public static ServiceHelper Instance => ResolveForRequest(HttpContextSource.Current);
        internal static ServiceHelper I => Instance;

        internal static ServiceHelper ResolveForRequest(HttpContextBase context)
        {
            if (context == null && HttpContext.Current == null)
                throw new InvalidOperationException("ServiceHelper can only be resolved during an active HTTP request.");

            // Check if we already have a cached instance for this request
            if (HttpContext.Current?.Items[CachedInstanceKey] is ServiceHelper cachedInstance)
            {
                return cachedInstance;
            }

            IServiceProvider provider = null;

            // Check if we already have a cached provider for this request
            if (HttpContext.Current?.Items[CachedProviderKey] is IServiceProvider cachedProvider)
            {
                provider = cachedProvider;
            }
            else
            {
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

                    // Cache the provider for this request
                    if (provider != null && HttpContext.Current != null)
                    {
                        HttpContext.Current.Items[CachedProviderKey] = provider;
                    }
                }
                catch
                {
                    // GetScope might throw during initialization
                }
            }

            if (provider == null)
                throw new InvalidOperationException("Unable to locate the request service provider. DI may not be initialized yet.");

            ServiceHelper helperInstance = null;

            // Prefer resolving the scoped instance registered in DI
            if (provider.GetService(typeof(ServiceHelper)) is ServiceHelper diHelper)
            {
                helperInstance = diHelper;
            }
            else
            {
                // Fallback: resolve dependencies manually if the container has not been configured
                var portalSvc = provider.GetService(typeof(IPortalAliasService)) as IPortalAliasService;
                var navSvc = provider.GetService(typeof(INavigationManager)) as INavigationManager;
                if (portalSvc != null && navSvc != null)
                {
                    helperInstance = new ServiceHelper(portalSvc, navSvc);
                }
            }

            if (helperInstance == null)
                throw new InvalidOperationException("ServiceHelper could not be resolved. Required services (IPortalAliasService, INavigationManager) are not available.");

            // Cache the instance for this request
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[CachedInstanceKey] = helperInstance;
            }

            MarkReady();
            return helperInstance;
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