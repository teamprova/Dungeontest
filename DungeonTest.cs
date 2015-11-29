using System;
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

        protected override void Initialize()
        {
            base.Initialize();

            elapsedTimeWatch.Start();
            currentScreen = new Start();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Tool.CreateBackground(GraphicsDevice);
            Tool.Font = Content.Load<SpriteFont>("Fonts/default");
            
            Player.frontSprite = new TextureData(Content, "Sprites/Entities/player");
            Enemy.sprite = new TextureData(Content, "Sprites/Entities/player");

            //blocks
            CoreGame.roofTextureData = new TextureData(Content, "Sprites/roof");
            Block.Cobble = new TextureData(Content, "Sprites/Blocks/cobble");
            Block.Wood = new TextureData(Content, "Sprites/Blocks/wood");
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
