using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeontest
{
    class Start : Screen
    {
        Button NewGame = new Button("Start Game", 300, 125);
        Button HostGame = new Button("Host Game", 300, 170);
        Button ServerList = new Button("Join Server", 300, 215);
        Button Exit = new Button("Leave Game", 300, 260);
        List<Tool> Tools = new List<Tool>();
        MouseState mouse = Mouse.GetState();

        public Start()
        {
            Input.lockMouse = false;

            Tools.Add(NewGame);
            Tools.Add(HostGame);
            Tools.Add(ServerList);
            Tools.Add(Exit);
        }

        public override Screen Update(float deltaTime)
        {
            mouse = Mouse.GetState();

            foreach (Tool tool in Tools)
                tool.Update(0);

            if (NewGame.Clicked)
                return new SinglePlayer();
            if (HostGame.Clicked)
                return new ServerHost();
            if (ServerList.Clicked)
                return new Join();
            if (Exit.Clicked)
                RequestExit = true;
            return this;
        }
        
        public override void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            BeginSpriteBatch(spriteBatch);

            foreach (Tool tool in Tools)
                tool.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.End();
        }
    }
}
