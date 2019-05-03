using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
    public class MainMenu : MenuScene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
		private readonly ContentManager _contentManager;
		private readonly List<string> _menuOptions;
		private int _selectedOption;
		private SpriteFont _menuFont;

		public MainMenu(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
			: base(sceneManager, inputManager, contentManager)
		{		
		}

		[MenuOption("New Game")]
		public void NewGame()
		{

		}
	}
}
