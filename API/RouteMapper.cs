using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DotNetNuke.Web.Api;

namespace FortyFingers.SeoRedirect.API
{
    public class RouteMapper : IServiceRouteMapper
    {
        /// <summary>
        /// RegisterRoutes is used to register the module's routes
        /// </summary>
        /// <param name="mapRouteManager"></param>
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "40Fingers",
                routeName: "default",
                url: "{controller}/{action}/{itemId}",
                defaults: new { itemId = RouteParameter.Optional },
                namespaces: new[] { "FortyFingers.SeoRedirect.API" });

        }
    }
}