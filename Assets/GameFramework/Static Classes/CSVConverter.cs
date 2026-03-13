using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GF
{
    public class CSVConverter
    {
        public void SaveToFile()
        {
            //for tempary we added comment later we will be replacing the list in foreach loop...
           /* foreach (var item in colourConfig.colors)
            {
                keyFrames.Add(new KeyFrame(counter + "", ToRGBHex(new Color(item.r, item.g, item.b, item.a))));
                counter++;
            }*/
            // Use the CSV generation from before
            var content = ToCSV();

            // The target file path e.g.
#if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
    var folder = Application.persistentDataPath;
#endif

            var filePath = Path.Combine(folder, "export.csv");

            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(content);
            }

            // Or just
            //File.WriteAllText(content);

            Debug.Log($"CSV file written to \"{filePath}\"");

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        private List<KeyFrame> keyFrames = new List<KeyFrame>(10000);
        public string ToCSV()
        {
            var sb = new StringBuilder("ID,Colour");
            foreach (var frame in keyFrames)
            {
                sb.Append('\n').Append(frame.ID).Append(',').Append(frame.color);
            }

            return sb.ToString();
        }
        private byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
        public string ToRGBHex(Color c)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }
    }
    [Serializable]
    public class KeyFrame
    {
        public string ID;
        public string color;

        public KeyFrame() { }

        public KeyFrame(string id, string _color)
        {
            ID = id;
            color = _color;
        }
    }
}
