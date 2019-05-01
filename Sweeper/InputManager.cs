﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sweeper
{
    public class InputManager : IInputManager
    {
        private readonly List<Tuple<Keys, GameInput>> _keyBindings;
        private KeyboardState _previousState;
        private KeyboardState _currentState;        

        public InputManager()
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
    }

    public interface IInputManager
    {
        bool WasInput(GameInput input);
    }

    public enum GameInput
    {
        MenuBack,
        MenuSelect,
        MenuUp,    
        MenuDown,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight
    }
}