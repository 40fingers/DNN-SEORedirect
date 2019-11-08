using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using FortyFingers.SeoRedirect.Components;
using FortyFingers.SeoRedirect.Components.Data;
using Newtonsoft.Json;

namespace FortyFingers.SeoRedirect.API.Models
{
    public class MappingsModel
    {
        public MappingsModel()
        {
            Mappings = new List<MappingModel>();
        }
        [JsonProperty("mappings")]
        public List<MappingModel> Mappings { get; set; }
    }

    public class MappingModel
    {
        public MappingModel() { }
        public MappingModel(Mapping mapping) : this()
        {
            Id = mapping.Id;
            SourceUrl = mapping.SourceUrl;
            TargetUrl = mapping.TargetUrl;
            TargetTabId = mapping.TargetTabId;
            UseRegex = mapping.UseRegex;
            EnableLogging = mapping.EnableLogging;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sourceUrl")]
        public string SourceUrl { get; set; }
        [JsonProperty("targetUrl")]
        public string TargetUrl { get; set; }
        [JsonProperty("targetTabId")]
        public int TargetTabId { get; set; }
        [JsonProperty("targetTabName")]
        public string TargetTabName
        {
            get
            {
                var retval = "";
                if (TargetTabId > 0)
                {
                    var tab = new TabController().GetTab(TargetTabId, PortalSettings.Current.PortalId, false);
                    if (tab != null)
                    {
                        retval = tab.TabName;
                    }
                }
                return retval;
            }
        }
        [JsonProperty("useRegex")]
        public bool UseRegex { get; set; }
        [JsonProperty("enableLogging")]
        public bool EnableLogging { get; set; }
    }
}