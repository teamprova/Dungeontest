﻿using Microsoft.Xna.Framework.Input;
namespace Dungeontest
{
    class SinglePlayer : CoreGame
    {
        public SinglePlayer()
        {
            server = true;

            players.Clear();
            ClearContent();

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

            if (!loading)
            {
                if (Input.Tapped(Keys.F5) && Dungeon.complete)
                {
                    initializing = true;
                    Dungeon.Generate();
                }

                Dungeon.Update();
                UpdateEntityArray();
            }

            return base.Update(deltaTime);
        }
    }
}
