using System;
using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

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
            Logger.log.Debug("OnApplicationStart");
        }

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnActiveSceneChanged(Scene prevScene, Scene newScene)
        {
            if (newScene.name == "MenuCore" && importDone == false)
            {
                // Get reference to PlayerDataModelSO
                PlayerDataModelSO[] playerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>();
                if (playerData == null || playerData.Length == 0)
                {
                    Logger.Log("Unable to get a handle on PlayerDataModelSO");
                    return;
                }
                
                // Get reference to ColorSchemesSettings in PlayerDataModelSO
                ColorSchemesSettings colorSchemes = playerData[0].playerData.colorSchemesSettings;
                /*
                for (int i = 0; i < colorSchemes.GetNumberOfColorSchemes(); i++)
                {
                    ColorScheme colorScheme = colorSchemes.GetColorSchemeForIdx(i);
                    Logger.Log("Color Scheme Idx: " + i + "; Color Scheme ID: " + colorScheme.colorSchemeId + "; Color Scheme Name: " + colorScheme.colorSchemeName);
                }
                */

                Util.CustomColorParser ccp = new Util.CustomColorParser();

                if (ccp.TryLoadCCConfig())
                {
                    // Save CustomColor settings to profile "Custom 3"
                    ColorScheme csu3 = colorSchemes.GetColorSchemeForId("User3");
                    csu3.SetPrivateField("_saberAColor", ccp.leftNoteColor);
                    csu3.SetPrivateField("_saberBColor", ccp.rightNoteColor);
                    csu3.SetPrivateField("_environmentColor0", ccp.leftLightColor);
                    csu3.SetPrivateField("_environmentColor1", ccp.rightLightColor);
                    csu3.SetPrivateField("_obstaclesColor", ccp.wallColor);
                }

                importDone = true;
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {

        }

        public void OnSceneUnloaded(Scene scene)
        {

        }
    }
}
