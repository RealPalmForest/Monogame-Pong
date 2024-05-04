using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pongage.Managers
{
    public static class InputManager
    {
        public static float PlayerOneVerticalVelocity;
        public static float PlayerTwoVerticalVelocity;

        public static void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            PlayerOneVerticalVelocity = 0;
            PlayerTwoVerticalVelocity = 0;

            if(keyboardState.IsKeyDown(Keys.W)) PlayerOneVerticalVelocity--;
            if (keyboardState.IsKeyDown(Keys.S)) PlayerOneVerticalVelocity++;

            if (keyboardState.IsKeyDown(Keys.Up)) PlayerTwoVerticalVelocity--;
            if (keyboardState.IsKeyDown(Keys.Down)) PlayerTwoVerticalVelocity++;
        }
    }
}
