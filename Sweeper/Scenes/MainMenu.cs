using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
    public class MainMenu : MenuScene
	{
		public MainMenu(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
			: base(sceneManager, inputManager, contentManager)
		{		
		}

		[MenuOption("New Game", 0)]
		public void NewGame()
		{
            SceneManager.StartScene<MainScene>();
		}

        [MenuOption("Exit", 1)]
        public void Exit()
        {
            SceneManager.Exit();
        }
	}
}
