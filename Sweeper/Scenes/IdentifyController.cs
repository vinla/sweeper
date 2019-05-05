using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.Scenes
{
    public abstract class SkillController : BaseController<MainScene>
    {
        public SkillController(MainScene scene) : base(scene)
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
            var target = Scene.Map.GetTileAt(Scene.PlayerPosition.Offset(direction));
            if (IsValid(target))
            {
                Action(target);
                Scene.Controllers.Pop();
            }
        }

        public override void DrawOverlay(SpriteBatch spriteBatch)
        {
            var origin = Scene.PlayerPosition;
            var overlay = spriteBatch.GraphicsDevice.CreateRectangeTexture(48, 48, 4, Color.White, Color.Transparent);

            foreach(var point in Direction.CompassPoints)
            {
                var target = Scene.Map.GetTileAt(origin.Offset(point));
                if(target != null)
                {
                    var color = IsValid(target) ? Color.Red : Color.Blue;
                    spriteBatch.Draw(overlay, new Vector2(target.Location.X * 48, target.Location.Y * 48), color);
                }
            }            
        }

        protected abstract bool IsValid(MapTile mapTile);

        protected abstract void Action(MapTile mapTile);
    }

    public class IdentifyController : SkillController
    {
        public IdentifyController(MainScene scene) : base(scene) { }
        protected override void Action(MapTile mapTile)
        {
            Scene.ResolveTile(mapTile);
        }

        protected override bool IsValid(MapTile mapTile)
        {
            return mapTile.Adjacents.HasValue == false;
        }
    }    

    public static class Direction
    {
        public static Point Up => new Point(0, -1);

        public static Point Down => new Point(0, 1);

        public static Point Left => new Point(-1, 0);

        public static Point Right => new Point(1, 0);

        public static Point[] CompassPoints => new[] { Up, Right, Down, Left };
    }    
}
