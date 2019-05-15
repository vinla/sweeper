using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sweeper.GameObjects;
using Sweeper.Scenes;

namespace Sweeper
{
	public class Map
	{
		private readonly MapTile[] _tiles;
        private IEncryptionProvider[] _encryptionProviders;

		public Map(int width, int height, MainScene scene)
		{
            Scene = scene;
			Width = width;
			Height = height;
			_tiles = new MapTile[width * height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    _tiles[i + (j * width)] = new MapTile(this, i, j, new Blocked());
                }

            _encryptionProviders = new IEncryptionProvider[4];
            _encryptionProviders[0] = new PlainEncryptionProvider();
            _encryptionProviders[1] = new SimpleEncryptionProvider();
            _encryptionProviders[2] = new RandomEncryptionProvider();
            _encryptionProviders[3] = new TotalEncryptionProvider();
        }

		public int Width { get; }

		public int Height { get; }

        public MainScene Scene { get; }

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

        public IEncryptionProvider GetEncryption(Point location)
        {
            return _encryptionProviders[0];
        }
    }

    public interface IEncryptionProvider
    {
        char Convert(int value);
    }

    public class PlainEncryptionProvider : IEncryptionProvider
    {
        public char Convert(int value)
        {
            return "012345678"[value];
        }
    }

    public class SimpleEncryptionProvider : IEncryptionProvider
    {
        public char Convert(int value)
        {
            return "0abcdefghi"[value];
        }
    }

    public class RandomEncryptionProvider : IEncryptionProvider
    {
        private readonly char[] _values;
        public RandomEncryptionProvider()
        {
            _values = new char[9];
            var rng = new System.Random();

            for (int i = 0; i < 9; i++)
                _values[i] = ((char)(rng.Next(0, 15) + 65));
        }

        public char Convert(int value)
        {
            return _values[value];
        }
    }

    public class TotalEncryptionProvider : IEncryptionProvider
    {
        public char Convert(int value)
        {
            return '?';
        }
    }
}
