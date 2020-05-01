using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace FortyFingers.SeoRedirect.Components.Data
{
    public class SqlDataProvider : DataProvider
    {
        private const string OwnerPrefix = "40F_";
        private const string ModulePrefix = "SEO_";

        private const string ProviderType = "data";

        private readonly string _connectionString;

        private readonly string _databaseOwner;
        private readonly string _objectQualifier;

        private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);

        private readonly string _providerPath;

        private string GetObjectName(string shortName)
        {
            return DatabaseOwner + ObjectQualifier + OwnerPrefix + ModulePrefix + shortName;
        }

        #region Constructors

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            var objProvider = (Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

            // Read the attributes for this provider
            _connectionString = DotNetNuke.Data.DataProvider.Instance().ConnectionString;

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }
        }

        #endregion

        #region Properties

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        #endregion

        public override void AddRedirectLog(int portalId, string requestedUrl, DateTime requestDateTime, string referrer, string httpUserAgent, string redirectedToUrl, bool isHandled = false)
        {
            if (isHandled)
                SqlHelper.ExecuteNonQuery(ConnectionString, GetObjectName("AddRedirectLogHandled"), portalId, requestedUrl, requestDateTime, referrer, httpUserAgent, redirectedToUrl);
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, GetObjectName("AddRedirectLog"), portalId, requestedUrl, requestDateTime, referrer, httpUserAgent, redirectedToUrl);
        }

        public override IDataReader GetTopUnhandledUrls(int portalId, DateTime startDate, int maxUrls)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetObjectName("GetTopUnhandledUrls"), portalId, startDate, maxUrls);
        }

        public override void SetHandledUrl(string url, DateTime handledOn, string handledBy)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetObjectName("SetHandledUrl"), url, handledOn, handledBy);
        }

        public override void CleanupRedirectLog(int portalId, int maxAgeDays, int maxEntries)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetObjectName("CleanRedirectLog"), portalId, maxAgeDays, maxEntries);
        }
    }
}