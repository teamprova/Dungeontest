using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DungeonTest
{
    class Dungeon
    {
        const double TAU = Math.PI * 2;

        public static List<Entity> entities = new List<Entity>();
        public static Vector2 spawn = new Vector2(2, 2);

        public static Block[,] map;
        public static byte[] mapData = new byte[] { };
        public static int width = 100;
        public static int height = 100;

        public static float deltaTime = 0;

        public static Thread generationThread;
        public static string task = "";
        public static bool complete = false;

        #region Generator
        public static void Generate()
        {
            complete = false;
            task = "";
            Clear();

            Console.WriteLine("\n[dungeontest] starting map generation\n");

            generationThread = new Thread(new ThreadStart(GeneratorThread));
            generationThread.IsBackground = true;
            generationThread.Start();
        }

        static void GeneratorThread()
        {
            ModHandler.HandleEvent("PreGenerate");
            ModHandler.HandleEvent("PostGenerate");
            UpdateData();

            complete = true;

            Console.WriteLine("\n[dungeontest] map generation complete\n");
        }

        public static void Clear()
        {
            map = new Block[width, height];
            entities.Clear();
        }

        public static float GetProgress()
        {
            float placedBlocks = 0;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (GetBlockAt(x, y) != null)
                        placedBlocks++;
                }

            return placedBlocks / (width * height);
        }
        #endregion

        #region Multiplayer
        static void UpdateData()
        {
            // 9 = protocol + spawn position
            mapData = new byte[9 + height * width * 2];

            mapData[0] = (byte)Protocol.MAP_CHANGE;
            BitConverter.GetBytes(spawn.X).CopyTo(mapData, 1);
            BitConverter.GetBytes(spawn.Y).CopyTo(mapData, 5);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int i = 9 + (y * width + x) * 2;

                    Block block = GetBlockAt(x, y);

                    mapData[i] = block.blockType;
                    mapData[i + 1] = BitConverter.GetBytes(block.solid)[0];
                }
        }

        public static void LoadFromData(byte[] mapData)
        {
            spawn.X = BitConverter.ToSingle(mapData, 0);
            spawn.Y = BitConverter.ToSingle(mapData, 4);

            Clear();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int i = 8 + (y * width + x) * 2;

                    byte type = mapData[i];
                    bool solid = BitConverter.ToBoolean(mapData, i + 1);

                    SetBlockAt(x, y, new Block(type, solid));
                }
        }
        #endregion

        // Modders may find this useful
        #region Tools
        
        /// <summary>
        /// Move the entity to the dungeon's spawn location
        /// </summary>
        /// <param name="e"></param>
        public static void MoveToSpawn(Entity e)
        {
            e.pos.X = spawn.X;
            e.pos.Y = spawn.Y;
        }
        
        /// <summary>
        /// For when all you want is to find the nearest block and measure the distance
        /// </summary>
        /// <param name="e">Entity to cast the ray from</param>
        /// <param name="rayAngle">Angle of the ray</param>
        /// <param name="collision">Where the ray hit</param>
        /// <returns></returns>
        public static float CastRay(Entity e, double rayAngle, out Vector2 collision)
        {
            float textureX;
            Block block;

            return CastRay(e, rayAngle, out block, out textureX, out collision);
        }

        /// <summary>
        /// Checks if the block is a wall
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsBlocking(Vector2 pos)
        {
            return IsBlocking(pos.X, pos.Y);
        }

        /// <summary>
        /// Checks if the block is a wall
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsBlocking(double x, double y)
        {// non air block in this location
            Block block = GetBlockAt(x, y);

            if (block == null)
                return true;

            return block.solid;
        }

        /// <summary>
        /// Gets the block at these coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Block GetBlockAt(int x, int y)
        {
            if (IsWithin(x, y))
                return map[y, x];
            return null;
        }
        
        /// <summary>
        /// Gets the block at these coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Block GetBlockAt(double x, double y)
        {
            return GetBlockAt((int)x, (int)y);
        }

        /// <summary>
        /// Sets the block at these coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void SetBlockAt(int x, int y, Block block)
        {
            if (IsWithin(x, y))
                map[y, x] = block;
        }

        /// <summary>
        /// Checks the coordinates to see if they're within the dungeon
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsWithin(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        /// <summary>
        /// Checks the coordinates to see if they're within the dungeon
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsWithin(double x, double y)
        {
            return IsWithin((int)x, (int)y);
        }
        #endregion

        #region GameStuff
        /// <summary>
        /// Sends updates to mods to update the entities in the dungeon
        /// </summary>
        public static void Update()
        {
            // TODO: Mod entity updates here
            ModHandler.HandleEvent("ServerUpdate", deltaTime);

            foreach (Entity e in entities.ToArray())
            {
                ModHandler.HandleEvent("EntityUpdate", e);
            }
        }

        public static float CastRay(Entity e, double rayAngle, out Block block, out float textureX, out Vector2 collision)
        {
            // Make sure the angle is between 0 and 360 degrees
            if (rayAngle < 0)
                rayAngle += TAU;
            rayAngle %= TAU;

            // Moving right/left? up/down? Determined by
            // which quadrant the angle is in
            bool right = rayAngle > TAU * 0.75 || rayAngle < TAU * 0.25;
            bool up = rayAngle < 0 || rayAngle > Math.PI;

            double angleSin = Math.Sin(rayAngle);
            double angleCos = Math.Cos(rayAngle);

            // The distance to the block we hit
            Vector2 dist = Vector2.Zero;

            //out variables
            textureX = 0;
            collision = new Vector2(0, 0);
            block = new Block(0, false);

            // First check against the vertical map/wall lines
            // we do this by moving to the right or left edge
            // of the block we're standing in and then moving
            // in 1 map unit steps horizontally. The amount we have
            // to move vertically is determined by the slope of
            // the ray, which is simply defined as sin(angle) / cos(angle).

            double slope;
            float dX, dY, x, y;

            // The slope of the straight line made by the ray
            if (angleCos != 0)
            {
                slope = angleSin / angleCos;
                // We move either 1 map unit to the left or right
                dX = right ? 1 : -1;
                // How much to move up or down
                dY = dX * (float)slope;

                // Starting horizontal position, at one
                // of the edges of the current map block
                x = (float)(right ? Math.Ceiling(e.pos.X) : Math.Floor(e.pos.X));
                // Starting vertical position. We add the small horizontal
                // step we just made, multiplied by the slope
                y = (float)(e.pos.Y + (x - e.pos.X) * slope);

                while (IsWithin(x, y))
                {
                    double wallX = x + (right ? 0 : -1);
                    double wallY = y;

                    // Is this point inside a wall block?
                    if (IsBlocking(wallX, wallY))
                    {
                        dist.X = x - e.pos.X;
                        dist.Y = y - e.pos.Y;

                        textureX = y % 1;   // where exactly are we on the wall? textureX is the x coordinate on the texture that we'll use when texturing the wall.
                        if (!right)
                            textureX = 1 - textureX; // if we're looking to the left side of the map, the texture should be reversed

                        collision = new Vector2(x, y);
                        block = GetBlockAt(wallX, wallY);
                        break;
                    }

                    x += dX;
                    y += dY;
                }
            }

            //horizontal walls
            slope = angleCos / angleSin;
            dY = up ? -1 : 1;
            dX = dY * (float)slope;
            y = (float)(up ? Math.Floor(e.pos.Y) : Math.Ceiling(e.pos.Y));
            x = e.pos.X + (y - e.pos.Y) * (float)slope;
            Vector2 blockDist = Vector2.Zero;

            while (IsWithin(x, y))
            {
                double wallY = y + (up ? -1 : 0);
                double wallX = x;

                if (IsBlocking(wallX, wallY))
                {
                    blockDist.X = x - e.pos.X;
                    blockDist.Y = y - e.pos.Y;

                    if (dist == Vector2.Zero || blockDist.Length() < dist.Length())
                    {
                        dist = blockDist;
                        textureX = x % 1;
                        if (!up)
                            textureX = 1 - textureX;

                        collision = new Vector2(x, y);
                        block = GetBlockAt(wallX, wallY);
                    }
                    break;
                }
                x += dX;
                y += dY;
            }

            return dist.Length();
        }
        #endregion
    }
}
