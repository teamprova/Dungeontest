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
            ModHandler.LoadMods();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Exiting game
            if (currentScreen.RequestExit)
                Exit();

            if (IsActive)
            {// Window has focus
                // Update input
                Input.Update(Window.ClientBounds);
                // Hide the mouse if it is locked to the center
                IsMouseVisible = !Input.lockMouse;
            }

            if (Input.Tapped(Keys.F11))
            {// Pressed F11
                // Fullscreen game
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Get the deltaTime
            Dungeon.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the screen
            currentScreen = currentScreen.Update(Dungeon.deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw the current screen
            currentScreen.Draw(GraphicsDevice, spriteBatch);

            base.Draw(gameTime);
        }
    }
}