using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeontest
{
    abstract class Screen
    {
        public static Point resolution = new Point(600, 400);
        public static Rectangle window = new Rectangle(0, 0, resolution.X, resolution.Y);
        public static Vector3 scale = new Vector3(1, 1, 1);

        public bool RequestExit = false;

        public abstract Screen Update(float deltaTime);
        public virtual void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
        }

        public void BeginSpriteBatch(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, GetTransformMatrix());
        }

        public static Matrix GetTransformMatrix()
        {
            return Matrix.CreateScale(1 / scale.X, 1 / scale.Y, 1 / scale.Z);
        }
    }
}

