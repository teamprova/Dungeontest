using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DungeonTest
{
    class TextureData
    {
        public int width;
        public int height;
        public uint[] data;
        float[] zIndex;

        public TextureData(int width, int height)
        {
            this.width = width;
            this.height = height;

            Clear();
        }

        public TextureData(Texture2D texture)
        {
            LoadTexture(texture);
        }

        public TextureData(ContentManager Content, string src)
        {
            Texture2D texture = Content.Load<Texture2D>(src);

            LoadTexture(texture);
        }

        void LoadTexture(Texture2D texture)
        {
            width = texture.Width;
            height = texture.Height;

            Clear();
            texture.GetData(data);
        }

        // Resets data
        public void Clear()
        {
            data = new uint[width * height];
            zIndex = new float[width * height];
        }

        public static void Darken(ref Color color, float scale)
        {
            color = Color.Lerp(Color.Black, color, scale);
        }

        void ApplyAlpha(ref Color color, int i)
        {
            Color oldColor = new Color();
            oldColor.PackedValue = data[i];

            color = Color.Lerp(oldColor, color, color.A / 255f);
        }

        // Sets a color on a pixel
        public void SetPixel(int x, int y, float z, Color color)
        {
            int i = y * width + x;

            if (i < data.Length && i >= 0)
            {
                if (z < zIndex[i])
                {
                    ApplyAlpha(ref color, i);

                    data[i] = color.PackedValue;
                    zIndex[i] = z;
                }
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            int i = y * width + x;

            if (i < data.Length && i >= 0)
            {
                data[i] = color.PackedValue;
            }
        }

        public void SetZIndex(int x, int y, float z)
        {
            int i = y * width + x;

            if (i < data.Length && i >= 0)
                zIndex[i] = z;
        }

        // Returns the color on a pixel
        public Color GetPixel(int x, int y)
        {
            Color pixel = Color.Transparent;

            if (x < width && x >= 0 && y >= 0 && y < height)
                pixel.PackedValue = data[y * width + x];
            return pixel;
        }
    }
}
