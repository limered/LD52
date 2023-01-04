using UnityEngine;

namespace SystemBase.Utils
{
    public static class InputExtensions
    {
        public static bool IsPressed(this string inputName)
        {
            return Input.GetButton(inputName);
        }

        public static bool WasPressed(this string inputName)
        {
            return Input.GetButtonDown(inputName);
        }

        public static bool WasReleased(this string inputName)
        {
            return Input.GetButtonUp(inputName);
        }

        public static bool IsPressed(this KeyCode key)
        {
            return Input.GetKey(key);
        }

        public static bool WasPressed(this KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public static bool WasReleased(this KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public static float Axis(this string inputName)
        {
            return Input.GetAxis(inputName);
        }
    }
}