using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework.Input;

namespace Dungeontest
{
    public enum Protocol : byte { UPDATE_ID, UPDATE_PLAYER, MAP_CHANGE, UPDATE_ENTITY_DATA, SEND_SPRITE }

    class ServerHost : CoreGame
    {
        const int SIO_UDP_CONNRESET = -1744830452;
        const int UPDATESPEED = 1000 / 60;

        public static string IPv4 = "";

        Thread joinListener;
        List<IPEndPoint> Clients = new List<IPEndPoint>();
        IPEndPoint ConnectingIP = new IPEndPoint(IPAddress.Any, PORT);
        UdpClient NewConnections;
        UdpClient Updater;
        byte[] entityData = new byte[] { };

        public ServerHost()
        {
            server = true;

            players.Clear();
            ClearContent();

            Dungeon.Generate();

            players.Add(player);
        }

        void FinishInitialization()
        {
            int currentClient = 0;
            foreach (Entity p in players)
            {
                Dungeon.MoveToSpawn(p);

                if (player != p)
                {// update client for position change
                    SendPlayerData(p, Clients[currentClient++]);
                }
            }

            if (NewConnections == null)
            {
                NewConnections = UdpServer(IPAddress.Any);
                Updater = UdpServer(IPAddress.Any);

                joinListener = new Thread(new ThreadStart(ClientListener));
                joinListener.IsBackground = true;
                joinListener.Start();

                Input.lockMouse = true;
                loading = false;
            }

            initializing = false;
        }

        public override Screen Update(float deltaTime)
        {
            if (initializing && Dungeon.complete)
                FinishInitialization();

            if (!loading)
            {
                if (Input.Tapped(Keys.F5) && Dungeon.complete)
                {
                    initializing = true;
                    Dungeon.Generate();
                }

                Dungeon.Update();
                KickIdlePlayers(deltaTime);

                UpdateEntityArray();

                UpdateClients();
            }

            return base.Update(deltaTime);
        }

        // removes idle players from the list
        // and removes their clients
        void KickIdlePlayers(float deltaTime)
        {
            int clientID = 0;
            List<Entity> tempPlayers = new List<Entity>();
            List<IPEndPoint> tempClients = new List<IPEndPoint>();

            tempPlayers.Add(player);

            //loop through players
            foreach (IPEndPoint client in Clients)
            {
                Entity clientEntity = players[++clientID];

                // add time to idle time
                clientEntity.idleTime += deltaTime;

                if (clientEntity.idleTime < MAX_IDLE_TIME)
                {// add if player isn't idle
                    tempPlayers.Add(clientEntity);
                    tempClients.Add(client);

                    // update their id
                    UpdateID(clientID, client);
                }
            }

            players = tempPlayers;
            Clients = tempClients;
        }

        UdpClient UdpServer(IPAddress EndIP)
        {
            IPEndPoint EndPoint = new IPEndPoint(EndIP, PORT);

            UdpClient newClient = new UdpClient();
            newClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            /*
            byte[] inValue = new byte[] { 0 };
            byte[] outValue = new byte[] { 0 };
            newClient.Client.IOControl(SIO_UDP_CONNRESET, inValue, outValue);*/
            newClient.Client.Bind(EndPoint);
            newClient.Client.ReceiveTimeout = 1;

            return newClient;
        }

        void ResetConnecting()
        {
            ConnectingIP = new IPEndPoint(IPAddress.Any, 0);
        }

        void ClientListener()
        {
            while (!closed)
            {
                if (NewConnections.Available > 0)
                {
                    try
                    {
                        byte[] response = NewConnections.Receive(ref ConnectingIP);

                        if (!Clients.Contains(ConnectingIP))
                        {//new client is connecting
                            if (players.Count < 4)
                            {//if players arent greater than 4 then connect
                                //connect client
                                ConnectClient(players.Count);
                            }//... otherwise ignore
                        }
                        else
                        {//update
                            int ID = BitConverter.ToInt32(response, 0);

                            if (ID > 0 && ID <= Clients.Count)
                            {
                                response = response.Skip(4).ToArray();

                                if (Clients[ID - 1].Equals(ConnectingIP))
                                {
                                    //update clients entity
                                    Entity clientPlayer = players[ID];
                                    clientPlayer.CopyBytes(response);

                                    //reset idle time
                                    clientPlayer.idleTime = 0;
                                }
                            }
                            else
                            {
                                // TODO: tell client to rejoin
                            }
                        }
                    }
                    catch (SocketException)
                    {
                    }

                    // reset for new client
                    ResetConnecting();
                }
            }
        }

        void ConnectClient(int AssignedID)
        {// send world data to the player
            // move entity with player id
            Entity duplicateEntity = GrabEntity(AssignedID);

            // create a player entity for the client
            Entity clientEntity = new Entity(0, Dungeon.spawn.X, Dungeon.spawn.Y);
            Clients.Add(ConnectingIP);
            players.Add(clientEntity);

            // send data to client
            UpdateID(AssignedID, ConnectingIP);

            // send the entity data
            SendPlayerData(clientEntity, ConnectingIP);

            for(int id = 0; id < sprites.Count; id++)
                SendSprite(sprites[id], ConnectingIP);

            // send dungeon data
            NewConnections.Send(Dungeon.mapData, Dungeon.mapData.Length, ConnectingIP);
        }

        void SendSprite(TextureData sprite, IPEndPoint IP)
        {
            //sprite data
            byte[] spriteData = sprite.GetBytes();

            // protocol size + sprite size
            byte[] message = new byte[1 + spriteData.Length];

            //protocol
            message[0] = (byte)Protocol.SEND_SPRITE;
            // data
            spriteData.CopyTo(message, 1);

            //send data
            NewConnections.Send(message, message.Length, IP);
        }

        void SendPlayerData(Entity clientEntity, IPEndPoint IP)
        {
            // protocol size + entity data size
            byte[] message = new byte[1 + Entity.BYTES];
            byte[] playerData = clientEntity.GetSendableFormat();

            //protocol
            message[0] = (byte)Protocol.UPDATE_PLAYER;
            // data
            playerData.CopyTo(message, 1);

            //send data
            NewConnections.Send(message, message.Length, IP);
        }

        void UpdateID(int AssignedID, IPEndPoint IP)
        {
            // 5 is the 1 byte protocol + 4 byte id
            byte[] message = new byte[5];
            byte[] clientID = BitConverter.GetBytes(AssignedID);

            // protocol
            message[0] = (byte)Protocol.UPDATE_ID;
            // data
            clientID.CopyTo(message, 1);

            //send data
            NewConnections.Send(message, message.Length, IP);
        }

        void UpdateWorldData()
        {
            int size = 1 + (players.Count + Dungeon.entities.Count) * Entity.BYTES;
            entityData = new byte[size];
            size = 1;

            entityData[0] = (byte)Protocol.UPDATE_ENTITY_DATA;

            foreach (Entity e in entityArray)
            {
                // create sendable format for the entity
                byte[] sendableData = e.GetSendableFormat();

                // save to the message
                sendableData.CopyTo(entityData, size);

                // add to the length
                size += Entity.BYTES;
            }
        }

        void UpdateClients()
        {
            try
            {
                UpdateWorldData();

                foreach (IPEndPoint ClientIP in Clients)
                    Updater.Send(entityData, entityData.Length, ClientIP);
            }
            catch (Exception) { }
        }

        protected Entity GrabEntity(int id)
        {
            int currentID = 0;

            foreach (Entity e in players.Concat(Dungeon.entities))
                if (currentID++ == id)
                    return e;
            return null;
        }
    }
}