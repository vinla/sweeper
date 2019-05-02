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

			var adjacentTiles = _scene.Map.GetAdjacentTiles(Location);
			var bestDistance = 10000;
			
			foreach(var tile in adjacentTiles)
			{
				if(tile.Adjacents.HasValue && tile.TileType == MapTileType.Empty)
				{
					var distance = tile.Location.DistanceTo(_scene.PlayerPosition);
					if (distance < bestDistance)
					{
						bestDistance = distance;
						target = tile;
					}
				}
			}

			Location = target.Location;
		}
	}	
}
