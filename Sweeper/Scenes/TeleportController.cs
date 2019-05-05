using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.Scenes
{
    public class TeleportController : BaseController<MainScene>
    {
        private MapTile _target;

        public TeleportController(MainScene scene) : base(scene)
        {
            _target = scene.Map.GetTileAt(scene.PlayerPosition);
        }

        [InputAction(GameInput.MoveUp)]
        public void MoveUp()
        {
            MoveTarget(0, -1);
        }

        [InputAction(GameInput.MoveDown)]
        public void MoveDown()
        {
            MoveTarget(0, 1);
        }

        [InputAction(GameInput.MoveLeft)]
        public void MoveLeft()
        {
            MoveTarget(-1, 0);
        }

        [InputAction(GameInput.MoveRight)]
        public void MoveRight()
        {
            MoveTarget(1, 0);
        }

        [InputAction(GameInput.ConfirmTarget)]
        public void Teleport()
        {
            if (TargetIsClear())
            {
                Scene.SetPlayerPosition(_target.Location);
                Scene.Controllers.Pop();
                Scene.PlayerMoved = true;
                Scene.Teleports--;
            }
        }

        [InputAction(GameInput.CancelSkill)]
        public void Cancel()
        {
            Scene.Controllers.Pop();
        }

        private void MoveTarget(int x, int y)
        {
            var intendedTarget = Scene.Map.GetTileAt(_target.Location.Offset(x, y));

            if (intendedTarget == null)
                return;

            _target = intendedTarget;
        }

        private bool TargetIsClear()
        {
            return _target.TileType == MapTileType.Empty && _target.Adjacents.HasValue;
        }

        public override void DrawOverlay(SpriteBatch spriteBatch)
        {
            var overlayColor = TargetIsClear() ? Color.Green : Color.Red;
            var screenPosition = new Vector2(_target.Location.X * 48, _target.Location.Y * 48);
            var overlayGraphic = spriteBatch.GraphicsDevice.CreateRectangeTexture(48, 48, 4, overlayColor, Color.Transparent);
            spriteBatch.Draw(overlayGraphic, screenPosition, Color.White);            
        }
    }
}
