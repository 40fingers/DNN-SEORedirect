﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

namespace FortyFingers.SeoRedirect.Components
{
    public class Config
    {
        #region Standard code

        private Hashtable _settings = null;
        private int _tabModuleId = Null.NullInteger;

        /// <summary>
        /// 40fingers config class 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="moduleId"></param>
        /// <param name="tabModuleId"></param>
        public Config(Hashtable settings, int moduleId, int tabModuleId)
        {
            _settings = settings;
            ModuleId = moduleId;
            _tabModuleId = tabModuleId;
        }
        /// <summary>
        /// module id
        /// </summary>
        public int ModuleId
        {
            get;
            private set;
        }

        private string GetDefault(string setting)
        {
            return Localization.GetString(setting, ConfigDefaultsResourceFile);
        }

        private int GetSettingInt(string setting, bool useDefault = true)
        {
            int i = Null.NullInteger;
            string settingValue = "";
            if (_settings.ContainsKey(setting))
            {
                settingValue = _settings[setting].ToString();
            }
            else if (useDefault)
            {
                settingValue = GetDefault(setting);
            }
            int.TryParse(settingValue, out i);

            return i;
        }

        private DateTime GetSettingDateTime(string setting, bool useDefault = true)
        {
            DateTime d = Null.NullDate;
            string settingValue = "";
            if (_settings.ContainsKey(setting))
            {
                settingValue = _settings[setting].ToString();
            }
            else if (useDefault)
            {
                settingValue = GetDefault(setting);
            }
            DateTime.TryParse(settingValue, out d);

            return d;
        }

        private string GetSetting(string setting, bool useDefault = true)
        {
            string settingValue = "";
            if (_settings.ContainsKey(setting))
            {
                settingValue = _settings[setting].ToString();
            }
            else if (useDefault)
            {
                settingValue = GetDefault(setting);
            }

            return settingValue;
        }

        private PortalSettings Ps
        {
            get { return PortalSettings.Current; }
        }

        private ModuleController _moduleCtrl;
        private ModuleController ModuleCtrl
        {
            get
            {
                if (_moduleCtrl == null)
                {
                    _moduleCtrl = new ModuleController();
                }

                return _moduleCtrl;
            }
        }
        #endregion

        private const string ConfigDefaultsResourceFile = "~/DesktopModules/40Fingers/SeoRedirect/App_LocalResources/ConfigDefaults.resx";

        /// <summary>
        /// number of settings to show
        /// </summary>
        public int NoOfEntries
        {
            get { return GetSettingInt("NoOfEntries", true); }
            set { ModuleCtrl.UpdateModuleSetting(ModuleId, "NoOfEntries", value.ToString()); }
        }

    }
}