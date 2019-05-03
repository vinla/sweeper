using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class PlayerController : BaseController<MainScene>
	{		
		public PlayerController(MainScene scene) : base(scene)
		{
		}

		[InputAction(GameInput.MoveUp)]
		public void MoveUp()
		{
			MovePlayer(0, -1);
		}

		[InputAction(GameInput.MoveDown)]
		public void MoveDown()
		{
			MovePlayer(0, 1);
		}

		[InputAction(GameInput.MoveLeft)]
		public void MoveLeft()
		{
			MovePlayer(-1, 0);
		}

		[InputAction(GameInput.MoveRight)]
		public void MoveRight()
		{
			MovePlayer(1, 0);
		}

		[InputAction(GameInput.IdentifySkill)]
		public void UseIndentify()
		{
			var idController = new Scenes.IdentifyController(Scene);
			idController.Initialise();
			Scene.Controllers.Push(idController);
		}

        [InputAction(GameInput.DisarmSkill)]
        public void UseDisarm()
        {
            var disarmController = new Scenes.DisarmController(Scene);
            disarmController.Initialise();
            Scene.Controllers.Push(disarmController);
        }

        [InputAction(GameInput.MenuBack)]
        public void PauseGame()
        {
            Scene.Pause();
        }        

		private void MovePlayer(int x, int y)
		{
			var target = new Point(Scene.PlayerPosition.X + x, Scene.PlayerPosition.Y + y);


			if (target.X < 0 || target.X >= Scene.Map.Width || target.Y < 0 || target.Y >= Scene.Map.Height)
				return;

			var targetTile = Scene.Map.GetTileAt(target);
			if (targetTile.TileType == MapTileType.Blocked)
				return;

			Scene.SetPlayerPosition(target);
			Scene.ResolveTile(targetTile);
			Scene.PlayerMoved = true;
		}
	}
}
