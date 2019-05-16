﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
	public abstract class MenuScene : Scene
	{		
		private readonly IInputManager _inputManager;
		private List<Tuple<string, Action>> _menuOptions;
		private int _selectedOption;
		private SpriteFont _menuFont;
        private Texture2D _background;

		public MenuScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			SceneManager = sceneManager;
			_inputManager = inputManager;
			ContentManager = contentManager;			
			_selectedOption = 0;
            _menuOptions = new List<Tuple<string, Action>>();
		}

		public ISceneManager SceneManager { get; }

        public ContentManager ContentManager { get; }

		public override void Initialise()
		{
			_menuFont = ContentManager.Load<SpriteFont>("MainMenu");
            _background = ContentManager.Load<Texture2D>(Background);
            var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            _menuOptions =
                methods
                    .Select(m => Tuple.Create(m.GetCustomAttribute<MenuOptionAttribute>(), m))
                    .Where(t => t.Item1 != null)
                    .OrderBy(t => t.Item1.Index)
                    .Select(t => Tuple.Create(t.Item1.Text, new Action(() => t.Item2.Invoke(this, null))))
                    .ToList();
        }

		public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Black);
			using (var spriteBatch = new SpriteBatch(graphicsDevice))
			{
				spriteBatch.Begin();
                spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
				for (int i = 0; i < _menuOptions.Count; i++)
				{
					spriteBatch.DrawString(
						_menuFont,
						_menuOptions[i].Item1,
						new Vector2(Offset.X, Offset.Y + (100 * i)),
						i == _selectedOption ? Color.Yellow : Color.White);
				}
				spriteBatch.End();
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (_inputManager.WasInput(GameInput.MenuUp))
				_selectedOption = _selectedOption.Decrement(0, _menuOptions.Count - 1);
			if (_inputManager.WasInput(GameInput.MenuDown))
				_selectedOption = _selectedOption.Increment(0, _menuOptions.Count - 1);
			if (_inputManager.WasInput(GameInput.MenuSelect))
				ExecuteOption(_selectedOption);
		}

		private void ExecuteOption(int optionIndex)
		{
            _menuOptions[optionIndex].Item2();
		}

        public abstract string Background { get; }

        public abstract Point Offset { get; }
	}

	public class MenuOptionAttribute : Attribute
	{
		public MenuOptionAttribute(string text, int index)
		{
			Text = text;
            Index = index;
		}
		public string Text { get; }

        public int Index { get;  }
	}
}
