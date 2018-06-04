using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortyFingers.SeoRedirect.Components.Data
{
    public class RedirectLogItem
    {
        public int Id { get; set; }
        public string RequestedUrl { get; set; }
        public DateTime RequestDateTime { get; set; }
        public string Referrer { get; set; }
        public string HTTP_USER_AGENT { get; set; }
        public string RedirectedToUrl { get; set; }
        public DateTime HandledOn { get; set; }
        public string HandledBy { get; set; }
        public int PortalId { get; set; }
    }

    public class RedirectLogUrl
    {
        public string Url { get; set; }
        public int Days { get; set; }
        public int Occurrences { get; set; }
    }
}