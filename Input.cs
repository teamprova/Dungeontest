using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonTest
{
    public static class Input
    {
        static GamePadState oldController = GamePad.GetState(PlayerIndex.One);
        static GamePadState newController = GamePad.GetState(PlayerIndex.One);
        static KeyboardState oldKeyboard = Keyboard.GetState();
        static KeyboardState newKeyboard = Keyboard.GetState();
        static MouseState oldMouse = Mouse.GetState();
        static MouseState newMouse = Mouse.GetState();
        public static Vector2 mouse = Vector2.Zero;
        public static Vector2 movement = Vector2.Zero;
        public static Vector2 deltaMouse = Vector2.Zero;
        public static int deltaWheel = 0;
        public static bool lockMouse = false;
        public static bool leftClick = false;

        public static bool Held(Keys key)
        {
            return newKeyboard.IsKeyDown(key);
        }

        public static bool Tapped(Keys key)
        {
            return newKeyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key);
        }

        public static bool Held(Buttons button)
        {
            if (button == Buttons.LeftTrigger || button == Buttons.RightTrigger)
                return TriggerDown(newController, button);

            return newController.IsButtonDown(button);
        }

        public static bool Tapped(Buttons button)
        {
            if (button == Buttons.LeftTrigger || button == Buttons.RightTrigger)
                return TriggerDown(newController, button) && !TriggerDown(oldController, button);

            return newController.IsButtonDown(button) && oldController.IsButtonUp(button);
        }

        static bool TriggerDown(GamePadState controller, Buttons button)
        {
            float trigger = (button == Buttons.LeftTrigger) ? controller.Triggers.Left : controller.Triggers.Right;

            return trigger > .8f;
        }

        public static void Update(Rectangle window)
        {
            movement = Vector2.Zero;
            Screen.window = window;
            Screen.scale.X = Screen.resolution.X / (float)window.Width;
            Screen.scale.Y = Screen.resolution.Y / (float)window.Height;

            UpdateKeyboard();
            UpdateMouse(window);
            UpdateController();
        }

        private static void UpdateController()
        {
            oldController = newController;
            newController = GamePad.GetState(PlayerIndex.One);

            GamePadThumbSticks ThumbStick = newController.ThumbSticks;
            
            Vector2 newMovement = ThumbStick.Left;
            Vector2 newDeltaMouse = ThumbStick.Right * 12;

            if (newMovement.Length() > movement.Length())
                movement = newMovement;
            
            if (newDeltaMouse.Length() > deltaMouse.Length())
                deltaMouse = newDeltaMouse;

            leftClick |= Held(Buttons.LeftTrigger);
        }

        public static void UpdateMouse(Rectangle window)
        {
            oldMouse = newMouse;
            newMouse = Mouse.GetState();

            mouse.X = newMouse.X;
            mouse.Y = newMouse.Y;
            deltaWheel = (newMouse.ScrollWheelValue - oldMouse.ScrollWheelValue) / 40;

            deltaMouse = mouse - new Vector2(window.Center.X, window.Center.Y);

            mouse.X *= Screen.scale.X;
            mouse.Y *= Screen.scale.Y;

            leftClick = newMouse.LeftButton == ButtonState.Pressed;

            if (lockMouse)
                SetMouse(window.Center.X, window.Center.Y);
        }

        static void SetMouse(int x, int y)
        {
            try
            {
                Mouse.SetPosition(x, y);
            }
            catch (Exception e)
            { }
        }

        public static void UpdateKeyboard()
        {
            oldKeyboard = newKeyboard;
            newKeyboard = Keyboard.GetState();

            if (newKeyboard.IsKeyDown(Keys.A))
                movement.X = -1;
            if (newKeyboard.IsKeyDown(Keys.D))
                movement.X = 1;
            if (newKeyboard.IsKeyDown(Keys.W))
                movement.Y = -1;
            if (newKeyboard.IsKeyDown(Keys.S))
                movement.Y = 1;

            if(movement != Vector2.Zero)
                movement.Normalize();
        }

        public static Keys[] GetPressedKeys()
        {
            return newKeyboard.GetPressedKeys();
        }
    }
}
