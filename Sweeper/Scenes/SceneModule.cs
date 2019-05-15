using Autofac;

namespace Sweeper.Scenes
{
	public class SceneModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
            builder.RegisterType<CreditsScene>();
			builder.RegisterType<MainMenu>();
			builder.RegisterType<MainScene>();
            builder.RegisterType<PauseMenuScene>();
            builder.RegisterType<HowToPlayScene>();
		}
	}
}
