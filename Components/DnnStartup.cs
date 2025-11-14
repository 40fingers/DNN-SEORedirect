using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FortyFingers.SeoRedirect.Components
{
    public class DnnStartup : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<IPortalAliasService, IPortalAliasService>();

            // IndexModel registration is required for
            // constructor injection to work
            services.AddSingleton<ServiceHelper>();
        }
    }
}