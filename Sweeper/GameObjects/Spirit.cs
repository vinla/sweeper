using System;
using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class Spirit
	{
		private MainScene _scene;

		public Spirit(int x, int y, MainScene scene)
		{
			_scene = scene;
			Location = new Point(x, y);
		}

		public Point Location { get; set; }		

		public void Update(GameTime time)
		{
			if (!_scene.PlayerMoved)
				return;

			var target = _scene.Map.GetTileAt(Location);
			if (target.TileType == MapTileType.Hazard)
				return;

			var ToPlayer = Location.VectorTo(_scene.PlayerPosition);			

			if (ToPlayer.X == 0 || ToPlayer.Y == 0)
			{
				_scene.Reset();
			}
            
            if(Math.Abs(ToPlayer.X) <= Math.Abs(ToPlayer.Y))
            {
                if (ToPlayer.X > 0)
                    target = _scene.Map.GetTileAt(Location.Offset(1, 0));
                else
                    target = _scene.Map.GetTileAt(Location.Offset(-1, 0));
            }
            else
            {
                if (ToPlayer.Y > 0)
                    target = _scene.Map.GetTileAt(Location.Offset(0, 1));
                else
                    target = _scene.Map.GetTileAt(Location.Offset(0, -1));
            }

            if(target != null)
			    Location = target.Location;
		}
	}	
}
