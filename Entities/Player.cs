using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Player : Entity
    {
        const float SPEED = 4;
        public int headBob = 0;
        double bobAngle = 0;
        public float IdleTime = 0;

        public static TextureData frontSprite;

        public Player()
            : base(frontSprite, Dungeon.spawn.X, Dungeon.spawn.Y, 1)
        {
        }

        public override void Update(Player player, float deltaTime)
        {
            angle += Input.deltaMouse.X * deltaTime / 4;
            angle %= Math.PI * 2;

            float forwardSpeed = Input.movement.Y * deltaTime * SPEED;
            float sideSpeed = Input.movement.X * deltaTime * SPEED;

            vel.X += (float)Math.Cos(angle) * -forwardSpeed;
            vel.Y += (float)Math.Sin(angle) * -forwardSpeed;

            vel.X += (float)Math.Cos(angle + Math.PI / 2) * sideSpeed;
            vel.Y += (float)Math.Sin(angle + Math.PI / 2) * sideSpeed;


            if (!Dungeon.IsBlocking(pos + vel))
            {
                pos += vel;

                bobAngle += 5 * vel.Length();
                bobAngle %= Math.PI * 2;

                headBob = (int)(Math.Sin(bobAngle) * 4);
            }

            vel = Vector2.Zero;
        }
    }
}
