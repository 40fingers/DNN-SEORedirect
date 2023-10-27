using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Exceptions;

namespace FortyFingers.SeoRedirect.Components
{
    public class RedirectConfig
    {
        #region Singleton stuff

        /// <summary>
        /// Static field to hold the instance of the RedirectConfigs
        /// </summary>
        private static Dictionary<int, RedirectConfig> _instances;

        /// <summary>
        /// Lock oject to prevent sync issues between threads
        /// </summary>
        private static object _lock = new object();

        public static RedirectConfig Instance
        {
            get
            {
                if ((_instances == null))
                {
                    lock (_lock)
                    {
                        _instances = new Dictionary<int, RedirectConfig>();
                    }
                }

                var pid = Common.CurrentPortalSettings.PortalId;
                if (!_instances.ContainsKey(pid))
                {
                    lock (_lock)
                    {
                        _instances.Add(pid, GetConfig());
                    }
                }

                return _instances[pid];
            }
        }

        #endregion


        public RedirectConfig()
        {
            Mappings = new List<Mapping>();
        }

        public static void Reload(int portalId)
        {
            if (_instances.ContainsKey(Common.CurrentPortalSettings.PortalId))
            {
                lock (_lock)
                {
                    _instances.Remove(portalId);
                }
            }
        }

        //private static string RedirectConfigCacheKey
        //{
        //    get { return String.Format("40FINGERS.SeoRedirect.CONFIG,{0}", Common.CurrentPortalSettings.PortalId); }
        //}
        //public static RedirectConfig GetConfig()
        //{
        //    return GetConfig(true);
        //}

        private static string _getConfigLockObject = "lockIt";
        private static RedirectConfig GetConfig()
        {
            RedirectConfig config = null;
            //if (useCache)
            //    try
            //    {
            //        config = DataCache.GetCache<RedirectConfig>(RedirectConfigCacheKey);
            //    }
            //    catch (Exception e)
            //    {
            //        /* do nothing */
            //    }

            // if there are no mappings, the use of this module is pointless
            // so we'll assume something went wrong reading the file in that case
            if (config == null || config.Mappings == null || config.Mappings.Count == 0)
            {
                lock (_getConfigLockObject)
                {
                    var file = Common.RedirectConfigFile();
                    config = FromFile(file);
                    // set the id's if they're empty. This comes in handy for backwards compatibility
                    if (config.Mappings.Any(m => String.IsNullOrEmpty(m.Id)))
                    {
                        foreach (var mapping in config.Mappings.Where(m => String.IsNullOrEmpty(m.Id)))
                        {
                            mapping.Id = Guid.NewGuid().ToString();
                        }
                        config.ToFile(file);
                    }
                    //DataCache.SetCache(RedirectConfigCacheKey, config, new DNNCacheDependency(file));
                }
            }
            return config;
        }

        public void ResetMappingDictionaries()
        {
            lock (_mappingsDicLockObject)
            {
                _mappingsDictionary = null;
            }
            lock (_mappingsDicRegexLockObject)
            {
                _mappingsDictionaryRegex = null;
            }
        }

        //private static string RedirectMappingsDictionaryCacheKey(bool usingRegex)
        //{
        //    return String.Format("40FINGERS.SeoRedirect.MAPPINGS,{0},{1}", Common.CurrentPortalSettings.PortalId, usingRegex);
        //}
        private static string _loggingListLockObject = "lockIt";
        private static string _statusCodesDicLockObject = "lockIt";
        private static string _mappingsDicLockObject = "lockIt";
        private static string _mappingsDicRegexLockObject = "lockIt";
        private List<string> _loggingSourceUrls = null;
        private Dictionary<string, Constants.HttpRedirectStatus> _statusCodesDictionary = null;
        private Dictionary<string, Mapping> _mappingsDictionary = null;
        private Dictionary<string, Mapping> _mappingsDictionaryRegex = null;
        public Dictionary<string, Mapping> MappingsDictionary(bool usingRegex)
        {
            if (usingRegex && _mappingsDictionaryRegex != null)
            {
                return _mappingsDictionaryRegex;
            }
            if (!usingRegex && _mappingsDictionary != null)
            {
                return _mappingsDictionary;
            }

            var lockobject = usingRegex ? _mappingsDicRegexLockObject : _mappingsDicLockObject;

            lock (lockobject)
            {
                Dictionary<string, Mapping> dic = null;

                //try
                //{
                //    dic = DataCache.GetCache<Dictionary<string, string>>(RedirectMappingsDictionaryCacheKey(usingRegex));
                //}
                //catch (NullReferenceException e)
                //{
                //    // do nothing. we should handle caching differently
                //}
                // if there are no mappings, the use of this module is pointless
                // so we'll assume something went wrong reading the file in that case
                dic = new Dictionary<string, Mapping>();
                // add the sourceurl lowercased: we're case insensitive
                foreach (var mapping in Mappings)
                {
                    if (mapping.UseRegex == usingRegex && !dic.ContainsKey(mapping.SourceUrl.ToLower()))
                    {
                        dic.Add(mapping.SourceUrl.ToLower(), mapping);
                    }
                }

                if (usingRegex)
                {
                    _mappingsDictionaryRegex = dic;
                }
                else
                {
                    _mappingsDictionary = dic;
                }

                //DataCache.SetCache(RedirectMappingsDictionaryCacheKey(usingRegex), dic, new DNNCacheDependency(Common.RedirectConfigFile()));
                return dic;
            }
        }

