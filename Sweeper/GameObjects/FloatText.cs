using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.GameObjects
{
	public class FloatText
	{
		private readonly string _text;
		private readonly Color _color;
		private readonly Vector2 _velocity;
		private readonly SpriteFont _font;
		private readonly float _originalTtl;

		private Vector2 _location;
		private float _ttl;

		public FloatText(string text, Color color, SpriteFont font, Vector2 location, Vector2 velocity, float ttl)
		{
			_text = text;
			_color = color;
			_font = font;
			_location = location;
			_velocity = velocity;
			_originalTtl = _ttl = ttl;
		}


		public void Update(GameTime gameTime)
		{
			if (IsActive)
			{
				_location += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
				_ttl -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (IsActive)
			{
				var color = new Color(_color, System.Math.Max(1, (_ttl / _originalTtl) + .25f));
				spriteBatch.DrawString(_font, _text, _location, color);
			}
		}

		public bool IsActive => _ttl > 0;
	}
}
