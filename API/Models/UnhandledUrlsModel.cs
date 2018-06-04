using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FortyFingers.SeoRedirect.Components.Data;
using Newtonsoft.Json;

namespace FortyFingers.SeoRedirect.API.Models
{
    public class UnhandledUrlsModel
    {
        public UnhandledUrlsModel()
        {
            Urls = new List<UnhandledUrlModel>();
        }
        [JsonProperty("urls")]
        public List<UnhandledUrlModel> Urls { get; set; }
    }

    public class UnhandledUrlModel
    {
        public UnhandledUrlModel(RedirectLogUrl url)
        {
            Url = url.Url;
            Days = url.Days;
            Occurrences = url.Occurrences;
        }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("days")]
        public int Days { get; set; }
        [JsonProperty("occurrences")]
        public int Occurrences { get; set; }
    }
}