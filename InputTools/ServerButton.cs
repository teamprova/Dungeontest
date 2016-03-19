using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeontest
{
    class ServerButton : Tool
    {
        static Vector2 Padding = new Vector2(8, 10);
        public static Color Color = Color.Black;
        public static Color PressedColor = Color.FromNonPremultiplied(10, 10, 10, 255);

        public const int WIDTH = 540;
        public const int HEIGHT = 85;

        public string Name = "";
        public string IP = "";

        public ServerButton(string name, string ip, int y)
        {
            Name = name;
            IP = ip;

            position = new Vector2(10, y);
            backgroundSize = new Rectangle(10, y, WIDTH, HEIGHT);
        }

        public override void Update(int offset)
        {
            base.Update(offset);
        }

        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, backgroundSize, (Hovering || Selected) ? PressedColor : Color);

            //server info
            spriteBatch.DrawString(Font, Name, position + Padding, Color.White);
            spriteBatch.DrawString(Font, IP, position + Padding + new Vector2(0, CharacterHeight + 5), Color.White);
        }
    }
}
