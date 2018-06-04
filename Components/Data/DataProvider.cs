using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Framework;

namespace FortyFingers.SeoRedirect.Components.Data
{
    public abstract class DataProvider
    {
        #region Shared/Static Methods

        // singleton reference to the instantiated object 

        private static DataProvider objProvider;
        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Reflection.CreateObject("data", "FortyFingers.SeoRedirect.Components.Data", "");
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        public abstract void AddRedirectLog(int portalId, string requestedUrl, DateTime requestDateTime, string referrer,
                                       string httpUserAgent, string redirectedToUrl);

        public abstract IDataReader GetTopUnhandledUrls(int portalId, DateTime startDate, int maxUrls);
        public abstract void SetHandledUrl(string url, DateTime handledOn, string handledBy);
    }
}