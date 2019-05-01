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
		private SpriteFont _gameFont;
        private Point _playerPosition;
        private Map _map;

		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
            _playerPosition = new Point(0, 0);
			_map = new Map(20, 15);

			_map.GetTileAt(0, 0).TileType = MapTileType.Start;
			var rng = new System.Random();
			for(int i = 0; i < 30; i++)
			{
				int x = rng.Next(0, _map.Width);
				int y = rng.Next(0, _map.Height);
				_map.GetTileAt(x, y).TileType = MapTileType.Hazard;
			}
            //_map.GetTileAt(5, 5).TileType = MapTileType.Hazard;
			//_map.GetTileAt(5, 6).TileType = MapTileType.Hazard;
		}

        public override void Initialise()
        {
            _playerSprite = _contentManager.Load<Texture2D>("ball");
			_gameFont = _contentManager.Load<SpriteFont>("MainMenu");
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Olive);

            var gridSprite = graphicsDevice.CreateRectangeTexture(48, 48, 2);

            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                spriteBatch.Begin();
                for (int i = 0; i < _map.Width; i++)
                {
                    for (int j = 0; j < _map.Height; j++)
                    {
						var tile = _map.GetTileAt(i, j);
						var color = GetTileColor(tile);
						var gridPosition = new Vector2(i * 48, j * 48);
						spriteBatch.Draw(gridSprite, gridPosition, color);
						if (tile.Adjacents > 0)
						{
							spriteBatch.DrawString(_gameFont, tile.Adjacents.ToString(), gridPosition, Color.Black);
						}
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
					case MapTileType.Hazard:
						return Color.Red;
					case MapTileType.Treasure:
						return Color.Gold;
					case MapTileType.Start:
						return Color.LightSteelBlue;
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
            
            if (_playerPosition.X < 0 || _playerPosition.X >= _map.Width || _playerPosition.Y < 0 || _playerPosition.Y >= _map.Height)
                _playerPosition = originalPosition;

            var targetTile = _map.GetTileAt(_playerPosition);
            if (targetTile.TileType == MapTileType.Blocked)
                _playerPosition = originalPosition;

            if (_playerPosition != originalPosition)
            {
                var playerTile = _map.GetTileAt(_playerPosition);
				ResolveTile(playerTile);
            }
		}

		public void ResolveTile(MapTile tile)
		{
			if (tile.Adjacents.HasValue)
				return;

			if(tile.TileType != MapTileType.Empty)
			{
				tile.Adjacents = 0;
				return;
			}

			var adjacentTiles = GetAdjacentTiles(tile);

			var hazardTiles = new[] { MapTileType.Hazard, MapTileType.Treasure };

			tile.Adjacents = adjacentTiles.Count(t => hazardTiles.Contains(t.TileType));

			if (tile.Adjacents == 0)
				foreach (var next in adjacentTiles)
					ResolveTile(next);
		}        
        
        public MapTile[] GetAdjacentTiles(MapTile tile)
        {
            var tiles = new List<MapTile>();

            for(int i = -1; i < 2; i++)
                for(int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    var adjTile = _map.GetTileAt(tile.Location.X + i, tile.Location.Y + j);
                    if (adjTile != null)
                        tiles.Add(adjTile);
                }

            return tiles.ToArray();
        }
	}
}
