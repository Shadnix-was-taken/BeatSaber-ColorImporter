using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Settings;
using ColorImporter.Settings;
using IPA;
using IPA.Loader;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace ColorImporter
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static string PluginName => "ColorImporter";
        public static string PluginVersion { get; private set; } = "0";                             // Default. Actual version is retrieved from the manifest

        public static Color defaultRedNote = new Color(0.6588235f, 0.1254902f, 0.1254902f, 1f);     // saberA
        public static Color defaultBlueNote = new Color(0.1254902f, 0.3921569f, 0.6588235f, 1f);    // saberB
        public static Color defaultRedLight = new Color(0.7529412f, 0.1882353f, 0.1882353f, 1f);    // env0
        public static Color defaultBlueLight = new Color(0.1882353f, 0.5960785f, 1f, 1f);           // env1
        public static Color defaultWall = new Color(1f, 0.1882353f, 0.1882353f, 1f);                // obstacle

        public static bool importDone = false;
        public static Util.CustomColorParser ccp = null;

        public void Init(IPALogger logger, PluginMetadata metadata)
        {
            if (logger != null)
            {
                Logger.log = logger;
            }

            if (metadata != null)
            {
                PluginVersion = metadata.Version.ToString();
            }
        }

        [OnEnable]
        public void OnEnable() => Load();

        [OnDisable]
        public void OnDisable() => Unload();

        public void OnActiveSceneChanged(Scene prevScene, Scene newScene)
        {
            if (newScene.name == "MenuViewControllers")
            {
                if (!importDone)
                {
                    ccp = new Util.CustomColorParser();
                    ccp.TryLoadCCConfig();
                    importDone = true;
                }
                if (prevScene.name == "EmptyTransition")
                {
                    BSMLSettings.instance.AddSettingsMenu("Color Importer", "ColorImporter.Settings.ColorImporterUI.bsml", ColorImporterUI.instance);
                }
            }
        }

        private void Load()
        {
            AddEvents();
        }

        private void Unload()
        {
            RemoveEvents();
        }

        private void AddEvents()
        {
            RemoveEvents();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void RemoveEvents()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public static bool importColorScheme(string targetSchemeName)
        {
            string targetScheme = "";

            // Get reference to PlayerDataModelSO
            PlayerDataModel[] playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>();
            if (playerData == null || playerData.Length == 0)
            {
                Logger.Log("Unable to get a handle on PlayerDataModel");
                return false;
            }

            // Get reference to ColorSchemesSettings in PlayerDataModelSO
            ColorSchemesSettings colorSchemes = playerData[0].playerData.colorSchemesSettings;
            if (colorSchemes == null)
            {
                Logger.Log("Unable to get a handle on playerData.colorSchemesSettings");
                return false;
            }

            // Get SchemeID for selected Color Scheme Name
            for (int i = 0; i < colorSchemes.GetNumberOfColorSchemes(); i++)
            {
                ColorScheme colorScheme = colorSchemes.GetColorSchemeForIdx(i);
                if (colorScheme.colorSchemeName == targetSchemeName)
                {
                    targetScheme = colorScheme.colorSchemeId;
                }
            }
            if (targetScheme == "")
            {
                Logger.Log("Unable to find targetScheme for targetSchemeName '" + targetSchemeName + "'");
                return false;
            }

            // Get reference to selected target scheme
            ColorScheme csu = colorSchemes.GetColorSchemeForId(targetScheme);
            if (csu == null)
            {
                Logger.Log("Unable to get a handle on target ColorScheme '" + targetScheme + "'");
                return false;
            }

            // Save CustomColor settings to selected profile
            try
            {
                csu.SetField("_saberAColor", ccp.leftNoteColor);
                csu.SetField("_saberBColor", ccp.rightNoteColor);
                csu.SetField("_environmentColor0", ccp.leftLightColor);
                csu.SetField("_environmentColor1", ccp.rightLightColor);
                csu.SetField("_obstaclesColor", ccp.wallColor);
            }
            catch (Exception e)
            {
                Logger.Log("Setting data in user ColorScheme failed");
                Logger.Log(e.ToString());
                return false;
            }

            // Return true for successful import
            return true;
        }
    }
}