        public bool IsLoggingEnabled(string mappingSourceUrl)
        {
            if (_loggingSourceUrls == null)
            {
                lock (_loggingListLockObject)
                {
                    _loggingSourceUrls = new List<string>();
                    foreach (var mapping in Mappings)
                    {
                        if (mapping.EnableLogging && !_loggingSourceUrls.Contains(mapping.SourceUrl.ToLower()))
                            _loggingSourceUrls.Add(mapping.SourceUrl.ToLower());
                    }
                }
            }

            return _loggingSourceUrls.Contains(mappingSourceUrl);
        }
        public Constants.HttpRedirectStatus GetRedirectStatus(string mappingSourceUrl)
        {
            if (_statusCodesDictionary == null)
            {
                lock (_statusCodesDicLockObject)
                {
                    _statusCodesDictionary = new Dictionary<string, Constants.HttpRedirectStatus>();
                    foreach (var mapping in Mappings)
                    {
                        if(!_statusCodesDictionary.ContainsKey(mapping.SourceUrl.ToLower()))
                            _statusCodesDictionary.Add(mapping.SourceUrl.ToLower(), mapping.StatusCode);
                    }
                }
            }

            return _statusCodesDictionary.ContainsKey(mappingSourceUrl) ? _statusCodesDictionary[mappingSourceUrl] : Constants.HttpRedirectStatus.MovedPermanently;
        }
        //private static void ClearCache()
        //{
        //    DataCache.RemoveCache(RedirectConfigCacheKey);
        //    DataCache.RemoveCache(RedirectMappingsDictionaryCacheKey(true));
        //    DataCache.RemoveCache(RedirectMappingsDictionaryCacheKey(false));
        //}

        //public static bool Exists()
        //{
        //    return File.Exists(Common.RedirectConfigFile());
        //}
        private static RedirectConfig FromFile(string filename)
        {
            // create the file if it doesn't exsist
            //if (!File.Exists(filename))
            //    CreateFile(filename);

            RedirectConfig retval = null;
            FileStream fs = null;

            try
            {
                // if it exists we can just open it
                // otherwise return new configuration
                if (File.Exists(filename))
                {
                    fs = new FileStream(filename, FileMode.Open);
                    retval = (RedirectConfig)XmlSer.Deserialize(fs);
                }
                else
                {
                    retval = new RedirectConfig();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return retval;
        }

        private static XmlSerializer XmlSer => new XmlSerializer(typeof(RedirectConfig));

        public void ToFile(string filename)
        {
            var filePath = filename.Substring(0, filename.LastIndexOf("\\"));
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            FileStream fs = null;

            try
            {
                if (File.Exists(filename))
                {
                    File.Copy(filename, filename.Replace(".xml", string.Format(".{0}.xml", DateTime.Now.ToString("yyyyMMdd-HHmmss"))));
                    File.Delete(filename);
                }

                fs = new FileStream(filename, FileMode.CreateNew);
                XmlSer.Serialize(fs, this);

                //ClearCache();
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        public static void CreateFile(string filename)
        {
            var emptyConfig = new RedirectConfig();
            emptyConfig.ToFile(filename);
        }

        [XmlArrayItem("Mapping")]
        public List<Mapping> Mappings { get; set; }

        private string _force404Lockobject = "lock";
        private IEnumerable<TabInfo> _force404Tabs = null;

        [XmlIgnore]
        public IEnumerable<TabInfo> Force404Tabs
        {
            get
            {
                if (_force404Tabs == null)
                {
                    lock (_force404Lockobject)
                    {
                        if (_force404Tabs == null)
                        {
                            var allTabs = TabController.GetPortalTabs(Common.CurrentPortalSettings.PortalId,
                                Null.NullInteger,
                                false,
                                "",
                                true,
                                false,
                                true,
                                false,
                                false);

                            _force404Tabs = allTabs.Where(t => t.IsForce404()).AsEnumerable<TabInfo>();
                        }
                    }
                }

                return _force404Tabs;
            }
        }
    }
}