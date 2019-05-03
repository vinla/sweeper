using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeper
{
    public static class MapGenerator
    {
        public static Map Generate(int width, int height, int difficulty)
        {
            var rng = new Random();
            var map = new Map(width, height);
            for(int i = 0; i < difficulty;)
            {
                var x = rng.Next(0, width);
                var y = rng.Next(0, height);

                if( x > 2 || y > 2 )
                {
                    var tile = map.GetTileAt(x, y);
                    if (tile.TileType == MapTileType.Empty)
                    {
                        tile.TileType = MapTileType.Hazard;
                        i++;
                    }
                }
            }

            return map;
        }
    }

    public class GameContext
    {
        public int CurrentLevel { get; set; }
    }
}
