using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.Scenes
{
	public class PauseMenuScene : MenuScene
	{
		
		public PauseMenuScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
			: base(sceneManager, inputManager, contentManager)
		{			
		}

        [MenuOption("Resume", 0)]
        public void Resume()
        {
            SceneManager.EndScene();
        }

        [MenuOption("Restart", 1)]
        public void Restart()
        {
            SceneManager.EndScene();
            SceneManager.EndScene();
            SceneManager.StartScene<MainScene>();
        }

        [MenuOption("Main Menu", 2)]
        public void Home()
        {
            SceneManager.EndScene();
            SceneManager.EndScene();
        }

        [MenuOption("Quit Game", 3)]
        public void Quit()
        {
            SceneManager.Exit();
        }		
	}
}
