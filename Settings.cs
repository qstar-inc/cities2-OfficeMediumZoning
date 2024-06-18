using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;

namespace OfficeMediumZoning
{
    [FileLocation("ModsSettings\\" + nameof(OfficeMediumZoning))]
    public class OfficeMediumZoningSetting : ModSetting
    {
        public const string sectionMain = "Main";
        public const string actions = "Actions";

        public OfficeMediumZoningSetting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUIHidden]
        public bool ProcessedThumbnails { get; set; }

        [SettingsUIHidden]
        public bool HasReadChangeNotice { get; set; }

        [SettingsUIHidden]
        public bool DummySetting { get; set; }


        [SettingsUIButton]
        [SettingsUISection(sectionMain, actions)]
        public bool RedoThumbnail
        {
            set { Mod.ProcessThumbnail(); }
        }

        public override void SetDefaults()
        {
            DummySetting = true;
            HasReadChangeNotice = false;
            ProcessedThumbnails = false;
        }

        public override void Apply()
        {
            base.Apply();
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly OfficeMediumZoningSetting m_Setting;
        public LocaleEN(OfficeMediumZoningSetting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Office Medium Zoning" },
                { m_Setting.GetOptionTabLocaleID(OfficeMediumZoningSetting.sectionMain), "Main" },
                { m_Setting.GetOptionGroupLocaleID(OfficeMediumZoningSetting.actions), "Actions" },

                { m_Setting.GetOptionLabelLocaleID(nameof(OfficeMediumZoningSetting.RedoThumbnail)), "Reset Thumbnail Cache [powered by Asset Icon Library]" },
                { m_Setting.GetOptionDescLocaleID(nameof(OfficeMediumZoningSetting.RedoThumbnail)), $"This button will redo the thumbnails from the Asset Icon Library to work with this mod. Make sure you're already subscribed to it to have the thumbnails." },

            };
        }

        public void Unload()
        {

        }
    }
}
