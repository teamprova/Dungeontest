using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonTest
{
    abstract class CoreGame : Screen
    {
        // Connection
        public static int PORT = 56200;

        // 5 seconds before kicking players
        protected const int MAX_IDLE_TIME = 5;

        // Resolution
        public const int GAME_WIDTH = 300;
        public const int GAME_HEIGHT = 200;
        const float GAME_SCALE = 2;
        const int LOADING_BAR_WIDTH = 560;

        // Test Stuff
        const int MAKE_ME_FEEL_TALLER = 0;

        const float MIN_BRIGHTNESS = .2f;

        public static TextureData roofTextureData;

        static Vector2 STATUS_TEXT_POS = new Vector2(20, 20);

        Container loadingBar = new Container(20, 340, 0, 40);

        Texture2D canvas;
        TextureData ctx = new TextureData(GAME_WIDTH, GAME_HEIGHT);

        public static float fov = 60;

        protected Entity player = new Entity(0, Dungeon.spawn.X, Dungeon.spawn.Y);
        protected List<Entity> players = new List<Entity>();
        protected Entity[] entityArray = new Entity[] { };

        public static List<TextureData> sprites = new List<TextureData>();
        public static List<string> spriteNames = new List<string>();

        protected bool initializing = true;
        protected bool loading = true;
        protected int id = 0;
        protected bool closed = false;

        bool debugging = false;
        float averagefps = 0;
        List<float> fps = new List<float>();
        public int headBob = 0;
        double bobAngle = 0;

        public void ClearSprites()
        {
            sprites.Clear();
            spriteNames.Clear();
        }

        protected void LoadDefaultSprites()
        {
            Dungeon.ClaimID("Player", "Content/Sprites/Entities/player.png");
            Dungeon.ClaimID("Cement", "Content/Sprites/Blocks/cement.png");
            Dungeon.ClaimID("CementBricks", "Content/Sprites/Blocks/brick.png");
        }
        
        public override Screen Update(float deltaTime)
        {
            GetFPS(deltaTime);

            if (Input.Tapped(Keys.Escape))
            {
                closed = true;
                return new Start();
            }
            if (Input.Tapped(Keys.F3))
                debugging = !debugging;

            if(!loading)
                ControlPlayer();

            return this;
        }

        // TODO : Move to a lua script
        void ControlPlayer()
        {
            player.angle += Input.deltaMouse.X * Dungeon.deltaTime / 4;
            player.angle %= Math.PI * 2;

            float forwardSpeed = Input.movement.Y * Dungeon.deltaTime * 4;
            float sideSpeed = Input.movement.X * Dungeon.deltaTime * 4;

            player.vel.X += (float)Math.Cos(player.angle) * -forwardSpeed;
            player.vel.Y += (float)Math.Sin(player.angle) * -forwardSpeed;

            player.vel.X += (float)Math.Cos(player.angle + Math.PI / 2) * sideSpeed;
            player.vel.Y += (float)Math.Sin(player.angle + Math.PI / 2) * sideSpeed;


            if (!Dungeon.IsBlocking(player.pos + player.vel))
            {
                player.pos += player.vel;

                bobAngle += 5 * player.vel.Length();
                bobAngle %= Math.PI * 2;

                headBob = (int)(Math.Sin(bobAngle) * 4);
            }

            player.vel = Vector2.Zero;
        }

        protected void UpdateEntityArray()
        {
            entityArray = players.Concat(Dungeon.entities).ToArray();
        }

        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            base.Draw(GraphicsDevice, spriteBatch);

            GraphicsDevice.Clear(Color.Black);

            if (loading)
            {
                DrawLoadingScreen(GraphicsDevice, spriteBatch);
                return;
            }

            if (canvas == null)
                canvas = new Texture2D(GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);

            // Clear the canvas
            ctx.Clear();
            RayCast();
            DrawEntities();

            if(debugging)
            DrawMiniMap();

            canvas.SetData(ctx.data);

            BeginSpriteBatch(spriteBatch);
            // Draw the screen
            spriteBatch.Draw(canvas, Vector2.Zero, null, Color.White, 0, Vector2.Zero, GAME_SCALE, SpriteEffects.None, 0);

            if(debugging)
                DrawDebuggingInfo(spriteBatch);

            spriteBatch.End();
        }

        protected void GetFPS(float deltaTime)
        {
            if(deltaTime != 0)
                fps.Add(1 / deltaTime);

            averagefps = 0;

            foreach (float frames in fps)
            {
                averagefps += frames;
            }

            averagefps = (int)(averagefps / fps.Count);
        }

        void DrawStringRight(SpriteBatch spriteBatch, string text, float x, float y)
        {
            text = Tool.FormatString(text);

            Vector2 stringSize = Tool.Font.MeasureString(text);

            Vector2 offset = new Vector2(resolution.X - stringSize.X - x, y);

            spriteBatch.DrawString(Tool.Font, text, offset, Color.White);

        }

        void DrawDebuggingInfo(SpriteBatch spriteBatch)
        {
            string fpsString = averagefps.ToString();

            DrawStringRight(spriteBatch, ServerHost.IPv4, 0, 0);
            DrawStringRight(spriteBatch, fpsString, 0, 30);
        }


        void DrawLoadingScreen(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            //draw the loading bar
            loadingBar.Width = (int)(Dungeon.GetProgress() * LOADING_BAR_WIDTH);
            loadingBar.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.Begin();

            //draw the status
            spriteBatch.DrawString(Tool.Font, Dungeon.status, STATUS_TEXT_POS, Color.White);

            spriteBatch.End();
        }

        void DrawEntities()
        {
            for(int i = 0; i < entityArray.Length; i++)
                if(i != id)
                    entityArray[i].Draw(ctx, player, headBob);
        }

        public void RayCast()
        {
            double fovInRadians = fov * Math.PI / 180f;

            for (var i = 0; i < canvas.Width; i++)
            {
                double rayAngle = (i / (float)GAME_WIDTH) * fovInRadians - fovInRadians / 2f;

                //add the player's angle for camera rotation
                rayAngle += player.angle;
                int bob = MAKE_ME_FEEL_TALLER + headBob;

                Block block;
                float textureX;
                Vector2 collision;

                //cast rays
                float dist = Dungeon.CastRay(player, rayAngle, out block, out textureX, out collision);
                RoofCast(rayAngle, i, bob);
                FloorCast(rayAngle, i, bob);

                DrawRay(block, i, bob, rayAngle, dist, textureX, collision);
            }
        }

        public void DrawRay(Block block, int x, int bob, double rayAngle, float dist, float textureX, Vector2 collision)
        {
            int height = (int)(GetViewDist() / (dist * Math.Cos(player.angle - rayAngle)));
            if (height == 0)
                return;

            int offsetHeight = (GAME_HEIGHT - height) / 2 + bob;

            float brightness = GetBrightness(collision.X, collision.Y);

            for (int i = (offsetHeight < 0) ? -offsetHeight : 0; i < height; i++)
            {
                if (i + offsetHeight > GAME_HEIGHT)
                    break;

                if (block == null)
                    continue;

                TextureData sprite = sprites[block.blockType];

                int pixelY = i * sprite.height / height;
                int pixelX = (int)(textureX * sprite.width);

                Color pixel = sprite.GetPixel(pixelX, pixelY);

                if (collision.X % 1 == 0)
                {
                    pixel.R -= 10;
                    pixel.G -= 10;
                    pixel.B -= 10;
                }

                TextureData.Darken(ref pixel, brightness);

                ctx.SetPixel(x, i + offsetHeight, pixel);
                ctx.SetZIndex(x, i + offsetHeight, dist);
            }
        }

        void RoofCast(double rayAngle, int x, int bob)
        {
            Matrix transform = Matrix.CreateRotationZ((float)rayAngle);
            double offsetAngle = Math.Cos(player.angle - rayAngle);


            for (int y = 0; y < GAME_HEIGHT / 2 + MAKE_ME_FEEL_TALLER; y++)
            {
                if (ctx.GetPixel(x, y) == Color.Transparent)
                {
                    float slope = GAME_HEIGHT / 2 - y + bob;
                    Color pixel = Color.Black;
                    float slopeIntercept = float.MaxValue;

                    if (slope != 0)
                    {
                        Vector2 texturePos = Vector2.Zero;

                        slopeIntercept = (float)(GetViewDist() / 2 / slope / offsetAngle);

                        texturePos.X = slopeIntercept;
                        texturePos = Vector2.Transform(texturePos, transform);
                        texturePos += player.pos;

                        if (texturePos.X < 0 || texturePos.Y < 0)
                            continue;

                        float brightness = GetBrightness(texturePos.X, texturePos.Y) / 2;

                        texturePos.X %= 1;
                        texturePos.Y %= 1;

                        int spriteX = roofTextureData.width - (int)(texturePos.X * roofTextureData.width) - 1;
                        int spriteY = (int)(texturePos.Y * roofTextureData.height);

                        pixel = roofTextureData.GetPixel(spriteX, spriteY);
                        TextureData.Darken(ref pixel, brightness);
                    }

                    ctx.SetPixel(x, y, pixel);
                    ctx.SetZIndex(x, y, slopeIntercept);
                }
            }
        }

        void FloorCast(double rayAngle, int x, int bob)
        {
            Matrix transform = Matrix.CreateRotationZ((float)rayAngle);
            double offsetAngle = Math.Cos(player.angle - rayAngle);

            for (int y = GAME_HEIGHT / 2 + MAKE_ME_FEEL_TALLER; y < GAME_HEIGHT; y++)
            {
                if (ctx.GetPixel(x, y) == Color.Transparent)
                {
                    float slope = y - GAME_HEIGHT / 2 - MAKE_ME_FEEL_TALLER - headBob;
                    Color pixel = Color.Black;
                    float slopeIntercept = float.MaxValue;

                    if (slope != 0)
                    {
                        Vector2 texturePos = Vector2.Zero;


                        slopeIntercept = (float)(GetViewDist() / 2 / slope / offsetAngle);
                        texturePos.X = slopeIntercept;
                        texturePos = Vector2.Transform(texturePos, transform);
                        texturePos += player.pos;


                        if (texturePos.Y < 0)
                        {
                            texturePos.Y += (float)Math.Ceiling(Math.Abs(texturePos.Y));
                        }

                        texturePos.X = Math.Abs(texturePos.X);
                        texturePos.Y = Math.Abs(texturePos.Y);

                        float brightness = GetBrightness(texturePos.X, texturePos.Y);

                        Block block = Dungeon.GetBlockAt(texturePos.X, texturePos.Y);

                        if (block != null)
                        {
                            texturePos.X %= 1;
                            texturePos.Y %= 1;

                            TextureData sprite = sprites[block.blockType];

                            int spriteX = (int)(texturePos.X * sprite.width);
                            int spriteY = (int)(texturePos.Y * sprite.height);

                            pixel = sprite.GetPixel(spriteX, spriteY);
                            TextureData.Darken(ref pixel, brightness);
                        }
                    }

                    ctx.SetPixel(x, y, pixel);
                    ctx.SetZIndex(x, y, slopeIntercept);
                }
            }
        }

        public static double GetViewDist()
        {
            return (GAME_WIDTH / 2) / Math.Tan(fov * Math.PI / 360);
        }

        void DrawMiniMap()
        {
            Point offset = new Point(0, 0);

            // loop through world
            for (int x = 0; x < Dungeon.WIDTH; x++)
                for (int y = 0; y < Dungeon.HEIGHT; y++)
                {
                    //draw solid blocks
                    if (!Dungeon.IsBlocking(x, y))
                        ctx.SetPixel(x + offset.X, y + offset.Y, 0, Color.FromNonPremultiplied(0, 255, 0, 100));
                }

            // draw the player
            ctx.SetPixel((int)player.pos.X + offset.X, (int)player.pos.Y + offset.Y, Color.FromNonPremultiplied(50, 50, 255, 200));
        }

        float GetBrightness(float x, float y)
        {
            float brightness = MIN_BRIGHTNESS;
            Vector2 coords = new Vector2(x, y);

            foreach (Entity e in entityArray)
            {
                if (e.luminosity == 0)
                    continue;

                Vector2 vectDist = coords - e.pos;
                float distance = vectDist.Length();

                float lightFromEntity = e.luminosity;

                if (distance != 0)
                    lightFromEntity /= distance;

                // current brightness is the brightest
                brightness = Math.Max(brightness, lightFromEntity);

                if (brightness >= 1)
                    return 1;
            }

            return brightness;
        }
    }
}
