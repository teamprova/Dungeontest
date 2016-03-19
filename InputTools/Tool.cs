using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeontest
{
    abstract class Tool
    {
        public static SpriteFont Font;
        public static Texture2D Background;

        public bool Clicked = false;
        public bool DoubleClicked = false;
        public bool Selected = false;
        public bool Hovering = false;

        DateTime lastClick = new DateTime(0);

        protected Rectangle backgroundSize;
        bool oldMouseDown = false;

        protected const int CharacterWidth = 21;
        protected const int CharacterHeight = 30;
        public Vector2 position;

        string text = "";
        
        protected float stringWidth = 0;

        public string Text
        {
            get { return text; }
            set {
                text = FormatString(value);
                stringWidth = Font.MeasureString(text).X;
            }
        }

        public virtual void Update(int offset)
        {
            Clicked = false;
            DoubleClicked = false;

            bool mouseDown = Input.leftClick;
            bool clickedSomewhere = mouseDown && !oldMouseDown;
            oldMouseDown = mouseDown;

            if (clickedSomewhere && Hovering)
            {
                Clicked = true;
                Selected = true;
                if (DateTime.Now.Subtract(lastClick).TotalMilliseconds < 500)
                    DoubleClicked = true;

                lastClick = DateTime.Now;
            }
            else if (clickedSomewhere && !Hovering)
                Selected = false;

            CheckHover(offset);
        }

        public abstract void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch);

        void CheckHover(int offset)
        {
            Rectangle MouseBounds = new Rectangle((int)Input.mouse.X, (int)Input.mouse.Y + offset, 1, 1);

            backgroundSize.Intersects(ref MouseBounds, out Hovering);
        }

        public static string FormatString(string text)
        {
            text = text.ToUpper();
            string formattedString = "";

            foreach (char c in text)
                if (Font.Characters.Contains(c))
                    formattedString += c;

            return formattedString;
        }

        public static void CreateBackground(GraphicsDevice GraphicsDevice)
        {
            Background = new Texture2D(GraphicsDevice, 1, 1);

            uint[] data = new uint[1] { 0xFFFFFFFF };
            Background.SetData<uint>(data);
        }
    }
}
