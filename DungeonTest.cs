using System;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace DungeonTest
{
    public class DungeonTest : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Screen currentScreen;

        public DungeonTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;

            // Fixes the weird update issues and allows the game run at higher frame rates
            IsFixedTimeStep = false;
        }

        // Load mods
        static void EmbeddedResourceScriptLoader()
        {
	       Script script = new Script();
	       Script.DefaultOptions.ScriptLoader = new EmbeddedResourcesScriptLoader();

           String folderPath = "mods/";
           foreach (String file in Directory.EnumerateFiles(folderPath, "*.lua"))
           {
               Console.Write("\n[dungeontest] initializing mod '{0}'\n", file);
               try
               {
                   script.DoFile(file);
                   Console.WriteLine("\n[dungeontest] mod '{0}' loaded!\n", file);
               }
               catch (ScriptRuntimeException ex)
               {
                   Console.WriteLine("\n[dungeontest] error occured in loading '{0}' mod: {1}\n", file, ex.DecoratedMessage);
                   Console.WriteLine("\n[dungeontest] could not load '{0}' mod!\n", file);
               }
           }

        }

        // API Error Handling (this does nothing currently)
        static void DoError()
        {
            throw new ScriptRuntimeException("[dungeontest] fatal error occured!");
        }

        protected override void Initialize()
        {
            // load content first
            Console.WriteLine("\n[dungeontest] loading game content\n");
            base.Initialize();
            Console.WriteLine("\n[dungeontest] game content loaded\n");

            // load scripts
            Console.WriteLine("\n[dungeontest] loading mods\n");
            EmbeddedResourceScriptLoader();
            Console.WriteLine("\n[dungeontest] mods have been loaded\n");

            // create main menu
            Console.WriteLine("\n[dungeontest] main menu loading\n");
            currentScreen = new Start();
            Console.WriteLine("\n[dungeontest] main menu loaded\n");
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Tool.CreateBackground(GraphicsDevice);
            Tool.Font = Content.Load<SpriteFont>("Fonts/default");

            Player.frontSprite = new TextureData("Content/Sprites/Entities/player.png");
            Enemy.sprite = new TextureData("Content/Sprites/Entities/player.png");

            //blocks
            CoreGame.roofTextureData = new TextureData("Content/Sprites/roof.png");
            Block.CementBrick = new TextureData("Content/Sprites/Blocks/brick.png");
            Block.Cement = new TextureData("Content/Sprites/Blocks/cement.png");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                Input.Update(Window.ClientBounds);
            }

            IsMouseVisible = !Input.lockMouse;

            if (Input.Tapped(Keys.F11))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            if (currentScreen.RequestExit)
                LeaveGame();

            float deltaTime = GetDeltaTime(gameTime);

            currentScreen = currentScreen.Update(deltaTime);

            base.Update(gameTime);
        }

        float GetDeltaTime(GameTime gameTime)
        {
            float elapsedMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            return elapsedMilliseconds / 1000f;
        }

        void LeaveGame()
        {
            try {
                Exit();
            } catch (Exception) { }
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(GraphicsDevice, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
