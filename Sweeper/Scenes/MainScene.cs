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
        private readonly List<string> _consoleMessages;        
		
        private Texture2D _playerSprite;
		private SpriteFont _gameFont;
        private int _countdown;

		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
			_controllerStack = new Stack<BaseController>();
            _consoleMessages = new List<string>();

            var playerController = new PlayerController(this);
			playerController.Initialise();
			_controllerStack.Push(playerController);
			
            PlayerPosition = new Point(0, 0);
            Map = MapGenerator.Generate(20, 15, Difficulty);

            _countdown = 20;
            Teleports = 3;
		}

        public static int Difficulty = 1;

        public static int Score = 0;

		public Map Map { get; }

		public Point PlayerPosition { get; private set; }

		public bool PlayerMoved { get; set;  }

        public int Teleports { get; set; }

        public int Countdown => _countdown;

		public Stack<BaseController> Controllers => _controllerStack;

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
				
				spriteBatch.Draw(_playerSprite, new Rectangle(PlayerPosition.X * 48, PlayerPosition.Y * 48, 48, 48), Color.White);
                var playerTile = Map.GetTileAt(PlayerPosition);
                if(playerTile.Adjacents.GetValueOrDefault() > 0)
                {
                    var offset = new Vector2(8, 8);
                    var gridPosition = new Vector2(PlayerPosition.X * 48, PlayerPosition.Y * 48);
                    spriteBatch.DrawString(_gameFont, playerTile.Adjacents.ToString(), gridPosition + offset, Color.White);
                }
				_controllerStack.Peek().DrawOverlay(spriteBatch);

                DrawHUD(graphicsDevice);
                DrawConsoleMessages(graphicsDevice);

                spriteBatch.End();
            }
		}

        private void DrawHUD(GraphicsDevice graphicsDevice)
        {
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                var offset = Matrix.CreateTranslation(0, 20, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offset);

                spriteBatch.DrawString(_gameFont, $"Level: {Difficulty}", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(_gameFont, $"Score: {Score}", new Vector2(10, 35), Color.White);
                spriteBatch.DrawString(_gameFont, $"Countdown: {_countdown}" , new Vector2(10, 60), Color.White);
                spriteBatch.DrawString(_gameFont, $"Teleports: {Teleports}", new Vector2(10, 85), Color.White);

                spriteBatch.End();
            }
        }

        private void DrawConsoleMessages(GraphicsDevice graphicsDevice)
        {
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                var offset = Matrix.CreateTranslation(0, 320, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offset);
                
                for(int i = 0; i < 10; i++)
                {
                    if( i < _consoleMessages.Count )
                        spriteBatch.DrawString(_gameFont, _consoleMessages[i], new Vector2(10, 25 * i), Color.White);
                }

                spriteBatch.End();
            }
        }

        public Color GetTileColor(MapTile mapTile)
        {
            if (mapTile.IsVisible == false)
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
            if(Map.Tiles.Any(t => t.TileType == MapTileType.Hazard && t.Adjacents.HasValue == false) == false)
            {
                ShowDialog("Level Complete", NextLevel);
            }
            else if(Countdown < 1)
            {
                ShowDialog("Times run out! Game Over", () => _sceneManager.EndScene());
            }
            else
            {
                var tile = Map.GetTileAt(PlayerPosition);
                ResolveTile(tile);
                if (PlayerMoved)
                    _countdown--;
                PlayerMoved = false;                
            }

            _controllerStack.Peek().ProcessInput(gameTime, _inputManager);
        }

        private void ShowDialog(string message, System.Action action)
        {
            var dialogContoller = new Scenes.DialogController(this, message, action);
            dialogContoller.Initialise();
            _controllerStack.Push(dialogContoller);
        }

        public void NextLevel()
        {
            Difficulty++;
            Score += Countdown + (Teleports * 10);
            Reset();
        }

        public void WriteConsoleMessage(string message)
        {
            _consoleMessages.Insert(0, message);
        }

		public void ResolveTile(MapTile tile)
		{
			if (tile.Adjacents.HasValue)
				return;

			if(tile.TileType == MapTileType.Hazard)
			{
                if(PlayerPosition == tile.Location)
                {
                    _countdown -= 4;
                    _consoleMessages.Insert(0, "Anomaly triggered!");
                }
                else
                {
                    _countdown += 4;
                    _consoleMessages.Insert(0, "Anomaly resolved");
                }
				tile.Adjacents = 0;                
				return;
			}

            if (tile.TileType == MapTileType.Blocked)
                return;

			var adjacentTiles = Map.GetAdjacentTiles(tile);
            tile.Adjacents = adjacentTiles.Count(t => t.TileType == MapTileType.Hazard);

			if (tile.Adjacents == 0)
				foreach (var next in adjacentTiles)
					ResolveTile(next);
		}		
	}
}
