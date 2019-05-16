using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sweeper
{
    public class InputManager : IInputManager
    {
        private readonly ControllerInputManager _controller;
        private readonly KeyBoardInputManager _keyboard;

        public InputManager()
        {
            _controller = new ControllerInputManager();
            _keyboard = new KeyBoardInputManager();
        }

        public void EarlyUpdate(GameTime gameTime)
        {
            _controller.EarlyUpdate(gameTime);
            _keyboard.EarlyUpdate(gameTime);
        }

        public void LateUpdate(GameTime gameTime)
        {
            _controller.LateUpdate(gameTime);
            _keyboard.LateUpdate(gameTime);
        }

        public GameInput Test(params GameInput[] inputs)
        {
            var test = _controller.Test(inputs);
            if (test == GameInput.None)
                test = _keyboard.Test(inputs);
            return test;
        }

        public bool WasInput(GameInput input)
        {
            return _controller.WasInput(input) || _keyboard.WasInput(input);
        }
    }

    public class ControllerInputManager : IInputManager
    {
        private readonly List<Tuple<Buttons, GameInput>> _bindings;
        private GamePadState _previousState;
        private GamePadState _currentState;

        public ControllerInputManager()
        {
            _bindings = new List<Tuple<Buttons, GameInput>>();
            _bindings.Add(Tuple.Create(Buttons.DPadUp, GameInput.MenuUp));
            _bindings.Add(Tuple.Create(Buttons.DPadDown, GameInput.MenuDown));
            _bindings.Add(Tuple.Create(Buttons.A, GameInput.MenuSelect));
            _bindings.Add(Tuple.Create(Buttons.B, GameInput.MenuBack));
            _bindings.Add(Tuple.Create(Buttons.DPadLeft, GameInput.MoveLeft));
            _bindings.Add(Tuple.Create(Buttons.DPadRight, GameInput.MoveRight));
            _bindings.Add(Tuple.Create(Buttons.DPadUp, GameInput.MoveUp));
            _bindings.Add(Tuple.Create(Buttons.DPadDown, GameInput.MoveDown));
            _bindings.Add(Tuple.Create(Buttons.A, GameInput.IdentifySkill));
            _bindings.Add(Tuple.Create(Buttons.B, GameInput.CancelSkill));
        }

        public void EarlyUpdate(GameTime gameTime)
        {            
            _currentState = GamePad.GetState(PlayerIndex.One);
        }

        public void LateUpdate(GameTime gameTime)
        {
            _previousState = GamePad.GetState(PlayerIndex.One);
        }

        public bool WasInput(GameInput input)
        {
            if (_currentState.IsConnected == false)
                return false;

            var buttons = _bindings.Where(kvp => kvp.Item2 == input).Select(kvp => kvp.Item1);
            foreach(var button in buttons)
            {
                if (_currentState.IsButtonDown(button) && _previousState.IsButtonUp(button))
                    return true;
            }

            return false;
        }

        public GameInput Test(params GameInput[] inputs)
        {
            if (_currentState.IsConnected)
            {
                foreach (var input in inputs)
                    if (WasInput(input))
                        return input;
            }

            return GameInput.None;
        }
    }

    public class KeyBoardInputManager : IInputManager
    {
        private readonly List<Tuple<Keys, GameInput>> _keyBindings;
        private KeyboardState _previousState;
        private KeyboardState _currentState;        

        public KeyBoardInputManager()
        {
            _keyBindings = new List<Tuple<Keys, GameInput>>();
            _keyBindings.Add(Tuple.Create(Keys.Up, GameInput.MenuUp));
            _keyBindings.Add(Tuple.Create(Keys.Down, GameInput.MenuDown));
            _keyBindings.Add(Tuple.Create(Keys.Enter, GameInput.MenuSelect));
            _keyBindings.Add(Tuple.Create(Keys.Space, GameInput.MenuSelect));
            _keyBindings.Add(Tuple.Create(Keys.Escape, GameInput.MenuBack));
            _keyBindings.Add(Tuple.Create(Keys.A, GameInput.MoveLeft));
            _keyBindings.Add(Tuple.Create(Keys.D, GameInput.MoveRight));
            _keyBindings.Add(Tuple.Create(Keys.W, GameInput.MoveUp));
            _keyBindings.Add(Tuple.Create(Keys.S, GameInput.MoveDown));
            _keyBindings.Add(Tuple.Create(Keys.Left, GameInput.MoveLeft));
            _keyBindings.Add(Tuple.Create(Keys.Right, GameInput.MoveRight));
            _keyBindings.Add(Tuple.Create(Keys.Up, GameInput.MoveUp));
            _keyBindings.Add(Tuple.Create(Keys.Down, GameInput.MoveDown));
			_keyBindings.Add(Tuple.Create(Keys.D1, GameInput.IdentifySkill));
            _keyBindings.Add(Tuple.Create(Keys.M, GameInput.ToggleMusic));
            _keyBindings.Add(Tuple.Create(Keys.N, GameInput.SkipTrack));
            _keyBindings.Add(Tuple.Create(Keys.LeftShift, GameInput.IdentifySkill));
            _keyBindings.Add(Tuple.Create(Keys.D2, GameInput.TeleportSkill));
            _keyBindings.Add(Tuple.Create(Keys.Escape, GameInput.CancelSkill));
            _keyBindings.Add(Tuple.Create(Keys.Enter, GameInput.ConfirmTarget));
            _keyBindings.Add(Tuple.Create(Keys.Space, GameInput.ConfirmTarget));
        }

        public void EarlyUpdate(GameTime time)
        {
            _currentState = Keyboard.GetState();
        }

        public void LateUpdate(GameTime time)
        {
            _previousState = _currentState;
        }

        public bool WasInput(GameInput input)
        {
            var keys = _keyBindings.Where(kvp => kvp.Item2 == input).Select(kvp => kvp.Item1);
            var keysJustPressed = _currentState.GetPressedKeys();
            if (_previousState != null)
                keysJustPressed = keysJustPressed.Except(_previousState.GetPressedKeys()).ToArray();
            return keysJustPressed.Intersect(keys).Any();
        }

		public GameInput Test(params GameInput[] inputs)
		{
			foreach (var input in inputs)
				if (WasInput(input))
					return input;
			return GameInput.None;
		}
    }

    public interface IInputManager
    {
        bool WasInput(GameInput input);
		GameInput Test(params GameInput[] inputs);
    }

    public enum GameInput
    {
		None,
        MenuBack,
        MenuSelect,
        MenuUp,    
        MenuDown,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
		IdentifySkill,
        TeleportSkill,
		CancelSkill,
        ConfirmTarget,
        ToggleMusic,
        SkipTrack
    }
}
