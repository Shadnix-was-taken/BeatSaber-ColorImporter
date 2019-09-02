using BS_Utils.Utilities;
using UnityEngine;


namespace ColorImporter.Util
{
    public class CustomColorParser
    {
        public Config ccConfig = new Config("CustomColors");

        public bool loadSuccessful = false;

        private int leftNoteColorPreset = 0;
        private int rightNoteColorPreset = 0;

        private int leftLightPreset = 0;
        private int rightLightPreset = 0;

        private int wallColorPreset = 0;

        private int leftUserR = 0;
        private int leftUserG = 0;
        private int leftUserB = 0;
        private Color leftUserColor;

        private int rightUserR = 0;
        private int rightUserG = 0;
        private int rightUserB = 0;
        private Color rightUserColor;

        public Color leftNoteColor;
        public Color rightNoteColor;

        public Color leftLightColor;
        public Color rightLightColor;

        public Color wallColor;

        public bool TryLoadCCConfig()
        {
            // Basic check to see if there actually is a CustomColors file
            if (!DoesCCConfigExist())
                return false;
            
            // Load values and create user colors
            leftNoteColorPreset = ccConfig.GetInt("Presets", "leftNoteColorPreset");
            rightNoteColorPreset = ccConfig.GetInt("Presets", "rightNoteColorPreset");

            leftLightPreset = ccConfig.GetInt("Presets", "leftLightPreset");
            rightLightPreset = ccConfig.GetInt("Presets", "rightLightPreset");

            wallColorPreset = ccConfig.GetInt("Presets", "wallColorPreset");

            leftUserR = ccConfig.GetInt("User Preset Colors", "Left User Preset R");
            leftUserG = ccConfig.GetInt("User Preset Colors", "Left User Preset G");
            leftUserB = ccConfig.GetInt("User Preset Colors", "Left User Preset B");
            leftUserColor = new Color(leftUserR / 255f, leftUserG / 255f, leftUserB / 255f);

            rightUserR = ccConfig.GetInt("User Preset Colors", "Right User Preset R");
            rightUserG = ccConfig.GetInt("User Preset Colors", "Right User Preset G");
            rightUserB = ccConfig.GetInt("User Preset Colors", "Right User Preset B");
            rightUserColor = new Color(rightUserR / 255f, rightUserG / 255f, rightUserB / 255f);

            // Set colors for BS color scheme
            // set note colors
            if (leftNoteColorPreset == 0)
                leftNoteColor = leftUserColor;
            else
                leftNoteColor = GetPresetColor(leftNoteColorPreset);

            if (rightNoteColorPreset == 0)
                rightNoteColor = rightUserColor;
            else
                rightNoteColor = GetPresetColor(rightNoteColorPreset);

            // set light colors as Custom Colors did
            switch (leftLightPreset)
            {
                case 0:
                    leftLightColor = new Color(1, 4 / 255f, 4 / 255f);
                    break;
                case 1:
                    leftLightColor = leftNoteColor;
                    if (leftNoteColorPreset != 1 && leftNoteColorPreset != 2)
                        leftLightColor *= .8f;
                    break;
                case 2:
                    leftLightColor = rightNoteColor;
                    if (rightNoteColorPreset != 1 && rightNoteColorPreset != 2)
                        leftLightColor *= .8f;
                    break;
                case 3:
                    leftLightColor = leftUserColor;
                    leftLightColor *= .8f;
                    break;
                case 4:
                    leftLightColor = rightUserColor;
                    leftLightColor *= .8f;
                    break;
                default:
                    leftLightColor = GetPresetColor(leftLightPreset - 2);
                    leftLightColor *= .8f;
                    break;
            }

            switch (rightLightPreset)
            {
                case 0:
                    rightLightColor = new Color(0, 192 / 255f, 1);
                    break;
                case 1:
                    rightLightColor = leftNoteColor;
                    if (leftNoteColorPreset != 1 && leftNoteColorPreset != 2)
                        rightLightColor *= .8f;
                    break;
                case 2:
                    rightLightColor = rightNoteColor;
                    if (rightNoteColorPreset != 1 && rightNoteColorPreset != 2)
                        rightLightColor *= .8f;
                    break;
                case 3:
                    rightLightColor = leftUserColor;
                    rightLightColor *= .8f;
                    break;
                case 4:
                    rightLightColor = rightUserColor;
                    rightLightColor *= .8f;
                    break;
                default:
                    rightLightColor = GetPresetColor(rightLightPreset - 2);
                    rightLightColor *= .8f;
                    break;
            }

            switch (wallColorPreset)
            {
                case 0:
                    wallColor = Plugin.defaultWall;
                    break;
                case 1:
                    wallColor = leftNoteColor;
                    break;
                case 2:
                    wallColor = rightNoteColor;
                    break;
                case 3:
                    wallColor = leftUserColor;
                    break;
                case 4:
                    wallColor = rightUserColor;
                    break;
                default:
                    wallColor = GetPresetColor(wallColorPreset - 2);
                    break;
            }


            loadSuccessful = true;
            return true;
        }

        private bool DoesCCConfigExist()
        {
            if (ccConfig.HasKey("Core", "disablePlugin"))
                return true;
            else
                return false;
        }

        private Color GetPresetColor(int ccPreset)
        {
            switch (ccPreset)
            {
                case 1:
                    return new Color(1, 0.0156862745f, 0.0156862745f);              // Default Red
                case 2:
                    return new Color(0.00001413f, .706f, 1);                        // Default Blue
                case 3:
                    return new Color(0, .98f, 2.157f);                              // Electric Blue
                case 4:
                    return new Color(0.00001413f, 1, 0);                            // Green
                case 5:
                    return new Color(1.05f, 0, 2.188f);                             // Purple
                case 6:
                    return new Color(2.157f, .588f, 0);                             // Orange
                case 7:
                    return new Color(2.157f, 1.76f, 0);                             // Yellow
                case 8:
                    return new Color(1, 1, 1);                                      // White
                case 9:
                    return new Color(.3f, .3f, .3f);                                // Black
                case 10:
                    return new Color(1.000f, 0.396f, 0.243f);                       // OST Orange
                case 11:
                    return new Color(0.761f, 0.125f, 0.867f);                       // OST Purple
                case 12:
                    return new Color(0.349f, 0.69f, 0.957f);                        // Klouder Blue
                case 13:
                    return new Color(0.0352941176f, 0.929411765f, 0.764705882f);    // Miku
                case 14:
                    return new Color(0f, 0.28000000000000003f, 0.55000000000000004f); // Dark Blue
                case 15:
                    return new Color(1f, 0.388f, .7724f);                           // Pink
                default:
                    return Color.black;
            }
        }
    }
}
