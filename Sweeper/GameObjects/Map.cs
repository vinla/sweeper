using System.Collections.Generic;
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
                {
                    _tiles[i + (j * width)] = new MapTile(i, j) { TileType = MapTileType.Blocked };                    
                }
		}

		public int Width { get; }

		public int Height { get; }

        public MapTile[] Tiles => _tiles;

		public MapTile GetTileAt(Point p)
		{
			return GetTileAt(p.X, p.Y);
		}

		public MapTile GetTileAt(int x, int y)
		{
			return _tiles.SingleOrDefault(t => t.Location.X == x && t.Location.Y == y);
		}

		public MapTile[] GetAdjacentTiles(MapTile tile)
		{
			return GetAdjacentTiles(tile.Location);
		}

		public MapTile[] GetAdjacentTiles(Point p)
		{
			var tiles = new List<MapTile>();

			for (int i = -1; i < 2; i++)
				for (int j = -1; j < 2; j++)
				{
					if (i == 0 && j == 0)
						continue;
					var adjTile = GetTileAt(p.X + i, p.Y + j);
					if (adjTile != null)
						tiles.Add(adjTile);
				}

			return tiles.ToArray();
		}
	}
}
