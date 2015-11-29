﻿using Microsoft.Xna.Framework.Input;
namespace DungeonTest
{
    class SinglePlayer : CoreGame
    {
        public SinglePlayer()
        {
            Dungeon.Generate();

            players.Add(player);
        }

        void FinishInitialization()
        {
            Dungeon.MoveToSpawn(player);

            Input.lockMouse = true;
            loading = false;
            initializing = false;
        }


        public override Screen Update(float deltaTime)
        {
            if (initializing && Dungeon.complete)
                FinishInitialization();

            if (loading)
                return this;

            if (Input.Tapped(Keys.F5) && Dungeon.complete)
            {
                initializing = true;
                Dungeon.Generate();
            }

            player.Update(player, deltaTime);
            Dungeon.Update(player, deltaTime);

            return base.Update(deltaTime);
        }
    }
}
