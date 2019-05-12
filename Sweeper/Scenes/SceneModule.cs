using Autofac;

namespace Sweeper
{
	public class SceneModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
            builder.RegisterType<Scenes.CreditsScene>();
			builder.RegisterType<MainMenu>();
			builder.RegisterType<MainScene>();
            builder.RegisterType<Scenes.PauseMenuScene>();
		}
	}
}
