using UnityEngine;
using System.Collections.Generic;

public enum DebugColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Magenta,
    Gray,
    Grey,
    White,
    Black,
    Orange,
    Pink,
    Brown,
    Gold,
    Silver,
    Violet,
    Indigo,
    Teal,
    Lime,
    Turquoise,
    Salmon,
    Chocolate,
    Coral,
    Orchid
}

namespace DebugUtils
{

    public static class DevDebug
    {
        // Predefined colors
        private static readonly Dictionary<DebugColor, Color> AllColors = new Dictionary<DebugColor, Color>()
        {
            {DebugColor.Red, Color.red},
            {DebugColor.Green, Color.green},
            {DebugColor.Blue, Color.blue},
            {DebugColor.Yellow, Color.yellow},
            {DebugColor.Cyan, Color.cyan},
            {DebugColor.Magenta, Color.magenta},
            {DebugColor.Gray, Color.gray},
            {DebugColor.Grey, Color.grey},
            {DebugColor.White, Color.white},
            {DebugColor.Black, Color.black},
            {DebugColor.Orange, new Color(1f, 0.65f, 0f)},
            {DebugColor.Pink, new Color(1f, 0.41f, 0.71f)},
            {DebugColor.Brown, new Color(0.59f, 0.29f, 0f)},
            {DebugColor.Gold, new Color(1f, 0.84f, 0f)},
            {DebugColor.Silver, new Color(0.75f, 0.75f, 0.75f)},
            {DebugColor.Violet, new Color(0.93f, 0.51f, 0.93f)},
            {DebugColor.Indigo, new Color(0.29f, 0f, 0.51f)},
            {DebugColor.Teal, new Color(0f, 0.51f, 0.51f)},
            {DebugColor.Lime, new Color(0.75f, 1f, 0f)},
            {DebugColor.Turquoise, new Color(0.25f, 0.88f, 0.82f)},
            {DebugColor.Salmon, new Color(0.98f, 0.5f, 0.45f)},
            {DebugColor.Chocolate, new Color(0.82f, 0.41f, 0.12f)},
            {DebugColor.Coral, new Color(1f, 0.5f, 0.31f)},
            {DebugColor.Orchid, new Color(0.85f, 0.44f, 0.84f)}
        };


        /// <summary>
        /// Logs a message using a color from the AllColors dictionary by enum.
        /// </summary>
        public static void Log(string message, DebugColor debugColor)
        {
            if (AllColors.TryGetValue(debugColor, out Color color))
            {
                string hex = ColorUtility.ToHtmlStringRGBA(color);
                Debug.Log($"<color=#{hex}>{message}</color>");
            }
            else
            {
                Debug.LogWarning($"Color '{debugColor}' not found in AllColors dictionary. Logging message in default color.");
                Debug.Log(message);
            }
        }

    }
}
