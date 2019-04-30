using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sweeper
{
	public class MainMenu : Scene
	{
		private readonly ISceneManager _sceneManager;
		private readonly ContentManager _contentManager;
		private readonly List<string> _menuOptions;
		private int _selectedOption;
		private SpriteFont _menuFont;

		public MainMenu(ISceneManager sceneManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
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
						new Vector2(100, 100 * i), 
						i == _selectedOption ? Color.Yellow : Color.White);
				}				
				spriteBatch.End();
			}
		}

		public override void Update(GameTime gameTime)
		{
			var keys = Keyboard.GetState().GetPressedKeys();

			if (keys.Contains(Keys.Up))
				_selectedOption = _selectedOption.Decrement(0, _menuOptions.Count - 1);
			if (keys.Contains(Keys.Down))
				_selectedOption = _selectedOption.Increment(0, _menuOptions.Count - 1);
			if (keys.Contains(Keys.Enter))
				ExecuteOption(_selectedOption);
		}

		private void ExecuteOption(int optionIndex)
		{
			switch(optionIndex)
			{
				case 0:
					_sceneManager.StartScene<Scene2>();
					break;
				case 3:
					_sceneManager.Exit();
					break;
			}
		}
	}

	public static class IntegerExtensions
	{
		public static int Decrement(this int i, int min, int max)
		{
			int returnValue = i - 1;
			return returnValue < min ? max : returnValue;
		}

		public static int Increment(this int i, int min, int max)
		{
			int returnValue = i + 1;
			return returnValue > max ? min : returnValue;
		}
	}

	public class Scene2 : Scene
	{
		private readonly ISceneManager _sceneManager;

		public Scene2(ISceneManager sceneManager)
		{
			_sceneManager = sceneManager;
		}

		public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Olive);
		}

		public override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Q))
				_sceneManager.EndScene();
		}
	}
}
