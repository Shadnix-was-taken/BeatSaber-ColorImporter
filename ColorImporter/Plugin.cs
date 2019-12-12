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
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginName => "ColorImporter";
        public static string PluginVersion { get; private set; } = "0"; // Default. Actual version is retrieved from the manifest

        public static Color defaultRedNote = new Color(0.6588235f, 0.1254902f, 0.1254902f, 1f);     // saberA
        public static Color defaultBlueNote = new Color(0.1254902f, 0.3921569f, 0.6588235f, 1f);    // saberB
        public static Color defaultRedLight = new Color(0.7529412f, 0.1882353f, 0.1882353f, 1f);    // env0
        public static Color defaultBlueLight = new Color(0.1882353f, 0.5960785f, 1f, 1f);           // env1
        public static Color defaultWall = new Color(1f, 0.1882353f, 0.1882353f, 1f);                // obstacle

        public static bool importDone = false;
        public static Util.CustomColorParser ccp = null;

        public void Init(IPALogger logger, PluginLoader.PluginMetadata metadata)
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

        public void OnApplicationStart()
        {

        }

        public void OnApplicationQuit()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {

        }

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
                BSMLSettings.instance.AddSettingsMenu("Color Importer", "ColorImporter.Settings.ColorImporterUI.bsml", ColorImporterUI.instance);
                PersistentSingleton<ColorImporterUI>.instance.Initialize();
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {

        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        public static bool importColorScheme(string targetSchemeName)
        {
            string targetScheme = "";

            // Get reference to PlayerDataModelSO
            PlayerDataModelSO[] playerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>();
            if (playerData == null || playerData.Length == 0)
            {
                Logger.Log("Unable to get a handle on PlayerDataModelSO");
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
                csu.SetPrivateField("_saberAColor", ccp.leftNoteColor);
                csu.SetPrivateField("_saberBColor", ccp.rightNoteColor);
                csu.SetPrivateField("_environmentColor0", ccp.leftLightColor);
                csu.SetPrivateField("_environmentColor1", ccp.rightLightColor);
                csu.SetPrivateField("_obstaclesColor", ccp.wallColor);
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
