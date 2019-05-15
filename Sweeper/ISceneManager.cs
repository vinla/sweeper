namespace Sweeper
{
	public interface ISceneManager
	{
		Scene StartScene<TScene>() where TScene : Scene;
		Scene CurrentScene { get; }
        Scene RunScene(Scene scene);
		void EndScene();
		void Exit();
	}
}
