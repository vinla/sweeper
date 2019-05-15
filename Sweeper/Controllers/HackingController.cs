using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sweeper.Scenes;

namespace Sweeper.Controllers
{
	public class HackingController : BaseController<MainScene>
    {
        public HackingController(MainScene scene) : base(scene)
        {
        }

        [InputAction(GameInput.MoveUp)]
        public void TargetUp()
        {
            Execute(Direction.Up);
        }

        [InputAction(GameInput.MoveDown)]
        public void TargetDown()
        {
            Execute(Direction.Down);
        }

        [InputAction(GameInput.MoveLeft)]
        public void TargetLeft()
        {
            Execute(Direction.Left);
        }

        [InputAction(GameInput.MoveRight)]
        public void TargetRight()
        {
            Execute(Direction.Right);
        }

        [InputAction(GameInput.CancelSkill)]
        public void Cancel()
        {
            Scene.Controllers.Pop();
        }

        private void Execute(Point direction)
        {
            var target = Scene.Map.GetTileAt(Scene.Player.Location.Offset(direction));
            if (IsValid(target))
            {
				Scene.Hack(target);
				Scene.Controllers.Pop();
            }
        }

		public override void DrawOverlay(SpriteBatch spriteBatch)
		{
			var origin = Scene.Player.Location;
			var overlay = spriteBatch.GraphicsDevice.CreateRectangeTexture(48, 48, 4, Color.White, Color.Transparent);

			foreach (var point in Direction.CompassPoints)
			{
				var target = Scene.Map.GetTileAt(origin.Offset(point));
				if (target != null)
				{
					var color = IsValid(target) ? Color.Red : Color.Blue;
					spriteBatch.Draw(overlay, new Vector2(target.Location.X * 48, target.Location.Y * 48), color);
				}
			}
		}

        private bool IsValid(MapTile mapTile)
        {
            return mapTile != null && mapTile.Discovered == false;
        }
    }    
}
