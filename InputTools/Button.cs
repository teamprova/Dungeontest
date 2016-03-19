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
    class Button : Tool
    {
        static Vector2 Padding = new Vector2(8, 2);
        public static Color Color = Color.Black;
        public static Color HoverColor = Color.White;

        public Button(string text, int x, int y)
        {
            Text = text;
            Vector2 textSize = Font.MeasureString(Text);

            int width = (int) textSize.X;
            int height = (int) textSize.Y;

            position = new Vector2(x - width / 2, y - height / 2);
            backgroundSize = new Rectangle((int)position.X, (int)position.Y, width + (int)Padding.X * 2, height + (int)Padding.Y * 2);
        }

        public override void Update(int offset)
        {
            base.Update(offset);
        }

        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, backgroundSize, null, (Hovering) ? HoverColor : Color);

            spriteBatch.DrawString(Font, Text, position + Padding, Color.White);
        }
    }
}
