using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class SceneManager : ISceneManager
	{
		private readonly Game _game;
		private readonly Stack<Scene> _sceneStack;
		private IContainer _container;

		public SceneManager(Game game)
		{
			_game = game;
			_sceneStack = new Stack<Scene>();
		}

		public Scene CurrentScene => _sceneStack.Peek();

		public void SetContainer(IContainer container)
		{			
			_container = container;
		}

		public void EndScene()
		{
			_sceneStack.Pop();
		}

		public Scene StartScene<TScene>() where TScene : Scene
		{
			var scene = _container.Resolve<TScene>();
			scene.Initialise();
			return RunScene(scene);
		}

		public Scene RunScene(Scene scene)
		{
			_sceneStack.Push(scene);
			return scene;
		}

		public void Exit()
		{
			_game.Exit();
		}
	}
}
