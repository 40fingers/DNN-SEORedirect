using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
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

        private static string getConfigLockObject = "lockIt";
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
                lock (getConfigLockObject)
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
            lock (mappingsDicLockObject)
            {
                _MappingsDictionary = null;
            }
            lock (mappingsDicRegexLockObject)
            {
                _MappingsDictionaryRegex = null;
            }
        }

        //private static string RedirectMappingsDictionaryCacheKey(bool usingRegex)
        //{
        //    return String.Format("40FINGERS.SeoRedirect.MAPPINGS,{0},{1}", Common.CurrentPortalSettings.PortalId, usingRegex);
        //}
        private static string loggingListLockObject = "lockIt";
        private static string mappingsDicLockObject = "lockIt";
        private static string mappingsDicRegexLockObject = "lockIt";
        private List<string> _LoggingSourceUrls = null;
        private Dictionary<string, string> _MappingsDictionary = null;
        private Dictionary<string, string> _MappingsDictionaryRegex = null;
        public Dictionary<string, string> MappingsDictionary(bool usingRegex)
        {
            if (usingRegex && _MappingsDictionaryRegex != null)
            {
                return _MappingsDictionaryRegex;
            }
            if (!usingRegex && _MappingsDictionary != null)
            {
                return _MappingsDictionary;
            }

            var lockobject = usingRegex ? mappingsDicRegexLockObject : mappingsDicLockObject;

            lock (lockobject)
            {
                Dictionary<string, string> dic = null;

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
                dic = new Dictionary<string, string>();
                // add the sourceurl lowercased: we're case insensitive
                foreach (var mapping in Mappings)
                {
                    if (mapping.UseRegex == usingRegex && !dic.ContainsKey(mapping.SourceUrl.ToLower()))
                    {
                        string targetUrl;
                        if (mapping.TargetTabId > 0)
                        {
                            targetUrl = mapping.TargetUrl;
                            //targetUrl = Globals.NavigateURL(mapping.TargetTabId, Common.CurrentPortalSettings, "");
                        }
                        else
                        {
                            targetUrl = mapping.TargetUrl;
                        }
                        dic.Add(mapping.SourceUrl.ToLower(), targetUrl);
                    }
                }

                if (usingRegex)
                {
                    _MappingsDictionaryRegex = dic;
                }
                else
                {
                    _MappingsDictionary = dic;
                }

                //DataCache.SetCache(RedirectMappingsDictionaryCacheKey(usingRegex), dic, new DNNCacheDependency(Common.RedirectConfigFile()));
                return dic;
            }
        }

        public bool IsLoggingEnabled(string mappingSourceUrl)
        {
            if (_LoggingSourceUrls == null)
            {
                lock (loggingListLockObject)
                {
                    _LoggingSourceUrls = new List<string>();
                    foreach (var mapping in Mappings)
                    {
                        if (mapping.EnableLogging && !_LoggingSourceUrls.Contains(mapping.SourceUrl.ToLower()))
                            _LoggingSourceUrls.Add(mapping.SourceUrl.ToLower());
                    }
                }
            }

            return _LoggingSourceUrls.Contains(mappingSourceUrl);
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
    }
}