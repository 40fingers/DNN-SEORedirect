using System;
using DotNetNuke;
using DotNetNuke.Entities.Modules;
using cfg = FortyFingers.SeoRedirect.Components.Config;

namespace FortyFingers.SeoRedirect
{
    public partial class Settings : ModuleSettingsBase
    {

        private cfg _config;
        private cfg Config
        {
            get
            {
                if (_config == null)
                    _config = new cfg(Settings, ModuleId, TabModuleId);

                return _config;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// loads the module settings
        /// </summary>
        public override void LoadSettings()
        {
            NoOfEntries.Text = Config.NoOfEntries.ToString();
            RegisterJquery.Checked = Config.RegisterJquery;
            EnableCleaner.Checked = Config.CleanerEnabled;
            MaxAgeDays.Text = Config.MaxAgeDays.ToString();
            MaxEntries.Text = Config.MaxEntries.ToString();
        }
        /// <summary>
        /// updates module settings
        /// </summary>
        public override void UpdateSettings()
        { 
            Config.NoOfEntries = int.Parse(NoOfEntries.Text);
            Config.RegisterJquery = RegisterJquery.Checked;
            Config.CleanerEnabled = EnableCleaner.Checked;
            Config.MaxEntries = int.Parse(MaxEntries.Text);
            Config.MaxAgeDays = int.Parse(MaxAgeDays.Text);
        }


    }
}