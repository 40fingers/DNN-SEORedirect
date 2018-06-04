namespace FortyFingers.SeoRedirect.Components
{

    public class Constants
    {
        /// <summary>
        /// Module root url, for use with ResolveUrl()
        /// </summary>
        public const string DESKTOPMODULES_MODULEROOT_URL = "~/DesktopModules/40Fingers/SeoRedirect/";

        public const string PORTALREDIRECTCONFIGFILE = "40Fingers\\SeoRedirect\\redirectconfig.xml";

        public const string SHAREDRESOURCES =
            "~/DesktopModules/40Fingers/SeoRedirect/App_LocalResources/SharedResources.resx";

        public const int UnhandledUrlsMaxDays = 30;
        //public const int UnhandledUrlsMaxResults = 5;

        public const string MODULE_LOGGER_NAME = "40Fingers.SeoRedirect";
    }
}