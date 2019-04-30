using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Sweeper
{
    public class MainScene : Scene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private Texture2D _playerSprite;
        private Point _playerPosition;
        private MapTile[] _map;

		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
            _playerPosition = new Point(0, 0);
            _map = new MapTile[100];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    _map[i + (j * 10)] = new MapTile(i, j);
            GetTileAt(5, 5).TileType = MapTileType.Blocked;
        }

        public override void Initialise()
        {
            _playerSprite = _contentManager.Load<Texture2D>("ball");
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Olive);

            var gridSprite = graphicsDevice.CreateRectangeTexture(48, 48, 2);

            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                spriteBatch.Begin();
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var color = GetTileColor(GetTileAt(i, j));
                        spriteBatch.Draw(gridSprite, new Vector2(i * 48, j * 48), color);
                    }
                }

                spriteBatch.Draw(_playerSprite, new Rectangle(_playerPosition.X * 48, _playerPosition.Y * 48, 48, 48), Color.White);

                spriteBatch.End();
            }
		}

        public Color GetTileColor(MapTile mapTile)
        {
            if (mapTile.Adjacents.HasValue == false)
                return Color.Black;
            else
            {
                switch(mapTile.TileType)
                {
                    case MapTileType.Blocked:
                        return Color.Gray;
                    default:
                        return Color.White;
                }
            }
        }

		public override void Update(GameTime gameTime)
		{
            var originalPosition = _playerPosition;

			if (_inputManager.WasInput(GameInput.MoveUp))
            {
                _playerPosition = new Point(_playerPosition.X, _playerPosition.Y - 1);
            }
            else if(_inputManager.WasInput(GameInput.MoveDown))
            {
                _playerPosition = new Point(_playerPosition.X, _playerPosition.Y + 1);
            }
            else if(_inputManager.WasInput(GameInput.MoveLeft))
            {
                _playerPosition = new Point(_playerPosition.X - 1, _playerPosition.Y);
            }
            else if(_inputManager.WasInput(GameInput.MoveRight))
            {
                _playerPosition = new Point(_playerPosition.X + 1, _playerPosition.Y);
            }
            
            if (_playerPosition.X < 0 || _playerPosition.X > 9 || _playerPosition.Y < 0 || _playerPosition.Y > 9)
                _playerPosition = originalPosition;

            var targetTile = GetTileAt(_playerPosition);
            if (targetTile.TileType == MapTileType.Blocked)
                _playerPosition = originalPosition;

            if (_playerPosition != originalPosition)
            {
                var playerTile = GetTileAt(_playerPosition);
                playerTile.Adjacents = 0;
                foreach (var tile in GetAdjacentTiles(playerTile))
                    tile.Adjacents = 0;
            }
		}

        private MapTile GetTileAt(Point p)
        {
            return GetTileAt(p.X, p.Y);
        }

        private MapTile GetTileAt(int x, int y)
        {
            return _map.SingleOrDefault(t => t.Location.X == x && t.Location.Y == y);
        }
        
        public MapTile[] GetAdjacentTiles(MapTile tile)
        {
            var tiles = new List<MapTile>();

            for(int i = -1; i < 2; i++)
                for(int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    var adjTile = GetTileAt(tile.Location.X + i, tile.Location.Y + j);
                    if (adjTile != null)
                        tiles.Add(adjTile);
                }

            return tiles.ToArray();
        }
	}

    public class MapTile
    {
        public MapTile(int x, int y)
        {
            Location = new Point(x, y);
        }
        public int? Adjacents { get; set; }

        public MapTileType TileType { get; set; }

        public Point Location { get; }
    }

    public enum MapTileType
    {        
        Empty,
        Start,
        Blocked,
        Hazard,
        Treasure,
        Exit
    }
}
