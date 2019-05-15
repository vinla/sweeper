using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Sweeper.Scenes
{
    public class MainMenu : MenuScene
	{
		public MainMenu(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
			: base(sceneManager, inputManager, contentManager)
		{		
		}

		[MenuOption("Standard Run", 0)]
		public void NewGame()
		{
            MainScene.Optons = new RunOptions { Ramp = 1, Penalties = new[] {10, 5, 3}, GameType = "standard" };
            MainScene.Difficulty = 10;
            MainScene.Score = 0;
            MainScene.HighScore = DataManager.ReadHighScore(MainScene.Optons.GameType);
            SceneManager.StartScene<MainScene>();
		}

        [MenuOption("Flawless Run", 1)]
        public void ChallengeRun()
        {
            MainScene.Optons = new RunOptions { Ramp = 2, Penalties = new[] {100, 100, 3 }, GameType = "flawless" };
            MainScene.Difficulty = 20;
            MainScene.Score = 0;
            MainScene.HighScore = DataManager.ReadHighScore(MainScene.Optons.GameType);
            SceneManager.StartScene<MainScene>();
        }

        [MenuOption("How to play", 2)]
        public void Instructions()
        {
            SceneManager.StartScene<HowToPlayScene>();
        }

        [MenuOption("Credits", 3)]
        public void Credits()
        {
            SceneManager.StartScene<CreditsScene>();
        }

        [MenuOption("Exit", 4)]
        public void Exit()
        {
            SceneManager.Exit();
        }

        public override string Background => "title";

        public override Point Offset => new Point(120, 270);
    }
}
