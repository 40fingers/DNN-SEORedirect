using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;
using FortyFingers.SeoRedirect.Components.Data;

namespace FortyFingers.SeoRedirect.Components
{
    public class TaskCleaner : SchedulerClient
    {
        public TaskCleaner(ScheduleHistoryItem objScheduleHistoryItem)
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                ScheduleHistoryItem.AddLogNote(String.Format("Cleaner started<br />", ThreadID));
                Progressing();

                var portals = PortalController.Instance.GetPortals();

                foreach (IPortalInfo portal in portals)
                {
                    if (PortalController.GetPortalSettingAsBoolean("40F_SEO_CleanerEnabled", portal.PortalId, false))
                    {
                        var maxAgeDays = PortalController.GetPortalSettingAsInteger("40F_SEO_MaxAgeDays", portal.PortalId, Null.NullInteger);
                        var maxEntries = PortalController.GetPortalSettingAsInteger("40F_SEO_MaxEntries", portal.PortalId, Null.NullInteger);
                        DataProvider.Instance().CleanupRedirectLog(portal.PortalId, maxAgeDays, maxEntries);
                        ScheduleHistoryItem.AddLogNote($"Portal {portal.PortalName} done.<br />");
                    }
                }
                ScheduleHistoryItem.AddLogNote("Finished.<br />");
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception e)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote($"Exception occurred: {e.Message}<br />");
                ScheduleHistoryItem.AddLogNote($"Export failed.<br />");
                DotNetNuke.Services.Exceptions.Exceptions.LogException(e);
                Errored(ref e);
            }
        }
    }
}