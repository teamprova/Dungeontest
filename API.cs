using System;
using System.IO;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace DungeonTest
{
    public class API
    {

        /// <summary>
        /// Claims a location in the game's sprite list
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="src">File location</param>
        /// <returns>The sprite's ID</returns>
        public static int ClaimID(string name, string src)
        {
            int id = CoreGame.sprites.Count;
            Console.WriteLine("\n[dungeontest] creating entity '{0}' under id '{1}'\n", name, id);

            try
            {
                //add name to sprite list
                CoreGame.spriteNames.Add(name);

                //add sprite to sprite list
                CoreGame.sprites.Add(new TextureData(src));

                Console.WriteLine("\n[dungeontest] created entity '{0}' under id '{1}'\n", name, id);

            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[dungeontest] error occured when spawning entity '{0}' under id '{1}'. error: {2}\n", name, id, ex.Message);
            }

            return id;
        }
			

        /// <summary>
        /// Spawns an an entity at (x, y)
        /// </summary>
        /// <param name="id">The ID of the sprite to use</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void SpawnEntity(int id, float x, float y)
        {
            Entity myEntity = new Entity(id, x, y);
            Console.WriteLine("\n[dungeontest] spawning entity '{0}'\n", id);
            try
            {
                Dungeon.entities.Add(myEntity);
                Console.WriteLine("\n[dungeontest] entity '{0}' spawned\n", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[dungeontest] error occured when spawning '{0}'. error: {1}\n", id, ex.Message);
            }
        }

        /// <summary>
        /// Checks if the block is a wall
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsBlockSolid(int x, int y)
        {
            return Dungeon.IsBlocking(x, y);
        }

        /// <summary>
        /// Returns the block at (x, y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static object GetBlock(int x, int y)
        {
            return Dungeon.GetBlockAt(x, y);
        }

        /// <summary>
        /// Sets the block at (x, y) to specified ID
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="id">The ID of the sprite to use</param>
        /// <param name="solid">Making the block a wall or floor</param>
        /// <returns></returns>
        public static void SetBlock(int x, int y, byte id, bool solid)
        {
            Dungeon.SetBlockAt(x, y, new Block(id, solid));
        }

        /// <summary>
        /// Gets the closest player to this entity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Entity GetNearestPlayer(Entity entity)
        {
            float closestDistance = float.MaxValue;
            Entity closestPlayer = CoreGame.players[0];

            foreach (Entity player in CoreGame.players)
            {
                float distance = Vector2.Distance(player.pos, entity.pos);

                //if this is closer than the closest found so far then this is the closest player
                if (distance < closestDistance)
                {
                    //set the closest distance
                    closestDistance = distance;
                    //set the closest player
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }

        /// <summary>
        /// Changes the name of the task in the loading screen
        /// </summary>
        /// <param name="name"></param>
        public static void ChangeTask(string name)
        {
            Dungeon.task = name;
        }

        public static void SetDungeonSize(int width, int height)
        {
        
        }
    }

    public class ModHandler
    {
        const string MODS_FOLDER = "mods/";

        public static List<Script> mods = new List<Script>();

        /// <summary>
        /// Load the mods in the mods folder
        /// </summary>
        public static void LoadMods()
        {
            Console.WriteLine("\n[dungeontest] loading mods\n");
            // Set script loader
            //Script.DefaultOptions.ScriptLoader = new EmbeddedResourcesScriptLoader();

            // Register classes
            UserData.RegisterType<API>();
            UserData.RegisterType<Input>();
            UserData.RegisterType<Block>();
            UserData.RegisterType<Entity>();
            UserData.RegisterType<Keys>();
            UserData.RegisterType<Vector2>();

            // Load static classes
            DynValue api = UserData.Create(new API());
            DynValue input = UserData.Create(new Input());
            DynValue keys = UserData.Create(new Keys());
			DynValue sound = UserData.Create(new Sounds());

            // Script Loader Base
            //((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "mods/?", "mods/?.lua" };
            //((ScriptLoaderBase)script.Options.ScriptLoader).IgnoreLuaPathGlobal = true;

            foreach (string file in Directory.EnumerateFiles(MODS_FOLDER, "*.lua"))
            {
                Console.Write("\n[dungeontest] initializing mod '{0}'\n", file);

                try
                {
                    Script script = new Script();
                    script.Globals.Set("API", api);
                    script.Globals.Set("Input", input);
                    script.Globals.Set("Keys", keys);
					script.Globals.Set("Sound", sound);

                    // Load the file
                    script.DoFile(file);

                    mods.Add(script);

                    // Log completion
                    Console.WriteLine("\n[dungeontest] mod '{0}' loaded!\n", file);
                }
                catch (ScriptRuntimeException ex)
                {
                    // Alerting the issue loading a mod
                    Console.WriteLine("\n[dungeontest] error occured in loading '{0}' mod: {1}\n", file, ex.Message);
                    Console.WriteLine("\n[dungeontest] could not load '{0}' mod!\n", file);
                }

                // Print how many mods were loaded
                Console.WriteLine("\n[dungeontest] {0} mods have been loaded\n", mods.Count);
            }
        }

        // API Error Handling (this does nothing currently)
        private static void DoError()
        {
            throw new ScriptRuntimeException("\n[dungeontest] fatal error occured!\n");
        }

        // implemented events:
        //
        // PreGenerate() - Runs before dungeon is generated
        // PostGenerate() - Runs after dungeon is generated
        // ServerUpdate(float deltaTime) - Runs every time the server updates
        // EntityUpdate(Entity entity) - Runs when the entity needs an update

        /// <summary>
        /// Runs event handler on every mod
        /// </summary>
        /// <param name="eventName">The event to handle</param>
        /// <param name="args">Parameters for the event</param>
        public static void HandleEvent(string eventName, params object[] args)
        {
            try
            {
                // Run the handler for the event on every mod
                foreach (Script mod in mods)
                {
                    // Get the method
                    object method = mod.Globals[eventName];

                    // See if it exists then call it
                    if (method != null)
                        mod.Call(method, args);
                }
            }
            catch (ScriptRuntimeException e)
            {
                // Error warning
                Console.WriteLine("\n[dungeontest] \"" + eventName + "\" event error: " + e.DecoratedMessage + "\n");
            }
        }
    }
}