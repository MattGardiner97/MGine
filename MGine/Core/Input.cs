using SharpDX.Windows;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGine.Interfaces;

namespace MGine.Core
{
    public class Input : IService
    {
        private Engine engine;

        private DirectInput input;
        private Keyboard keyboard;

        private bool[] lastFrameState;
        private bool[] currentFrameState;

        public Input (Engine Engine)
        {
            engine = Engine;
        }

        public void Init()
        {
            input = new DirectInput();
            var x = input.GetDevices(DeviceClass.Keyboard, DeviceEnumerationFlags.AllDevices);
            keyboard = new Keyboard(input);
            keyboard.Acquire();

            var allKeys = keyboard.GetCurrentState().AllKeys;
            lastFrameState = new bool[256];
            currentFrameState = new bool[256];
        }
        

        public void EarlyUpdate()
        {
            var x = keyboard.GetCurrentState();
            foreach (var key in x.PressedKeys)
                currentFrameState[(int)key] = true;
        }

        public void LateUpdate()
        {
            var temp = lastFrameState;
            lastFrameState = currentFrameState;
            currentFrameState = temp;

            for (int i = 0; i < currentFrameState.Length; i++)
                currentFrameState[i] = false;
        }

        public void Dispose()
        {
            input.Dispose();
            keyboard.Dispose();
        }

        public bool GetKey(Key Key)
        {
            return currentFrameState[(int)Key];
        }

        public bool GetKeyDown(Key Key)
        {
             return currentFrameState[(int)Key] && lastFrameState[(int)Key] == false;
        }

        public bool GetKeyUp(Key Key)
        {
            return lastFrameState[(int)Key] && currentFrameState[(int)Key] == false;


        }

    }
}
