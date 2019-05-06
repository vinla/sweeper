using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeper.GameObjects
{
    public class Hacker
    {
        private Queue<MapTile> _trail = new Queue<MapTile>();

        public MapTile Tile { get; private set; }

        public Point Location => Tile.Location;

        public void MoveTo(MapTile tile)
        {
            _trail.Enqueue(tile);
            Tile = tile;
        }

        public Queue<MapTile> Trail => _trail;
    }
}
