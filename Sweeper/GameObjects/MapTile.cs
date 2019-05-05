using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class MapTile
    {		
        public MapTile(int x, int y)
        {
            Location = new Point(x, y);
        }
        public int? Adjacents { get; set; }

        public MapTileType TileType { get; set; }

        public Point Location { get; }

        public bool IsVisible => Adjacents.HasValue || TileType == MapTileType.Blocked;
    }
}
