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
    class TextBox : Tool
    {
        static Vector2 Padding = new Vector2(8, 2);
        static string[] symbols = new string[]{
            ")",
            "!",
            "@",
            "#",
            "$",
            "%",
            "^",
            "&",
            "*",
            "("
        };

        string placeholder = "";
        int limit;

        public TextBox(string Placeholder, int Limit, int x, int y)
        {
            placeholder = FormatString(Placeholder);

            position = new Vector2(x, y);
            limit = Limit;
            backgroundSize = new Rectangle(x, y, (int)CharacterWidth * limit + (int)Padding.X * 2, CharacterHeight + (int)Padding.Y * 2);
        }

        public override void Update(int offset)
        {
            base.Update(offset);

            if (Selected)
                checkKeys();
        }

        void checkKeys()
        {
            if (Input.Held(Keys.LeftControl))
            {
                if (Input.Tapped(Keys.V))
                {//paste
                    string paste = System.Windows.Forms.Clipboard.GetText();

                    paste = paste.Replace("\n", "");
                    paste = paste.Replace("\r", "");

                    Text += paste;
                }
                return;
            }

            foreach (Keys k in Input.GetPressedKeys())
                if (Input.Tapped(k))
                {
                    string key = k.ToString();

                    switch(key)
                    {
                        case "Back":
                            if (Text.Length > 0)
                                Text = Text.Remove(Text.Length - 1);
                            break;
                        default:
                            if (key.Contains("NumPad"))
                                Text += key.Remove(0, 6);
                            else if (key.Contains("OemPeriod"))
                                Text += ".";
                            else if (key.Contains("D") && key.Length == 2)
                            {
                                key = key.Remove(0, 1);

                                if (Input.Held(Keys.LeftShift))
                                    key = symbols[Int16.Parse(key)];

                                Text += key;
                            }
                            else if (key.Length > 1)
                                break;
                            else
                                if (Input.Held(Keys.LeftShift) || Input.Held(Keys.CapsLock))
                                    Text += key;
                                else
                                    Text += key.ToLower();
                            break;
                    }
                }

            if (Text.Length > limit)
                Text = Text.Remove(limit);
        }

        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, backgroundSize, Color.Black);

            if (Text.Length == 0 && !Selected)
                spriteBatch.DrawString(Font, placeholder, position + Padding, Color.Gray);
            else
                spriteBatch.DrawString(Font, Text, position + Padding, Color.White);
        }
    }
}
