using System.Linq;
using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class Map
	{		
		private readonly MapTile[] _tiles;

		public Map(int width, int height)
		{
			Width = width;
			Height = height;
			_tiles = new MapTile[width * height];			
			for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					_tiles[i + (j * width)] = new MapTile(i, j);
		}

		public int Width { get; }

		public int Height { get; }

		public MapTile GetTileAt(Point p)
		{
			return GetTileAt(p.X, p.Y);
		}

		public MapTile GetTileAt(int x, int y)
		{
			return _tiles.SingleOrDefault(t => t.Location.X == x && t.Location.Y == y);
		}
	}
}
