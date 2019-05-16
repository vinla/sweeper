using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Sweeper.Scenes
{
	public class PauseMenuScene : MenuScene
	{
		
		public PauseMenuScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
			: base(sceneManager, inputManager, contentManager)
		{			
		}

        public override void Update(GameTime gameTime)
        {
            if(MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
            base.Update(gameTime);
        }

        [MenuOption("Resume", 0)]
        public void Resume()
        {
            MediaPlayer.Resume();
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

        public override string Background => "title";

        public override Point Offset => new Point(120, 300);
    }
}
