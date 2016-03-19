using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Dungeontest
{
    class Client : CoreGame
    {
        UdpClient manager;
        IPEndPoint HostIP;

        //time given to let the player read error text
        const int READ_TIME = 3;
        float timeSpentReading = 0;
        float lastUpdate = 0;

        public Client(IPAddress IP)
        {
            server = false;

            players.Clear();
            ClearContent();

            Dungeon.task = "CONNECTING";
            Dungeon.Clear();

            HostIP = new IPEndPoint(IP, PORT);

            manager = new UdpClient();
            manager.Client.ReceiveTimeout = 1;
            manager.Client.Blocking = true;
            manager.Connect(HostIP);
        }

        public override Screen Update(float deltaTime)
        {
            lastUpdate += deltaTime;

            if (lastUpdate >= MAX_IDLE_TIME)
            {
                Dungeon.task = "CONNECTION FAILED";
                timeSpentReading += deltaTime;

                if (timeSpentReading > READ_TIME || !loading)
                    return new Join();
            }

            Listen();
            SendUpdates();
            
            return base.Update(deltaTime);
        }

        private void Listen()
        {
            while (manager.Available > 0)
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
                            Dungeon.task = "UPDATING CLIENT ID";
                            id = BitConverter.ToInt32(response, 0);
                            break;
                        case (byte)Protocol.UPDATE_ENTITY_DATA:
                            Dungeon.task = "FETCHING ENTITIES";
                            UpdateEntities(response);
                            break;
                        case (byte)Protocol.MAP_CHANGE:
                            Dungeon.task = "UPDATING DUNGEON";
                            Dungeon.LoadFromData(response);
                            Input.lockMouse = true;
                            loading = false;
                            break;
                        case (byte)Protocol.UPDATE_PLAYER:
                            Dungeon.task = "GETTING PLAYER DATA";
                            player.CopyBytes(response);
                            break;
                        case (byte)Protocol.SEND_SPRITE:
                            Dungeon.task = "DOWNLOADING ASSETS";
                            TextureData data = new TextureData(response);
                            sprites.Add(data);
                            spriteNames.Add("?");
                            break;
                    }
                }
                catch (Exception) { }
            }
        }

        void UpdateEntities(byte[] response)
        {
            players.Clear();
            Dungeon.entities.Clear();

            for (int i = 0; i < response.Length; i += Entity.BYTES)
            {
                Entity e = new Entity(0, 0, 0);
                int type = BitConverter.ToInt32(response, i);

                switch (type)
                {
                    case 0:
                        if (i / Entity.BYTES == id)
                            players.Add(player);
                        else
                            players.Add(e);
                        break;
                    default:
                        Dungeon.entities.Add(e);
                        break;
                }

                byte[] EntityBytes = response.Skip(i).Take(Entity.BYTES).ToArray();

                if (i / Entity.BYTES != id)
                    e.CopyBytes(EntityBytes);
            }

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
