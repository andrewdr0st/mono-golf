using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public static class InputManager
    {
        private static MouseState mouseState = new MouseState();
        private static MouseState prevState;
        private static KeyboardState keyboardState = new KeyboardState();

        private static float moveSensitivity = 0.05f;
        private static float scrollSensitivity = 0.005f;

        public static Vector2 MoveVector { get; private set; }

        public static void Update()
        {
            prevState = mouseState;
            mouseState = Mouse.GetState();

            keyboardState = Keyboard.GetState();
            float up = keyboardState.IsKeyDown(Keys.W) ? 1f : 0f;
            float down = keyboardState.IsKeyDown(Keys.S) ? -1f : 0f;
            float left = keyboardState.IsKeyDown(Keys.A) ? -1f : 0f;
            float right = keyboardState.IsKeyDown(Keys.D) ? 1f : 0f;
            MoveVector = new Vector2(left + right, up + down);
        }

        public static bool LeftPressed()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool LeftCLick()
        {
            return mouseState.LeftButton != ButtonState.Pressed && prevState.LeftButton == ButtonState.Pressed;
        }

        public static bool RightPressed()
        {
            return mouseState.RightButton == ButtonState.Pressed;
        }

        public static bool MiddlePressed()
        {
            return mouseState.MiddleButton == ButtonState.Pressed;
        }

        public static Vector2 GetMoveAmount()
        {
            float x = (mouseState.X - prevState.X) * moveSensitivity;
            float y = (mouseState.Y - prevState.Y) * moveSensitivity;
            return new Vector2(x, y);
        }

        public static Vector2 MouseCoords()
        {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public static float GetScrollAmount()
        {
            return (mouseState.ScrollWheelValue - prevState.ScrollWheelValue) * scrollSensitivity;
        }
    }
}
