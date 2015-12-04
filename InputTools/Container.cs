using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Container : Tool
    {
        static Vector2 Padding = new Vector2(8, 2);
        public Color Color = Color.White;

        Viewport viewport;

        public List<Tool> Controls = new List<Tool>();
        int Scroll = 0;

        int width;
        int height;

        public int Width
        {
            get { return width; }
            set {
                width = value;
                viewport.Width = width;
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                viewport.Height = height;
            }
        }

        public Container(int x, int y, int w, int h)
        {
            width = w;
            height = h;

            position = new Vector2(x, y);
            backgroundSize = new Rectangle(x, y, width, height);
            viewport = new Viewport(backgroundSize);
        }

        public override void Update(int offset)
        {
            base.Update(offset);

            if (Hovering)
            {
                Scroll -= Input.deltaWheel * 5;

                float maxScroll = ((ServerButton.HEIGHT + Padding.Y + 10) * Controls.Count) - backgroundSize.Height;

                Scroll = (int)MathHelper.Clamp(Scroll, 0, maxScroll);
            }

            offset = (int)-position.Y + Scroll;

            foreach (Tool tool in Controls)
                tool.Update(offset);
        }

        public void AddControl(Tool tool)
        {
            Controls.Add(tool);
        }

        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            Viewport mainViewport = GraphicsDevice.Viewport;
            Viewport currentViewport = new Viewport(viewport.Bounds);
            
            currentViewport.X = (int)(currentViewport.X / Screen.scale.X);
            currentViewport.Y = (int)(currentViewport.Y / Screen.scale.Y);
            currentViewport.Width = (int)(currentViewport.Width / Screen.scale.X);
            currentViewport.Height = (int)(currentViewport.Height / Screen.scale.Y);

            GraphicsDevice.Viewport = currentViewport;
            
            
            Matrix CameraMatrix = Screen.GetTransformMatrix() * Matrix.CreateTranslation(0, -Scroll / Screen.scale.Y, 0);

            //scroll
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, CameraMatrix);

            //draw background
            spriteBatch.Draw(Background, new Rectangle(0, Scroll, viewport.Width, viewport.Height), Color);

            //draw each tool
            foreach (Tool tool in Controls)
                tool.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.End();

            GraphicsDevice.Viewport = mainViewport;
        }
    }
}
