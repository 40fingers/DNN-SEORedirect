using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Scheduling;

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

        public static string IncomingUrl
        {
            get
            {
                if (HttpContext.Current.Items["40F_SEO_IncomingUrl"] != null)
                    return (string)HttpContext.Current.Items["40F_SEO_IncomingUrl"];

                return "";
            }
            set => HttpContext.Current.Items["40F_SEO_IncomingUrl"] = value;
        }

        public static bool Is404Detected
        {
            get
            {
                if (HttpContext.Current.Items["40F_SEO_404Detected"] != null)
                    return (bool)HttpContext.Current.Items["40F_SEO_404Detected"];

                return false;
            }
            set => HttpContext.Current.Items["40F_SEO_404Detected"] = value;
        }
        public static bool IsAlreadyLogged
        {
            get
            {
                if (HttpContext.Current.Items["40F_SEO_AlreadyLogged"] != null)
                    return (bool)HttpContext.Current.Items["40F_SEO_AlreadyLogged"];

                return false;
            }
            set => HttpContext.Current.Items["40F_SEO_AlreadyLogged"] = value;
        }

        public static string RedirectConfigFile()
        {
            var file = Globals.ResolveUrl(String.Format("{0}\\{1}", CurrentPortalSettings.HomeDirectoryMapPath,
                Constants.PORTALREDIRECTCONFIGFILE));

            // if the file doesn't exist, we might need to rename it
            if (!File.Exists(file))
            {
                var oldFile = Globals.ResolveUrl(String.Format("{0}\\{1}", CurrentPortalSettings.HomeDirectoryMapPath,
                    Constants.PORTALREDIRECTCONFIGFILE_OLD));
                if (File.Exists(oldFile)) File.Move(oldFile, file);
            }

            return file;
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

        public static bool IsForce404(this TabInfo tab)
        {
            if (tab.TabSettings.ContainsKey(Constants.Force404TabSetting) && (string)tab.TabSettings[Constants.Force404TabSetting] == bool.TrueString)
            {
                return true;
            }

            return false;
        }

        public static void SetForce404(this TabInfo tab, bool newValue)
        {
            new TabController().UpdateTabSetting(tab.TabID, Constants.Force404TabSetting, newValue.ToString());
        }

        /// <summary>
        /// Redirect current response to 404 error page or output 404 content if error page not defined.
        /// Method copied from DNN8.0.4 UrlUtils, because it's missing in 7
        /// </summary>
        /// <param name="response"></param>
        /// <param name="portalSetting"></param>
        public static void Handle404Exception(HttpResponse response, PortalSettings portalSetting)
        {
            if (portalSetting?.ErrorPage404 > Null.NullInteger)
            {
                response.Redirect(Globals.NavigateURL(portalSetting.ErrorPage404, string.Empty, "status=404"));
            }
            else
            {
                response.ClearContent();
                response.TrySkipIisCustomErrors = true;
                response.StatusCode = 404;
                response.Status = "404 Not Found";
                response.Write("404 Not Found");
                response.End();
            }
        }

        public static ScheduleItem GetScheduleItem(string fullTypeName)
        {
            ScheduleItem retval = null;

            var provider = SchedulingProvider.Instance();

            var list = new List<ScheduleItem>();
            list.AddRange(provider.GetSchedule().OfType<ScheduleItem>());

            if (list.Any(s => s.TypeFullName == fullTypeName))
                retval = list.First(s => s.TypeFullName == fullTypeName);

            return retval;
        }
        public static bool IsScheduleItemEnabled(string fullTypeName)
        {
            var item = GetScheduleItem(fullTypeName);
            return item != null && item.Enabled;
        }

        public static ScheduleItem EnableScheduleItem(string fullTypeName)
        {
            var provider = SchedulingProvider.Instance();
            var item = GetScheduleItem(fullTypeName);

            if (item == null)
            {
                item = new ScheduleItem();
                item.CatchUpEnabled = false;
                item.FriendlyName = Constants.CleanerTaskName;
                item.RetainHistoryNum = 50;
                item.RetryTimeLapse = -1;
                item.RetryTimeLapseMeasurement = "d";
                item.TimeLapse = 1;
                item.TimeLapseMeasurement = "d";
                item.TypeFullName = fullTypeName;

                item.ScheduleID = provider.AddSchedule(item);
            }

            item.Enabled = true;
            provider.UpdateSchedule(item);

            return item;
        }
        public static void DisableScheduleItem(string fullTypeName)
        {
            var provider = SchedulingProvider.Instance();
            var item = GetScheduleItem(fullTypeName);

            if (item != null)
            {
                item.Enabled = false;
                provider.UpdateSchedule(item);
            }
        }

    }
}