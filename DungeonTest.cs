using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonTest
{
    public class DungeonTest : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Screen currentScreen;
        Stopwatch elapsedTimeWatch = new Stopwatch();

        public DungeonTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;
        }

        // Load a testing mod
        static void EmbeddedResourceScriptLoader()
        {
	       Script script = new Script();
	       Script.DefaultOptions.ScriptLoader = new EmbeddedResourcesScriptLoader();
           Console.Write("[dungeontest] Initializing test mod");
	       script.DoFile("mods/test.lua");
           Console.WriteLine("[dungeontest] test mod loaded!");
        }


        protected override void Initialize()
        {
            base.Initialize();
            EmbeddedResourceScriptLoader();
            elapsedTimeWatch.Start();
            currentScreen = new Start();
        }

        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Tool.CreateBackground(GraphicsDevice);
            Tool.Font = Content.Load<SpriteFont>("Fonts/default");
<<<<<<< HEAD

            Player.frontSprite = new TextureData(Content, "Sprites/Entities/player");
            Enemy.sprite = new TextureData(Content, "Sprites/Entities/player");
=======
            
            Player.frontSprite = new TextureData("Content/Sprites/Entities/player.png");
            Enemy.sprite = new TextureData("Content/Sprites/Entities/player.png");
>>>>>>> d3c236b1b42626e3312e614463f5cc47b73f18f0

            //blocks
            CoreGame.roofTextureData = new TextureData("Content/Sprites/roof.png");
            Block.Cobble = new TextureData("Content/Sprites/Blocks/cobble.png");
            Block.Wood = new TextureData("Content/Sprites/Blocks/wood.png");
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

            float deltaTime = GetDeltaTime();

            currentScreen = currentScreen.Update(deltaTime);

            base.Update(gameTime);
        }

        float GetDeltaTime()
        {
            float deltaTime = elapsedTimeWatch.ElapsedMilliseconds;
            elapsedTimeWatch.Restart();

            return deltaTime / 1000f;
        }

        void LeaveGame()
        {
            try {
                Exit();
            } catch (Exception e) { }
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(GraphicsDevice, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
