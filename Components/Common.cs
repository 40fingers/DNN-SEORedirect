using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;

namespace FortyFingers.SeoRedirect.Components
{
    public static class Common
    {
        public static ILog Logger
        {
            get
            {
                return LoggerSource.Instance.GetLogger(Constants.MODULE_LOGGER_NAME);
            }
        }

        public static PortalSettings CurrentPortalSettings
        {
            get 
            { 
                var retval = PortalSettings.Current;

                // if there's no current portal, try and get it from the requested domain name
                if (retval == null)
                {
                    var domainName = Globals.GetDomainName(HttpContext.Current.Request, true);

                    // in multiligual sites, DNN6 might have the current locale appended
                    var portalAliasInfo = PortalAliasController.GetPortalAliasInfo(domainName);
                    if (portalAliasInfo == null)
                    {
                        if (Regex.IsMatch(domainName, ".*//??-??"))
                        {
                            domainName = domainName.Substring(0, domainName.IndexOf("/"));
                            portalAliasInfo = PortalAliasController.GetPortalAliasInfo(domainName);
                        }
                    }

                    if (portalAliasInfo != null)
                    {
                        retval = new PortalSettings(portalAliasInfo.PortalID);
                    }
                }
                return retval;
            }
        }

        public static string RedirectConfigFile()
        {
            return Globals.ResolveUrl(String.Format("{0}\\{1}", CurrentPortalSettings.HomeDirectoryMapPath,
                                                 Constants.PORTALREDIRECTCONFIGFILE));
        }

        public static void AddCssLink(string linkFile, Page page)
        {
            HtmlLink oLink = new HtmlLink();
            if (!linkFile.EndsWith("/"))
            {
                oLink.Attributes["rel"] = "stylesheet";
                oLink.Attributes["media"] = "screen";
                oLink.Attributes["type"] = "text/css";
                oLink.Attributes["href"] = linkFile;
                Control oCSS = page.FindControl("CSS");
                // try to insert it in a DNN way first
                if ((oCSS != null))
                {
                    oCSS.Controls.Add(oLink);
                }
                else
                {
                    page.Header.Controls.Add(oCSS);
                }
            }
        }

        public static bool IsInt(string s)
        {
            int i;
            var retval = int.TryParse(s.Trim(), out i);

            return retval;
        }
    }
}