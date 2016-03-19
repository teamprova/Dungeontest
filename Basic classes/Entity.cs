using System;
using Microsoft.Xna.Framework;

namespace Dungeontest
{
    public class Entity
    {
        public Vector2 pos = Vector2.Zero;
        public double angle = 0;
        public int id = 0;
        public float luminosity = 1f;
        public float idleTime = 0;

        public Entity(int ID, float x, float y)
        {
            pos = new Vector2(x, y);
            id = ID;
        }

        public void MoveTo(float x, float y)
        {
            pos = new Vector2(x, y);
        }

        /// <summary>
        /// Makes this entity chase another (e)
        /// </summary>
        /// <param name="e">Who to chase</param>
        /// <param name="speed">How fast the entity should move</param>
        public void Chase(Entity e, float speed)
        {
            /* commented since we don't actually use vel
            // Relative velocity (closing vel)
            vel = e.vel - vel;

            // Relative distance (range to close)
            Vector2 distance = e.pos - pos;

            // Time it will take to travel the relative distance at a speed equal to the closing speed
            Vector2 time = distance / vel;

            // Goal for the enemy
            Vector2 goal = e.pos + (e.vel * time);

            // Approach the goal
            pos += goal * Dungeon.deltaTime;
            */
            
            Vector2 direction = e.pos - pos;
            direction.Normalize();

            pos += direction * speed * Dungeon.deltaTime;
        }

        /// <summary>
        /// Makes this entity evade another (e)
        /// </summary>
        /// <param name="e">Who to evade</param>
        /// <param name="speed">How fast the entity should move</param>
        public void Evade(Entity e, float speed)
        {
            Vector2 direction = pos - e.pos;
            direction.Normalize();

            pos += direction * speed * Dungeon.deltaTime;
        }

        public virtual void Draw(TextureData ctx, Entity player, int headBob)
        {
            // sprite doesnt exist for this entity
            if (CoreGame.sprites.Count <= id)
                return;

            TextureData spriteData = CoreGame.sprites[id];

            // Translate position to viewer space
            Vector2 dist = pos - player.pos;

            float distance = dist.Length();

            // Sprite angle relative to viewing angle
            double spriteAngle = Math.Atan2(dist.Y, dist.X) - player.angle;

            // Size of the sprite
            int size = (int)(CoreGame.viewDist / (Math.Cos(spriteAngle) * distance));

            // X-position on screen
            int left = (int)(Math.Tan(spriteAngle) * CoreGame.viewDist);
            left = (CoreGame.GAME_WIDTH / 2 + left - size / 2);

            int top = (CoreGame.GAME_HEIGHT - size) / 2 + headBob;

            for(int x = (left < 0) ? -left : 0; x < size; x++)
            {
                if (x + left >= CoreGame.GAME_WIDTH)
                    break;

                for (int y = (top < 0) ? -top : 0; y < size; y++)
                {
                    if (y + top >= CoreGame.GAME_HEIGHT)
                        break;

                    int pixelX = x * spriteData.width / size;
                    int pixelY = y * spriteData.height / size;

                    Color pixel = spriteData.GetPixel(pixelX, pixelY);

                    // draw if the alpha is greater than zero
                    if(pixel.A > 0)
                        ctx.SetPixel(x + left, y + top, distance, pixel);
                }
            }
        }

        // Multiplayer stuff
        public const int BYTES = 12;

        public byte[] GetSendableFormat()
        {
            byte[] data = new byte[BYTES];

            BitConverter.GetBytes(id).CopyTo(data, 0);

            BitConverter.GetBytes(pos.X).CopyTo(data, 4);
            BitConverter.GetBytes(pos.Y).CopyTo(data, 8);

            return data;
        }

        public void CopyBytes(byte[] response)
        {
            id = BitConverter.ToInt32(response, 0);

            pos.X = BitConverter.ToSingle(response, 4);
            pos.Y = BitConverter.ToSingle(response, 8);
        }
    }
}
