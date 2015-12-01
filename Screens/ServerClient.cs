using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DungeonTest
{
    class Client : CoreGame
    {
        UdpClient manager;
        IPEndPoint HostIP;
        Thread listener;

        //time given to let the player read error text
        const int READ_TIME = 3;
        float timeSpentReading = 0;
        float lastUpdate = 0;

        public Client(IPAddress IP)
        {
            ClearSprites();

            Dungeon.status = "CONNECTING";
            Dungeon.Clear();

            HostIP = new IPEndPoint(IP, PORT);

            manager = new UdpClient();
            manager.Client.ReceiveTimeout = 1;
            manager.Client.Blocking = true;
            manager.Connect(HostIP);

            listener = new Thread(new ThreadStart(Listen));
            listener.IsBackground = true;
            listener.Start();
        }

        public override Screen Update(float deltaTime)
        {
            lastUpdate += deltaTime;

            if (lastUpdate >= MAX_IDLE_TIME)
            {
                Dungeon.status = "CONNECTION FAILED";
                timeSpentReading += deltaTime;

                if (timeSpentReading > READ_TIME || !loading)
                    return new Join();
            }

            SendUpdates();
            
            return base.Update(deltaTime);
        }

        private void Listen()
        {
            while (lastUpdate < MAX_IDLE_TIME && !closed)
            {
                if (manager.Available > 0)
                {
                    try
                    {
                        byte[] response = manager.Receive(ref HostIP);

                        //new update
                        lastUpdate = 0;

                        byte protocol = response[0];

                        response = response.Skip(1).ToArray();

                        switch (protocol)
                        {
                            case (byte)Protocol.UPDATE_ID:
                                Dungeon.status = "UPDATING CLIENT ID";
                                id = BitConverter.ToInt32(response, 0);
                                break;
                            case (byte)Protocol.UPDATE_ENTITY_DATA:
                                Dungeon.status = "FETCHING ENTITIES";
                                UpdateEntities(response);
                                break;
                            case (byte)Protocol.MAP_CHANGE:
                                Dungeon.status = "UPDATING DUNGEON";
                                Dungeon.LoadFromData(response);
                                Input.lockMouse = true;
                                loading = false;
                                break;
                            case (byte)Protocol.UPDATE_PLAYER:
                                Dungeon.status = "GETTING PLAYER DATA";
                                player.CopyBytes(response);
                                break;
                            case (byte)Protocol.SEND_SPRITE:
                                Dungeon.status = "DOWNLOADING ASSETS";
                                TextureData data = new TextureData(response);
                                sprites.Add(data);
                                spriteNames.Add("?");
                                break;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        void UpdateEntities(byte[] response)
        {
            List<Entity> PlayerResponseList = new List<Entity>();
            List<Entity> EntityResponseList = new List<Entity>();

            for (int i = 0; i < response.Length; i += Entity.BYTES)
            {
                Entity e = new Entity(id, 0, 0);
                int type = BitConverter.ToInt32(response, i);

                switch (type)
                {
                    case 0:
                        if (i / Entity.BYTES == id)
                            PlayerResponseList.Add(player);
                        else
                            PlayerResponseList.Add(e);
                        break;
                    default:
                        EntityResponseList.Add(e);
                        break;
                }

                byte[] EntityBytes = response.Skip(i).Take(Entity.BYTES).ToArray();

                if (i / Entity.BYTES != id)
                    e.CopyBytes(EntityBytes);
            }

            Dungeon.entities = EntityResponseList;
            players = PlayerResponseList;
            UpdateEntityArray();
        }

        void SendUpdates()
        {
            //send player updates to the server
            byte[] message = new byte[4 + Entity.BYTES];

            byte[] clientID = BitConverter.GetBytes(id);
            byte[] playerData = player.GetSendableFormat();

            clientID.CopyTo(message, 0);
            playerData.CopyTo(message, 4);

            manager.Send(message, message.Length);
        }
    }
}
