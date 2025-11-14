using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Services.Exceptions;
using log4net.Core;

namespace FortyFingers.SeoRedirect.Components
{
    public class ServiceHelper
    {
        private static readonly Lazy<ServiceHelper> StaticInstance = new Lazy<ServiceHelper>(() =>
        {
            try
            {
                // we need to get the IServiceProvider from the current HttpContext
                // it's set there by DNN, so contains a lot of the registered services already
                var lsp = HttpContextSource.Current?.GetScope()?.ServiceProvider;

                // See if a ServiceHelper is already registered in DI
                if (lsp.GetService(typeof(ServiceHelper)) is ServiceHelper diHelper)
                    return diHelper;

                // Otherwise resolve the dependency and construct
                // this seems to be never called, but it's a good fallback
                var portalSvc = lsp.GetService(typeof(IPortalAliasService)) as IPortalAliasService;
                var navSvc = lsp.GetService(typeof(INavigationManager)) as INavigationManager;
                if (portalSvc != null && navSvc != null)
                {
                    // set indication to HttpApplication to indicate it is ready
                    HttpContext.Current.Application["SEORedirect_ServiceHelper_Ready"] = true;

                    return new ServiceHelper(portalSvc, navSvc);
                }
            }
            catch
            {
            }

            return null;
        }, isThreadSafe: true);

        public static ServiceHelper Instance => StaticInstance.Value;
        internal static ServiceHelper I => Instance;

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