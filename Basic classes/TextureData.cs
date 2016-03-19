using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeontest
{
    public class TextureData
    {
        public int width = 1;
        public int height = 1;
        public uint[] data;
        float[] zIndex;

        public TextureData(int width, int height)
        {
            this.width = width;
            this.height = height;

            Clear();
        }

        public TextureData(byte[] bytes)
        {
            CopyBytes(bytes);
        }

        public TextureData(string src)
        {
            try
            {
                System.Drawing.Bitmap texture = new System.Drawing.Bitmap(src);

                width = texture.Width;
                height = texture.Height;

                Clear();

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        Color xnaColor = new Color();
                        System.Drawing.Color formsColor = texture.GetPixel(x, y);

                        xnaColor.R = formsColor.R;
                        xnaColor.G = formsColor.G;
                        xnaColor.B = formsColor.B;
                        xnaColor.A = formsColor.A;

                        data[y * width + x] = xnaColor.PackedValue;
                    }
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to load file: " + e.Message);
                Clear();
            }
        }

        #region Tools
        public static void Darken(ref Color color, float scale)
        {
            color = Color.Lerp(Color.Black, color, scale);
        }

        // Resets data
        public void Clear()
        {
            data = new uint[width * height];
            zIndex = new float[width * height];
        }
        #endregion

        #region ModifyingData
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

        void ApplyAlpha(ref Color color, int i)
        {
            Color oldColor = new Color();
            oldColor.PackedValue = data[i];

            color = Color.Lerp(oldColor, color, color.A / 255f);
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
        #endregion

        #region Multiplayer
        public byte[] GetBytes()
        {
            // first four bytes are the size of the message and the next 8 define the size
            byte[] bytes = new byte[8 + data.Length * 4];

            BitConverter.GetBytes(width).CopyTo(bytes, 0);
            BitConverter.GetBytes(height).CopyTo(bytes, 4);

            for (int i = 0; i < data.Length; i++)
                BitConverter.GetBytes(data[i]).CopyTo(bytes, i * 4 + 8);

            return bytes;
        }

        public void CopyBytes(byte[] bytes)
        {
            width = BitConverter.ToInt32(bytes, 0);
            height = BitConverter.ToInt32(bytes, 4);

            Clear();
            
            for (int i = 0; i < data.Length; i++)
                data[i] = BitConverter.ToUInt32(bytes, i * 4 + 8);
        }
        #endregion
    }
}
