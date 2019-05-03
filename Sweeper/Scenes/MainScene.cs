using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sweeper
{
	public class MainScene : Scene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
		private readonly Stack<BaseController> _controllerStack;
		
        private Texture2D _playerSprite;
		private SpriteFont _gameFont;
		private List<Spirit> _spirits;		

		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
			_controllerStack = new Stack<BaseController>();

			var playerController = new PlayerController(this);
			playerController.Initialise();
			_controllerStack.Push(playerController);
			
            PlayerPosition = new Point(0, 0);
            Map = MapGenerator.Generate(20, 15, 5);
			

			var spirit = new Spirit(5, 6, this);
			_spirits = new List<Spirit>();
			_spirits.Add(spirit);
		}

		public Map Map { get; }

		public Point PlayerPosition { get; private set; }

		public bool PlayerMoved { get; set;  }

		public Stack<BaseController> Controllers => _controllerStack;

		public List<Spirit> Spirits => _spirits;

		public SpriteFont Font => _gameFont;

		public void SetPlayerPosition(Point p)
		{
			PlayerPosition = p;
		}

		public void Reset()
		{
			_sceneManager.EndScene();
			_sceneManager.StartScene<MainScene>();
		}

        public void Pause()
        {
            _sceneManager.StartScene<Scenes.PauseMenuScene>();
        }

        public override void Initialise()
        {
            _playerSprite = _contentManager.Load<Texture2D>("ball");
			_gameFont = _contentManager.Load<SpriteFont>("MainMenu");
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Olive);

            var gridSprite = graphicsDevice.CreateRectangeTexture(48, 48, 2, Color.Black, Color.White);

            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
				var offsetTransform = Matrix.CreateTranslation(320, 0, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offsetTransform);
                for (int i = 0; i < Map.Width; i++)
                {
                    for (int j = 0; j < Map.Height; j++)
                    {
						var tile = Map.GetTileAt(i, j);
						var color = GetTileColor(tile);
						var gridPosition = new Vector2(i * 48, j * 48);
						spriteBatch.Draw(gridSprite, gridPosition, color);
						if (tile.Adjacents > 0)
						{
							var offset = new Vector2(8, 8);
							spriteBatch.DrawString(_gameFont, tile.Adjacents.ToString(), gridPosition + offset, Color.Black);
						}
                    }
                }

				foreach (var spirit in _spirits)
					spriteBatch.Draw(_playerSprite, new Rectangle(spirit.Location.X * 48, spirit.Location.Y * 48, 48, 48), Color.Green);

				spriteBatch.Draw(_playerSprite, new Rectangle(PlayerPosition.X * 48, PlayerPosition.Y * 48, 48, 48), Color.White);
				_controllerStack.Peek().DrawOverlay(spriteBatch);

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
						return mapTile.Adjacents > 0 ? Color.Yellow : Color.White;
                }
            }
        }

		public override void Update(GameTime gameTime)
		{
			var tile = Map.GetTileAt(PlayerPosition);
			ResolveTile(tile);
			PlayerMoved = false;
			_controllerStack.Peek().ProcessInput(gameTime, _inputManager);
			foreach (var spirit in _spirits)
				spirit.Update(gameTime);
			CheckEvents();
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

			var adjacentTiles = Map.GetAdjacentTiles(tile);

			var hazardTiles = new[] { MapTileType.Hazard, MapTileType.Treasure };

			tile.Adjacents = adjacentTiles.Count(t => hazardTiles.Contains(t.TileType));

			if (tile.Adjacents == 0)
				foreach (var next in adjacentTiles)
					ResolveTile(next);
		}

		public void CheckEvents()
		{
			// TODO: Check for player death
			// TODO: Check for spirits

			for(int i =0; i < _spirits.Count; )
			{
				var tile = Map.GetTileAt(_spirits[i].Location);
				if (tile.TileType == MapTileType.Hazard)
				{
					_spirits.RemoveAt(i);
					tile.TileType = MapTileType.Empty;
					foreach(var adjTile in Map.GetAdjacentTiles(tile))
					{
						adjTile.Adjacents = null;
					}
					ResolveTile(tile);
				}
				else
					i++;
			}
		}
	}
}
