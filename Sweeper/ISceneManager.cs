using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeper
{
	public interface ISceneManager
	{
		Scene StartScene<TScene>() where TScene : Scene;
		Scene CurrentScene { get; }
		void EndScene();
		void Exit();
	}
}
