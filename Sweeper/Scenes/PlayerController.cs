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

        [InputAction(GameInput.TeleportSkill)]
        public void UseTeleport()
        {
            
            var teleportController = new Scenes.TeleportController(Scene);
            teleportController.Initialise();
            Scene.Controllers.Push(teleportController);
        }

        [InputAction(GameInput.MenuBack)]
        public void PauseGame()
        {
            Scene.Pause();
        }        

		private void MovePlayer(int x, int y)
		{
            var target = Scene.Player.Location.Offset(x, y);


			if (target.X < 0 || target.X >= Scene.Map.Width || target.Y < 0 || target.Y >= Scene.Map.Height)
				return;

			var targetTile = Scene.Map.GetTileAt(target);
			if (targetTile.Modifier.CanEnter == false)
				return;

			Scene.Player.MoveTo(targetTile);
			Scene.EnterTile(targetTile);
		}
	}
}
