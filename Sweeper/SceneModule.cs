using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Sweeper
{
	public class SceneModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MainMenu>();
			builder.RegisterType<Scene2>();
		}
	}
}
