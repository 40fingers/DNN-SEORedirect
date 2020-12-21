namespace FortyFingers.SeoRedirect.Components
{

    public class Constants
    {
        /// <summary>
        /// Module root url, for use with ResolveUrl()
        /// </summary>
        public const string DESKTOPMODULES_MODULEROOT_URL = "~/DesktopModules/40Fingers/SeoRedirect/";

        public const string PORTALREDIRECTCONFIGFILE_OLD = "40Fingers\\SeoRedirect\\redirectconfig.xml";
        public const string PORTALREDIRECTCONFIGFILE = "40Fingers\\SeoRedirect\\redirectconfig.xml.resources";

        public const string SHAREDRESOURCES =
            "~/DesktopModules/40Fingers/SeoRedirect/App_LocalResources/SharedResources.resx";

        public const int UnhandledUrlsMaxDays = 30;
        //public const int UnhandledUrlsMaxResults = 5;

        public const string MODULE_LOGGER_NAME = "40Fingers.SeoRedirect";

        public const string Force404TabSetting = "40F_SEO_Force404";

        public const string CleanerTaskName = "40F Seo Cleaner";
        public const string CleanerTaskTypeName = "FortyFingers.SeoRedirect.Components.TaskCleaner, 40Fingers.DNN.Modules.SeoRedirect";

        public enum HttpRedirectStatus
        {
            MovedPermanently = 301,
            Found = 302,
            SeeOther = 303,
            NotModified = 304,
            UseProxy = 305,
            TemporaryRedirect = 307,
            PermanentRedirect = 308
        }
    }
}