using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
	public abstract class Scene
	{
		public virtual void Initialise()
		{
		}
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(GameTime gameTime, GraphicsDevice graphicsDevice);
	}
}
