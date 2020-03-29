using BeatSaberMarkupLanguage.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ColorImporter.Settings
{
    public class ColorImporterUI : PersistentSingleton<ColorImporterUI>
    {
        public string schemeChoice = "Custom 3";

        [UIComponent("txt_usage")]
        private TextMeshProUGUI txt_usage;

        [UIComponent("txt_successInfo")]
        private TextMeshProUGUI txt_successInfo;

        [UIValue("ddl_schemeList")]
        private List<object> colorSchemeList = new List<object>()
        {
            "Custom 0",
            "Custom 1",
            "Custom 2",
            "Custom 3"
        };

        [UIValue("ddl_schemeChoice")]
        public string _schemeChoice
        {
            get => schemeChoice;
            set => schemeChoice = value;
        }

        [UIAction("import")]
        private void OnImport()
        {
            bool importSuccess = Plugin.importColorScheme(schemeChoice);
            if (importSuccess)
            {
                txt_successInfo.text = "Import to " + schemeChoice + " successful!";
            }
            else
            {
                txt_successInfo.text = "Import to " + schemeChoice + " failed. Please submit a bug report.";
            }
        }

        [UIAction("#apply")]
        private void OnApply()
        {
            txt_successInfo.text = " ";
        }

        [UIAction("#cancel")]
        private void OnCancel()
        {
            txt_successInfo.text = " ";
        }


        public void Awake()
        {
            StartCoroutine(Initialize());
        }

        // Initialize text based on the load result of CustomColors.ini
        private IEnumerator Initialize()
        {
            yield return new WaitForSeconds(2);
            if (Plugin.ccp.loadSuccessful)
            {
                txt_usage.text = "CustomColors.ini sucessfully loaded. Please choose the target color scheme and click \"Import\" to import the settings.\n<color=\"red\">Note: Existing settings will be overwritten.</color>";
            }
            else
            {
                txt_usage.text = "<color=#fb484e>Failed to load CustomColors.ini - Unable to import settings.</color>";
            }
            txt_successInfo.text = " ";
        }
    }
}
