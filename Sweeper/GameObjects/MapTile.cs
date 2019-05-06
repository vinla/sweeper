using Sweeper.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sweeper
{
	public class MapTile
    {
        private Map _map;
        public MapTile(Map map, int x, int y, TileModifier modifier)
        {
            _map = map;
            Location = new Point(x, y);
            Modifier = modifier;
        }
        public MapTile[] AdjacentTiles => _map.GetAdjacentTiles(this);

        public int DiscoveredNodes => AdjacentTiles.Count(t => t.Modifier.Detectable);

        public Map Map => _map;

        public TileModifier Modifier{ get; set; }

        public Point Location { get; }

        public bool Discovered { get; set; }

        public void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            var rect = new Rectangle(Location.X * 48, Location.Y * 48, 48, 48);

            if (Discovered || Modifier is Blocked)
            {                
                Modifier.Draw(rect, this, spriteBatch, textures, font);
            }
            else
            {
                spriteBatch.Draw(textures["GridCell"], rect, Color.Black);
            }
        }        
    }
}
