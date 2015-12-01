using System;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Collections.Generic;

namespace DungeonTest
{
    public class API
    {
        public static int ClaimID(string name, string src)
        {
            return Dungeon.ClaimID(name, src);
        }

        public static void SpawnEntity(int id, float x, float y)
        {
            Entity myEntity = new Entity(id, x, y);

            Dungeon.entities.Add(myEntity);
        }

        public static object GetBlock(int x, int y)
        {
            return Dungeon.GetBlockAt(x, y);
        }
    }

    public class ModHandler
    {
        public static List<Script> mods = new List<Script>();


        // Mod API stuff
        public static void LoadMods()
        {
            // Set script loader
            Script.DefaultOptions.ScriptLoader = new EmbeddedResourcesScriptLoader();

            //Register classes
            UserData.RegisterType<API>();
            UserData.RegisterType<Input>();
            UserData.RegisterType<Block>();

            // Script Loader Base
            //((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "mods/?", "mods/?.lua" };
            //((ScriptLoaderBase)script.Options.ScriptLoader).IgnoreLuaPathGlobal = true;

            // Load mods
            string folderPath = "mods/";

            foreach (string file in Directory.EnumerateFiles(folderPath, "*.lua"))
            {
                Console.Write("\n[dungeontest] initializing mod '{0}'\n", file);
                try
                {
                    Script script = new Script();
                    DynValue api = UserData.Create(new API());
                    script.Globals.Set("API", api);
                    DynValue input = UserData.Create(new Input());
                    script.Globals.Set("Input", input);

                    // Load the file
                    script.DoFile(file);

                    mods.Add(script);

                    // Log completion
                    Console.WriteLine("\n[dungeontest] mod '{0}' loaded!\n", file);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n[dungeontest] error occured in loading '{0}' mod: {1}\n", file, ex.Message);
                    Console.WriteLine("\n[dungeontest] could not load '{0}' mod!\n", file);
                }
            }
        }

        // API Error Handling (this does nothing currently)
        static void DoError()
        {
            throw new ScriptRuntimeException("[dungeontest] fatal error occured!");
        }

        // implemented methods:
        //
        // PreGenerate - claim sprite IDs here
        // Update - runs on server

        public static void RunEvent(string methodName)
        {
            try
            {
                //run the event on every mod
                foreach (Script mod in mods)
                {
                    object method = mod.Globals[methodName];

                    //method exists then call it
                    if (method != null)
                        mod.Call(method);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[dungeontest] error occured " + e.Message);
            }
        }
    }
}
