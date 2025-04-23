using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public static class InputManager
    {
        private static MouseState mouseState = new MouseState();
        private static MouseState prevState;

        private static float moveSensitivity = 0.05f;
        private static float scrollSensitivity = 0.005f;


        public static void Update()
        {
            prevState = mouseState;
            mouseState = Mouse.GetState();
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

        public static Vector2 GetMoveAmount()
        {
            float x = (mouseState.X - prevState.X) * moveSensitivity;
            float y = (mouseState.Y - prevState.Y) * moveSensitivity;
            return new Vector2(x, y);
        }

        public static float GetScrollAmount()
        {
            return (mouseState.ScrollWheelValue - prevState.ScrollWheelValue) * scrollSensitivity;
        }
    }
}
