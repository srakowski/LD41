using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PixelVisionRunner;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using System.Collections.Generic;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Buttons = PixelVisionSDK.Chips.Buttons;
using ButtonState = PixelVisionSDK.ButtonState;

namespace LD41.PV8SDK
{
    class InputFactory : IInputFactory
    {
        private Dictionary<Buttons, Keys>[] keyBindings = new Dictionary<Buttons, Keys>[]
        {
            new Dictionary<Buttons, Keys>
            {
                { Buttons.Up, Keys.Up },
                { Buttons.Down, Keys.Down },
                { Buttons.Left, Keys.Left },
                { Buttons.Right, Keys.Right },
                { Buttons.A, Keys.X },
                { Buttons.B, Keys.C },
                { Buttons.Start, Keys.A },
                { Buttons.Select, Keys.S },
            },
            new Dictionary<Buttons, Keys>
            {
                { Buttons.Up, Keys.I },
                { Buttons.Down, Keys.K },
                { Buttons.Left, Keys.J },
                { Buttons.Right, Keys.L },
                { Buttons.A, Keys.OemQuotes },
                { Buttons.B, Keys.Enter },
                { Buttons.Start, Keys.Y },
                { Buttons.Select, Keys.U },
            },
        };
        
        public InputFactory()
        {
        }

        public ButtonState CreateButtonBinding(int playerIdx, Buttons button)
        {
            return new KeyboardButtonInput(button, (int)keyBindings[playerIdx][button]);
        }

        public IKeyInput CreateKeyInput()
        {
            return new KeyInput();
        }

        public IMouseInput CreateMouseInput()
        {
            return null;
        }
    }

    public class KeyboardButtonInput : ButtonState
    {
        protected int keyCode;

        public KeyboardButtonInput(Buttons buttons, int keyCode)
        {
            this.buttons = buttons;
            mapping = keyCode;
            this.keyCode = keyCode;
        }

        public override void Update(float timeDelta)
        {
            value = InputStates.CurrKeyboardState.IsKeyDown((Keys)keyCode);
            base.Update(timeDelta);
        }

    }

    static class InputStates
    {
        private static string _input = "";

        public static string TextInput
        {
            get
            {
                var curr = _input;
                _input = "";
                return curr;
            }
        }

        public static MouseState PrevMouseState { get; private set; }
        public static MouseState CurrMouseState { get; private set; }

        public static KeyboardState PrevKeyboardState { get; private set; }
        public static KeyboardState CurrKeyboardState { get; private set; }

        public static void Update()
        {
            PrevMouseState = CurrMouseState;
            CurrMouseState = Mouse.GetState();

            PrevKeyboardState = CurrKeyboardState;
            CurrKeyboardState = Keyboard.GetState();
        }

        public static void InputStates_TextInput(object sender, TextInputEventArgs e)
        {
            _input += e.Character;
        }
    }

    public class KeyInput : IKeyInput
    {
        public bool GetKey(int key)
        {
            return InputStates.CurrKeyboardState.IsKeyDown((Keys)key) &&
                InputStates.PrevKeyboardState.IsKeyUp((Keys)key);
        }

        public bool GetKeyDown(int key)
        {
            return InputStates.CurrKeyboardState.IsKeyDown((Keys)key);
        }

        public bool GetKeyUp(int key)
        {
            return InputStates.CurrKeyboardState.IsKeyUp((Keys)key);
        }

        public string ReadInputString()
        {
            return InputStates.TextInput;
        }
    }
}
