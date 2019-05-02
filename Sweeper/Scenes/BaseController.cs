using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sweeper
{
	public abstract class BaseController
	{		
		private Dictionary<GameInput, Action> _actions;

		protected BaseController()
		{		
			_actions = new Dictionary<GameInput, Action>();
		}		

		public void Initialise()
		{
			var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
			_actions =
				methods
					.Select(m => Tuple.Create(m.GetCustomAttribute<InputActionAttribute>(), m))
					.Where(t => t.Item1 != null)
					.ToDictionary(t => t.Item1.Input, t => new Action(() => t.Item2.Invoke(this, null)));
		}

		protected void SetAction(GameInput gameInput, Action action)
		{
			_actions.Add(gameInput, action);
		}		

		public virtual void ProcessInput(GameTime gameTime, IInputManager inputManager)
		{
			var userInput = inputManager.Test(_actions.Keys.ToArray());
			if (_actions.ContainsKey(userInput))
				_actions[userInput]();
		}

		public virtual void DrawOverlay(SpriteBatch spriteBatch)
		{
			
		}
	}

	public abstract class BaseController<TScene> : BaseController where TScene : Scene
	{
		protected BaseController(TScene scene)
		{
			Scene = scene;
		}

		public TScene Scene { get; }
	}
}
