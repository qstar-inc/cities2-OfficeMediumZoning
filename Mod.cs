using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;
using Colossal.PSI.Environment;
using Colossal.UI;
using Game.Modding;
using Game.SceneFlow;
using Game.UI.Menu;
using Game;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Entities;

namespace OfficeMediumZoning
{
    public class Mod : IMod
    {
        public const string ModName = "Medium Office Zoning";
        //public static ILog log = LogManager.GetLogger($"{nameof(OfficeMediumZoning)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static readonly string openModPage = "https://mods.paradoxplaza.com/mods/79067/Windows";
        public static OfficeMediumZoningSetting Setting;
        public static string uiHostName = "starq-medium-office";
        public static NotificationUISystem _notificationUISystem;
        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            //if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            //log.Info($"Current mod asset at {asset.path}");
            GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset);
            _notificationUISystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<NotificationUISystem>();
            UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "thumbs"), false);
            Setting = new OfficeMediumZoningSetting(this);
            Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Setting));
            AssetDatabase.global.LoadSettings(nameof(OfficeMediumZoning), Setting, new OfficeMediumZoningSetting(this));
            Setting.DummySetting = false;
        }

        public void OnCreate()
        {
            if (Setting.ProcessedThumbnails == false)
            {
                ProcessThumbnail();
            }
        }

        public static void ProcessThumbnail()
        {
            string modFolder = EnvPath.kCacheDataPath + "/Mods/mods_subscribed/";
            string ailCustomFolder = EnvPath.kUserDataPath + "/ModsData/AssetIconLibrary/CustomThumbnails";

            var thumbNotification = _notificationUISystem.AddOrUpdateNotification("starq-medium-office-thumbnail-process",
                title: "Processing Thumbnails (Mixed Residential Office Zoning)",
                text: "Starting system...",
                progressState: ProgressState.Indeterminate,
                progress: 0);
            if (Directory.GetDirectories(modFolder, "79634_*").Length > 0)
            {
                if (!CheckExistingFiles(ailCustomFolder, thumbNotification))
                {
                    FindAndCopyFiles(modFolder, ailCustomFolder, thumbNotification);
                }
            }
            else
            {
                thumbNotification.text = "Asset Icon Library not found, cancelling...";
                thumbNotification.progressState = ProgressState.Complete;
                thumbNotification.progress = (4 / 4 * 100);
            }
        }

        protected static bool CheckExistingFiles(string ailCustomFolder, NotificationUISystem.NotificationInfo thumbNotification = null)
        {
            Directory.CreateDirectory(ailCustomFolder);
            string[] existingFiles = Directory.GetFiles(ailCustomFolder, "*.png");
            int count = 0;

            if (existingFiles.Length > 0)
            {
                count = existingFiles.Count(file =>
                {
                    Match match = Regex.Match(Path.GetFileName(file), @"(([EUNA]{2})_MixedOffice([LeftRight\d]{2,7})_L([\d])_([\dx]{3}).png)");
                    return match.Success;
                });
            }

            if (thumbNotification != null && count == 330)
            {
                thumbNotification.text = "Thumbnails already exists; cancelling...";
                thumbNotification.progressState = ProgressState.Complete;
                thumbNotification.progress = (4 / 4 * 100);
                _notificationUISystem.RemoveNotification("starq-medium-office-thumbnail-process");
            }
            return count == 330;

        }

        protected static void FindAndCopyFiles(string modFolder, string ailCustomFolder, NotificationUISystem.NotificationInfo thumbNotification)
        {
            string[] directories = Directory.GetDirectories(modFolder, "79634_*");

            if (directories.Length > 0)
            {
                thumbNotification.text = "Asset Icon Library found...";
                thumbNotification.progressState = ProgressState.Progressing;
                thumbNotification.progress = (2 / 4 * 100);
            }
            foreach (string directory in directories)
            {

                string thumbdirectory = directory + "/Thumbnails";
                string[] files = Directory.GetFiles(thumbdirectory, "*.png");
                if (files.Length > 0)
                {
                    thumbNotification.text = $"Configuring thumbnails...";
                    thumbNotification.progressState = ProgressState.Progressing;
                    thumbNotification.progress = (3 / 4 * 100);
                }
                foreach (string file in files)
                {
                    Match match = Regex.Match(Path.GetFileName(file), @"(([EUNA]{2})_CommercialHigh([\d]{2})_L([\d])_([\dx]{3})\.png)");


                    if (match.Success)
                    {
                        string newFileName = match.Groups[2].Value + "_CommercialHigh" + match.Groups[3].Value + "_L" + match.Groups[4].Value + "_" + match.Groups[5].Value + ".png";
                        string ailCustomFolderFilePath = Path.Combine(ailCustomFolder, newFileName);
                        File.Copy(file, ailCustomFolderFilePath, true);
                    }
                }
            }

            if (CheckExistingFiles(ailCustomFolder))
            {
                thumbNotification.text = "Thumbnail processing completed...";
                thumbNotification.progressState = ProgressState.Complete;
                thumbNotification.progress = (4 / 4 * 100);
                Setting.ProcessedThumbnails = true;
                _notificationUISystem.RemoveNotification("starq-medium-office-thumbnail-process");
            }
        }

        public void OnDispose()
        {
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
            if (Setting != null)
            {
                Setting.UnregisterInOptionsUI();
                Setting = null;
            }
        }
    }

}
