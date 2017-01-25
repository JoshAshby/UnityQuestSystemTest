using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public class Theme
    {
        public Texture2D CanvasBackground { get; }

        public string InputKnob { get; }
        public string OutputKnob { get; }
        public string BothKnob { get; }

        public string AddKnobName { get; }
        public string RemoveKnobName { get; }

        public Texture2D AddKnob { get; }
        public Texture2D RemoveKnob { get; }

        public Texture2D ResizeHandle { get; }

        public Texture2D Line { get; }

        private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private Dictionary<string, Color> _colorCache = new Dictionary<string, Color>();

        public Theme()
        {
            CanvasBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/Grid Background@2x.png");

            InputKnob = "Input Knob@2x";
            OutputKnob = "Output Knob@2x";
            BothKnob = "Both Knob@2x";

            AddKnobName = "Add Knob@2x";
            RemoveKnobName = "Remove Knob@2x";

            AddKnob = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/Add Knob@2x.png");
            RemoveKnob = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/Remove Knob@2x.png");

            ResizeHandle = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/Resize Handle@2x.png");

            Line = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/AA Line@2x.png");
        }

        public Texture2D GetTexture(string name, int ccrotation, Color color)
        {
            string key = name + ccrotation.ToString() + color.ToString();

            if (_textureCache.ContainsKey(key))
                return _textureCache[key];

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Systems/Ashode/Resources/Textures/" + name + ".png");

            if (ccrotation != 0)
                texture = RotateTexture(texture, ccrotation);

            if (color != null)
                texture = ColorTexture(texture, color);

            _textureCache.Add(key, texture);

            return texture;
        }

        public Color GetColor(string id)
        {
            if (_colorCache.ContainsKey(id))
                return _colorCache[id];

            int hash = id.GetHashCode();
            float r = ((hash & 0xFF0000) >> 16) / 100f;
            float g = ((hash & 0x00FF00) >> 8) / 100f;
            float b = (hash & 0x0000FF) / 100f;

            return new Color(r, g, b);
        }

        public Texture2D RotateTexture(Texture2D Texture, int QuarterCounterClockwise)
        {
            Texture2D rotatedTexture = UnityEngine.Object.Instantiate(Texture);

            int width = rotatedTexture.width;
            int height = rotatedTexture.height;

            Color[] columns = rotatedTexture.GetPixels();
            Color[] rotatedColumns = new Color[width * height];

            for (int i = 0; i < QuarterCounterClockwise; i++)
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        rotatedColumns[x * width + y] = columns[(width - y - 1) * width + x];

                rotatedColumns.CopyTo(columns, 0);
            }

            rotatedTexture.SetPixels(columns);
            rotatedTexture.Apply();

            return rotatedTexture;
        }

        public Texture2D ColorTexture(Texture2D Texture, Color Color)
        {
            Texture2D coloredTexture = UnityEngine.Object.Instantiate(Texture);

            int width = coloredTexture.width;
            int height = coloredTexture.height;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    coloredTexture.SetPixel(x, y, coloredTexture.GetPixel(x, y) * Color);

            coloredTexture.Apply();
            return coloredTexture;
        }
    }
}