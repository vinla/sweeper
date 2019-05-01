﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
    public class MainMenu : Scene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
		private readonly ContentManager _contentManager;
		private readonly List<string> _menuOptions;
		private int _selectedOption;
		private SpriteFont _menuFont;

		public MainMenu(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
			_contentManager = contentManager;
			_menuOptions = new List<string> { "New Game", "Load Game", "Options", "Exit" };
			_selectedOption = 0;
		}

		public override void Initialise()
		{            
            _menuFont = _contentManager.Load<SpriteFont>("MainMenu");
		}

		public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.DarkRed);
			using (var spriteBatch = new SpriteBatch(graphicsDevice))
			{
				spriteBatch.Begin();
				for (int i = 0; i < _menuOptions.Count; i++ )
				{
					spriteBatch.DrawString(
						_menuFont, 
						_menuOptions[i], 
						new Vector2(100, 50 + (100 * i)), 
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
			switch(optionIndex)
			{
				case 0:
					_sceneManager.StartScene<MainScene>();
					break;
				case 3:
					_sceneManager.Exit();
					break;
			}
		}
	}
}