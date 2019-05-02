using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.Scenes
{
	public class PushController : BaseController<MainScene>
	{
		private readonly List<Point> _affectedTiles;
		private Point _direction;
		private int _power;

		public PushController(MainScene scene) : base(scene)
		{
			_direction = new Point(0, -1);
			_affectedTiles = new List<Point>();
			_power = 1;
		}

		public override void ProcessInput(GameTime gameTime, IInputManager inputManager)
		{
			base.ProcessInput(gameTime, inputManager);
			CalculateOverlay();
		}

		[InputAction(GameInput.MoveUp)]
		public void TargetUp()
		{
			_direction = new Point(0, -1);
		}

		[InputAction(GameInput.MoveDown)]
		public void TargetDown()
		{
			_direction = new Point(0, 1);
		}

		[InputAction(GameInput.MoveLeft)]
		public void TargetLeft()
		{
			_direction = new Point(-1, 0);
		}

		[InputAction(GameInput.MoveRight)]
		public void TargetRight()
		{
			_direction = new Point(1, 0);
		}

		[InputAction(GameInput.CancelCast)]
		public void CancelCast()
		{
			Scene.Controllers.Pop();
		}

		[InputAction(GameInput.PowerUp)]
		public void PowerUp()
		{
			_power++;
		}

		[InputAction(GameInput.PowerDown)]
		public void PowerDown()
		{
			if(_power > 1)
				_power--;
		}

		[InputAction(GameInput.ConfirmCast)]
		public void ConfirmCast()
		{
			// Find target
			// Push target
			// Check conditions

			foreach(var targetTile in _affectedTiles)
			{
				// Is there a spirit in the tile?
				var spirit = Scene.Spirits.SingleOrDefault(sp => sp.Location == targetTile);
				if (spirit != null)
				{					
					// TODO: Implement proper push that checks for collision etcs
					spirit.Location = spirit.Location.Offset(_direction.Times(_power));
					break; 
				}
			}

			Scene.Controllers.Pop();
		}

		public override void DrawOverlay(SpriteBatch spriteBatch)
		{
			var overlaySprite = spriteBatch.GraphicsDevice.CreateRectangeTexture(48, 48, 4, Color.Red, Color.Transparent);
			foreach (var tile in _affectedTiles)
				spriteBatch.Draw(overlaySprite, new Vector2(tile.X * 48, tile.Y * 48), Color.White);
		}

		private void CalculateOverlay()
		{
			_affectedTiles.Clear();			
			_affectedTiles.Add(Scene.PlayerPosition);

			var pathEnded = false;

			while(!pathEnded)
			{
				var nextPoint = _affectedTiles.Last().Offset(_direction);
				var nextTile = Scene.Map.GetTileAt(nextPoint);
				if (nextTile != null && CanTraverse(nextTile))
					_affectedTiles.Add(nextPoint);
				else
					pathEnded = true;
			}
		}

		private bool CanTraverse(MapTile mapTile)
		{
			return mapTile.TileType == MapTileType.Empty && mapTile.Adjacents.HasValue;
		}
	}
}
