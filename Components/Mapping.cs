using System;
using System.Xml;
using System.Xml.Serialization;

namespace FortyFingers.SeoRedirect.Components
{
    public class Mapping
    {
        // constructor to set defaults
        public Mapping()
        {
            EnableLogging = true;
        }

        // dummy xmldocument for creating cdata elements
        [XmlIgnore]
        private XmlDocument _dummyXmlDoc = null;
        [XmlIgnore]
        private XmlDocument DummyXmlDoc
        {
            get
            {
                if(_dummyXmlDoc == null)
                    _dummyXmlDoc = new XmlDocument();

                return _dummyXmlDoc;
            }
        }

        /// <summary>
        /// Id of the mapping
        /// </summary>
        public string Id { get; set; }

        // Ignore this property in serialization because XmlSerializer doesn't support CDATA
        [XmlIgnore]
        public string SourceUrl { get; set; }

        // Serialize this eloement as SourceUrl
        // Method will only be called while serializing
        [XmlElement("SourceUrl")]
        public XmlCDataSection CDataSourceUrl
        {
            get
            {
                return DummyXmlDoc.CreateCDataSection(SourceUrl);
            }
            set
            {
                SourceUrl = value.Value;
            }
        }

        // Ignore this property in serialization because XmlSerializer doesn't support CDATA
        [XmlIgnore]
        public string TargetUrl { get; set; }

        // Serialize this eloement as TargetUrl
        // Method will only be called while serializing
        [XmlElement("TargetUrl")]
        public XmlCDataSection CDataTargetUrl
        {
            get
            {
                return DummyXmlDoc.CreateCDataSection(TargetUrl);
            }
            set
            {
                TargetUrl = value.Value;
            }
        }

        [XmlElement("TargetTabId")]
        public int TargetTabId { get; set; }

        [XmlElement("UseRegex")]
        public bool UseRegex { get; set; }
        [XmlElement("EnableLogging")]
        public bool EnableLogging { get; set; }
        [XmlElement("StatusCode")]
        public Constants.HttpRedirectStatus StatusCode { get; set; } = Constants.HttpRedirectStatus.MovedPermanently;
    }
}