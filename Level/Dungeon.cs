using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DungeonTest
{
    class Dungeon
    {
        public static int WIDTH = 100;
        public static int HEIGHT = 100;

        const double TAU = Math.PI * 2;
        public static int MIN_ROOM_SIZE = 8;
        public static int MAX_ROOM_SIZE = 15;
        public static int MIN_ROOMS = 25;
        public static int MAX_ROOMS = 50;

        public static List<Entity> entities = new List<Entity>();
        public static Block[,] map = new Block[HEIGHT, WIDTH];
        public static Vector2 spawn = new Vector2(2, 2);
        public static byte[] mapData = new byte[] { };

        public static List<Rectangle> rooms = new List<Rectangle>();
        public static string status = "";
        public static bool complete = false;

        public static float deltaTime = 0;

        static Random rnd = new Random();
        static float roomsToMake = 0;

        public static Thread generationThread;

        #region Generator
        public static void Generate()
        {
            complete = false;
            status = "";
            Clear();

            Console.WriteLine("\n[dungeontest] starting map generation\n");

            ModHandler.RunEvent("PreGenerate");

            generationThread = new Thread(new ThreadStart(GeneratorThread));
            generationThread.IsBackground = true;
            generationThread.Start();
        }

        static void GeneratorThread()
        {
            // Variables for the room
            roomsToMake = rnd.Next(MIN_ROOMS, MAX_ROOMS); // Random room count

            MakeRooms();
            SquashRooms();
            PlaceBlocks();
            UpdateData();

            complete = true;

            ModHandler.RunEvent("PostGenerate");

            Console.WriteLine("\n[dungeontest] map generation complete\n");
        }

        public static void Clear()
        {
            map = new Block[WIDTH, HEIGHT];
            entities.Clear();
            rooms.Clear();
        }

        public static float GetProgress()
        {
            float placedBlocks = 0;

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (GetBlockAt(x, y) != null)
                        placedBlocks++;
                }

            return placedBlocks / (WIDTH * HEIGHT);
        }

        static void MakeRooms()
        {// Generate the rooms
            status = "MAKING ROOMS";
            Console.WriteLine("\n[dungeontest] generating rooms in map\n");

            for (int i = 0; i < roomsToMake; i++)
            {
                Rectangle room = new Rectangle();

                room.X = rnd.Next(1, WIDTH - MAX_ROOM_SIZE - 1);
                room.Y = rnd.Next(1, HEIGHT - MAX_ROOM_SIZE - 1);
                room.Width = rnd.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE);
                room.Height = rnd.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE);

                if (DoesCollide(room, i))
                {
                    i--;
                    continue;
                }
                room.Width--;
                room.Height--;

                rooms.Add(room);
            }
        }

        static void SquashRooms()
        {
            status = "SQUISHING ROOMS";
            Console.WriteLine("\n[dungeontest] squishing rooms in map\n");

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < rooms.Count; j++)
                {
                    Rectangle room = rooms[j];

                    while (true)
                    {
                        Point oldPosition = new Point(room.X, room.Y);

                        // try to move to 1, 1
                        if (room.X > 1) room.X--;
                        if (room.Y > 1) room.Y--;
                        rooms[j] = room;

                        //reached 1, 1
                        if ((room.X == 1) && (room.Y == 1))
                            break;

                        if (DoesCollide(room, j))
                        {// hit another room
                            // this is as far as we go
                            room.X = oldPosition.X;
                            room.Y = oldPosition.Y;
                            rooms[j] = room;
                            break;
                        }
                    }
                }
            }
        }

        static void PlaceBlocks()
        {
            //making pathways
            status = "CREATING PATHS";
            Console.WriteLine("\n[dungeontest] generating pathways\n");

            for (int i = 0; i < rooms.Count; i++)
            {
                Rectangle room = rooms[i];
                Rectangle closestRoom = FindClosestRoom(room, room);
                Rectangle secondClosestRoom = FindClosestRoom(room, closestRoom);

                CreatePath(room, closestRoom);
                CreatePath(room, secondClosestRoom);
            }

            //place rooms
            status = "PLACING ROOMS";
            Console.WriteLine("\n[dungeontest] placing pathways\n");

            foreach (Rectangle room in rooms)
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    for (int y = room.Y; y < room.Y + room.Height; y++)
                    {
                        SetBlockAt(x, y, new Block(1, false));
                    }
                }

            //set null blocks to solid cobble
            status = "PLACING BLOCKS";
            Console.WriteLine("\n[dungeontest] placing blocks\n");

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (GetBlockAt(x, y) == null)
                        SetBlockAt(x, y, new Block(2, true));
                }
        }

        static void CreatePath(Rectangle roomA, Rectangle roomB)
        {
            Point pointA = new Point(
                rnd.Next(roomA.X, roomA.X + roomA.Width),
                rnd.Next(roomA.Y, roomA.Y + roomA.Height));

            Point pointB = new Point(
               rnd.Next(roomB.X, roomB.X + roomB.Width),
               rnd.Next(roomB.Y, roomB.Y + roomB.Height)
            );
            while (!pointA.Equals(pointB))
            {
                if (pointB.X != pointA.X)
                {
                    if (pointB.X > pointA.X)
                        pointB.X--;
                    else
                        pointB.X++;
                }
                else if (pointB.Y != pointA.Y)
                {
                    if (pointB.Y > pointA.Y)
                        pointB.Y--;
                    else
                        pointB.Y++;
                }

                SetBlockAt(pointB.X, pointB.Y, new Block(1, false));
            }
        }

        static bool DoesCollide(Rectangle room, int ignore)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i == ignore)
                    continue;

                Rectangle check = rooms[i];

                if (!((room.X + room.Width < check.X) || (room.X > check.X + check.Width) || (room.Y + room.Height < check.Y) || (room.Y > check.Y + check.Height)))
                    return true;
            }

            return false;
        }

        static Rectangle FindClosestRoom(Rectangle room, Rectangle ignore)
        {
            Point mid = room.Center;

            Rectangle closest = new Rectangle();
            int closestDistance = 1000;

            for (int i = 0; i < rooms.Count; i++)
            {
                Rectangle check = rooms[i];
                if (check.Equals(room) || check.Equals(ignore)) continue;
                Point checkMid = check.Center;

                int distance = Math.Abs(mid.X - checkMid.X) + Math.Abs(mid.Y - checkMid.Y);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = check;
                }
            }
            return closest;
        }
        #endregion

        #region Multiplayer
        static void UpdateData()
        {
            // 9 = protocol + spawn position
            mapData = new byte[9 + HEIGHT * WIDTH * 2];

            mapData[0] = (byte)Protocol.MAP_CHANGE;
            BitConverter.GetBytes(spawn.X).CopyTo(mapData, 1);
            BitConverter.GetBytes(spawn.Y).CopyTo(mapData, 5);

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    int i = 9 + (y * WIDTH + x) * 2;

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

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    int i = 8 + (y * WIDTH + x) * 2;

                    byte type = mapData[i];
                    bool solid = BitConverter.ToBoolean(mapData, i + 1);

                    SetBlockAt(x, y, new Block(type, solid));
                }
        }
        #endregion

        // Modders may find this useful
        #region Tools

        public static void MoveToSpawn(Entity e)
        {
            e.pos.X = spawn.X;
            e.pos.Y = spawn.Y;
        }

        // For when all you want is to find the nearest block and measure the dist
        public static float CastRay(Entity e, double rayAngle, out Vector2 collision)
        {
            float textureX;
            Block block;

            return CastRay(e, rayAngle, out block, out textureX, out collision);
        }

        public static bool IsBlocking(Vector2 pos)
        {
            return IsBlocking(pos.X, pos.Y);
        }

        public static bool IsBlocking(double x, double y)
        {// non air block in this location
            Block block = GetBlockAt(x, y);

            if (block == null)
                return true;

            return block.solid;
        }

        public static Block GetBlockAt(int x, int y)
        {
            if (IsWithin(x, y))
                return map[y, x];
            return null;
        }

        public static Block GetBlockAt(double x, double y)
        {
            return GetBlockAt((int)x, (int)y);
        }

        public static void SetBlockAt(int x, int y, Block block)
        {
            if (IsWithin(x, y))
                map[y, x] = block;
        }

        public static bool IsWithin(int x, int y)
        {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        public static bool IsWithin(double x, double y)
        {
            return IsWithin((int)x, (int)y);
        }
        #endregion

        #region GameStuff
        public static void Update()
        {
            // TODO: Mod entity updates here
            ModHandler.RunEvent("Update");
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
