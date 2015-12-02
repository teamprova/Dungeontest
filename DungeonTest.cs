using System;
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

        public DungeonTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;

            // Fixes the weird update issues and allows the game run at higher frame rates
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // load content first
            Console.WriteLine("\n[dungeontest] loading game content\n");
            base.Initialize();
            Console.WriteLine("\n[dungeontest] game content loaded\n");

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
            CoreGame.roofTextureData = new TextureData("Content/Sprites/roof.png");

            // Load mods
            Console.WriteLine("\n[dungeontest] loading mods\n");
            ModHandler.LoadMods();
            Console.WriteLine("\n[dungeontest] mods have been loaded\n");
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

            GetDeltaTime(gameTime);

            Dungeon.deltaTime = GetDeltaTime(gameTime);

            currentScreen = currentScreen.Update(Dungeon.deltaTime);

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
