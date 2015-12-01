using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Enemy : Entity
    {
        public static TextureData sprite;
        static Random rnd = new Random();
        float enemySpeed = 1f;

        public Enemy(float x, float y)
            : base(sprite, x, y, 2)
        {
            vel = new Vector2(1, 0);
            //vel = Vector2.Transform(vel, Matrix.CreateRotationZ((float)(rnd.NextDouble() * Math.PI * 2)));
        }


        public override void Update(Player player, float deltaTime)
        {

            // Setup a movement variables, to later add to the position
            Vector2 movement = vel * deltaTime;

            // Get the direction to travel to, and Normalize it.
            Vector2 direction = player.pos - pos;
            direction.Normalize();

            // Setup velocity
            vel = direction * enemySpeed;

            // Move towards player, if we are not touching the wall
            if (Dungeon.IsBlocking(pos + movement))
            {
                pos.X += direction.X * deltaTime;
                pos.Y += direction.Y * deltaTime;
            } else
                pos += movement;


            ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!///
            ///EVERYTHING COMMENTED BELOW IS OLD TEST CODE///
            ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!///

            // A really shitty method of AI follow, just for testing
            //pos.X = player.pos.X - 15;
            //pos.Y = player.pos.Y - 15;

            //Vector2 movement = vel * deltaTime;
            //Vector2 differenceToPlayer = player.pos - pos;
            //differenceToPlayer.Normalize();

            //pos += differenceToPlayer * (float)gameTime.ElapsedGameTime.TotalMilliseconds * enemySpeed;
            //pos += differenceToPlayer * enemySpeed;

            //if (level.IsBlocking(pos + movement))
            //{
            //    vel.X = player.vel.X;
            //    vel.Y = player.vel.Y;
            //vel = Vector2.Transform(vel, Matrix.CreateRotationZ((float)(rnd.NextDouble())));
            //}
            //else
            //{
            //    pos += movement;
            //}

        }
    }
}
