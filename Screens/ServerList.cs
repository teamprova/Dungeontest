using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonTest
{
    class Join : Screen
    {
        List<Tool> Tools = new List<Tool>();
        Container serverlist = new Container(20, 20, 560, 282);
        TextBox ServerName = new TextBox("ServerName", 10, 20, 307);
        TextBox IPbox = new TextBox("XXX.XXX.XXX.XXX", 15, 250, 307);

        Button Back = new Button("Back", 62, 360);
        Button AddServer = new Button("Add Server", 275, 360);
        Button Remove = new Button("Remove", 500, 360);

        MouseState mouse = Mouse.GetState();
        ServerButton SelectedServer = new ServerButton("", "", 0);

        public Join()
        {
            Input.lockMouse = false;

            Tools.Add(ServerName);
            Tools.Add(IPbox);
            Tools.Add(AddServer);
            Tools.Add(Back);
            Tools.Add(Remove);

            LoadServers();
        }

        public override Screen Update(float deltaTime)
        {
            //update input
            mouse = Mouse.GetState();

            foreach(Tool tool in Tools)
                tool.Update(0);
            serverlist.Update(0);

            return checkInput();
        }

        Screen checkInput()
        {
            if (Back.Clicked)
                return new Start();

            if (Remove.Clicked)
                RemoveServer();

            if (AddServer.Clicked)
            {
                IPAddress ServerIP;

                //is the ip legit bruh
                if (IsLegitIP(IPbox.Text, out ServerIP))
                    SaveServer();
            }

            foreach (ServerButton server in serverlist.Controls)
            {
                //make server visibly selected if selected
                if (server == SelectedServer)
                    server.Selected = true;
                else if (server.Selected)
                {//select server
                    //deselect other server
                    SelectedServer.Selected = false;
                    SelectedServer = server;
                }

                if (server.DoubleClicked)
                    return JoinServer();
            }

            return this;
        }

        void RemoveServer()
        {
            //remove all of the controls from the serverlist
            serverlist.Controls.Clear();

            //move old file
            File.Move("Content/SaveData/ServerList.txt", "Content/SaveData/OldServerList.txt");

            StreamWriter writer = new StreamWriter("Content/SaveData/ServerList.txt");
            StreamReader reader = new StreamReader("Content/SaveData/OldServerList.txt");

            string serverName = reader.ReadLine();

            while (serverName != null)
            {
                string IP = reader.ReadLine();

                if (serverName != SelectedServer.Name)
                {
                    writer.WriteLine(serverName);
                    writer.WriteLine(IP);
                    
                    newServerButton(serverName, IP);
                }
                serverName = reader.ReadLine();
            }

            writer.Close();
            reader.Close();

            File.Delete("Content/SaveData/OldServerList.txt");

            SelectedServer = new ServerButton("", "", 0);
        }
        
        bool IsLegitIP(string IP, out IPAddress ServerIP)
        {
            //is the ip legit bruh
            if (IPAddress.TryParse(IP, out ServerIP) && IP != Program.getIPv4())
                return true;
            return false;
        }

        Screen JoinServer()
        {
            IPAddress ServerIP;

            if (IPAddress.TryParse(SelectedServer.IP, out ServerIP))
               return new Client(ServerIP);
            return this;
        }

        void SaveServer()
        {//adding a server to the list
            using (StreamWriter writer = new StreamWriter("Content/SaveData/ServerList.txt", true))
            {
                writer.WriteLine(ServerName.Text);
                writer.WriteLine(IPbox.Text);
                writer.Close();
            }

            newServerButton(ServerName.Text, IPbox.Text);
            ServerName.Text = "";
            IPbox.Text = "";
        }

        void LoadServers()
        {
            using (StreamReader reader = new StreamReader("Content/SaveData/ServerList.txt"))
            {
                string serverName = reader.ReadLine();

                while (serverName != null)
                {
                    string IPAddress = reader.ReadLine();

                    newServerButton(serverName, IPAddress);

                    serverName = reader.ReadLine();
                }

                reader.Close();
            }
        }

        void newServerButton(string serverName, string IPAddress)
        {
            serverlist.AddControl(new ServerButton(serverName, IPAddress, serverlist.Controls.Count * (ServerButton.HEIGHT + 10) + 10));
        }
        
        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            BeginSpriteBatch(spriteBatch);

            foreach (Tool tool in Tools)
                tool.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.End();

            serverlist.Draw(GraphicsDevice, spriteBatch);
        }
    }
}
