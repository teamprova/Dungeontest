using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Entity
    {
        public Vector2 pos = Vector2.Zero;
        public Vector2 vel = Vector2.Zero;
        public double angle = 0;
        public int type = 0;
        public float luminosity = 1;

        public TextureData spriteData;

        public Entity(TextureData sprite, float x, float y, int id)
        {
            pos = new Vector2(x, y);

            type = id;

            spriteData = sprite;
        }

        public virtual void Update(Player player, float deltaTime){}

        public virtual void AI(int action, Player player, float speed, float deltaTime)
        {
            // Create a movement Vector2
            Vector2 movement = vel * deltaTime;

            // Find the direction of the player
            Vector2 direction = player.pos - pos;
            direction.Normalize();


            // Detect what kind of AI we are shooting for
            if (action == 0) // Chasing an entity
            {
                // Relative velocity (closing vel)
                vel = player.vel - vel;

                // Relative distance (range to close)
                Vector2 distance = player.pos - pos;

                // Time it will take to travel the relative distance at a speed equal to the closing speed
                Vector2 time = distance / vel;

                // Goal for the enemy
                Vector2 goal = player.pos + (player.vel * time);

                // Approach the goal
                pos.X += goal.X;
                pos.Y += goal.Y;

                // Move to the player (old code in case i fuck up)
                //entityName.pos.X += vel.X * deltaTime;
                //entityName.pos.Y += vel.Y * deltaTime;

            }

            if (action == 1) // Evading an entity
            {
                // Set up velocity for movement
                vel = direction * speed;

                // Move away from the player
                pos.X -= direction.X * deltaTime;
                pos.Y -= direction.Y * deltaTime;
            }
        }

        public virtual void Draw(TextureData ctx, Player player)
        {
            if (spriteData == null)
                return;

            // Translate position to viewer space
            Vector2 dist = Vector2.Zero;
            dist.X = pos.X - player.pos.X;
            dist.Y = pos.Y - player.pos.Y;

            // Sprite angle relative to viewing angle
            double spriteAngle = Math.Atan2(dist.Y, dist.X) - player.angle;

            // Size of the sprite
            int size = (int)(CoreGame.GetViewDist() / (Math.Cos(spriteAngle) * dist.Length()));

            // X-position on screen
            float left = (float)(Math.Tan(spriteAngle) * CoreGame.GetViewDist());
            left = (CoreGame.GAME_WIDTH / 2 + left - size / 2);

            float top = (CoreGame.GAME_HEIGHT - size) / 2 + player.headBob;

            float zIndex = dist.Length();

            for(int x = (left < 0) ? (int)-left : 0; x < size; x++)
            {
                if (x + left > CoreGame.GAME_WIDTH)
                    break;

                for (int y = (top < 0) ? (int)-top : 0; y < size; y++)
                {
                    if (y + top > CoreGame.GAME_HEIGHT)
                        break;

                    int pixelX = x * (spriteData.width) / size;
                    int pixelY = y * (spriteData.height) / size;

                    Color pixel = spriteData.GetPixel(pixelX, pixelY);

                    if(pixel != Color.Transparent)
                        ctx.SetPixel(x + (int)left, y + (int)top, zIndex, pixel);
                }
            }
        }

        public const int BYTES = 20;

        public byte[] GetSendableFormat()
        {
            byte[] data = new byte[BYTES];

            BitConverter.GetBytes(pos.X).CopyTo(data, 0);
            BitConverter.GetBytes(pos.Y).CopyTo(data, 4);

            BitConverter.GetBytes(vel.X).CopyTo(data, 8);
            BitConverter.GetBytes(vel.Y).CopyTo(data, 12);

            BitConverter.GetBytes(type).CopyTo(data, 16);

            return data;
        }

        public void CopyBytes(byte[] response)
        {
            pos.X = BitConverter.ToSingle(response, 0);
            pos.Y = BitConverter.ToSingle(response, 4);

            vel.X = BitConverter.ToSingle(response, 8);
            vel.Y = BitConverter.ToSingle(response, 12);
        }
    }
}
