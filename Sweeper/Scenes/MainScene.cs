using Sweeper.GameObjects;
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
        private readonly Dictionary<string, Texture2D> _textures;
		private readonly Stack<BaseController> _controllerStack;
        private readonly List<string> _consoleMessages;        
		
        private Texture2D _playerSprite;
		private SpriteFont _gameFont;
     
		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
			_controllerStack = new Stack<BaseController>();
            _consoleMessages = new List<string>();
            _textures = new Dictionary<string, Texture2D>();

            var playerController = new PlayerController(this);
			playerController.Initialise();
			_controllerStack.Push(playerController);

            Player = new Hacker();
            Map = MapGenerator.Generate(20, 15, Difficulty, this);
            Penalties = new List<TracePenalty>();
		}

        public static int Difficulty = 1;

        public static int BitCoin = 0;

        public List<TracePenalty> Penalties { get; }

        public int Trace => System.Math.Max(0, Penalties.Cast<int>().Sum() + Player.Trail.Where(t => t.Modifier is HackedNode == false).Distinct().Count());

		public Map Map { get; }

		public Hacker Player { get; }

        public Stack<BaseController> Controllers => _controllerStack;

		public SpriteFont Font => _gameFont;
		

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
            _textures.Add("Player", _playerSprite);
            Player.MoveTo(Map.GetTileAt(0, 0));
            EnterTile(Map.GetTileAt(Player.Location));
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Olive);

            if (_textures.ContainsKey("GridCell") == false)
            {
                var gridSprite = graphicsDevice.CreateRectangeTexture(48, 48, 2, Color.Black, Color.White);
                _textures.Add("GridCell", gridSprite);
            }

            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
				var offsetTransform = Matrix.CreateTranslation(320, 0, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offsetTransform);
                for (int i = 0; i < Map.Width; i++)
                {
                    for (int j = 0; j < Map.Height; j++)
                    {
						var tile = Map.GetTileAt(i, j);
                        tile.Draw(spriteBatch, _textures, _gameFont);						
                    }
                }
				
				spriteBatch.Draw(_playerSprite, new Rectangle(Player.Location.X * 48, Player.Location.Y * 48, 48, 48), Color.White);

                var playerTile = Map.GetTileAt(Player.Location);
                if(playerTile.Modifier is Empty && playerTile.AdjacentTiles.Count(t => t.Modifier.Detectable) > 0)
                {
                    var offset = new Vector2(8, 8);
                    var gridPosition = new Vector2(playerTile.Location.X * 48, playerTile.Location.Y * 48);
                    spriteBatch.DrawString(_gameFont, playerTile.AdjacentTiles.Count(t => t.Modifier.Detectable).ToString(), gridPosition + offset, Color.White);
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

                spriteBatch.DrawString(_gameFont, $"Alerts: {Penalties.Count(p => p == TracePenalty.NodeFault)}", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(_gameFont, $"Misses: {Penalties.Count(p => p == TracePenalty.HackError)}", new Vector2(10, 35), Color.White);
                spriteBatch.DrawString(_gameFont, $"Detection Level: {Trace}", new Vector2(10, 60), Color.White);

                spriteBatch.DrawString(_gameFont, $"Bit Coin: {BitCoin}", new Vector2(10, 110), Color.Yellow);

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

		public override void Update(GameTime gameTime)
		{
            if(Map.Tiles.Any(t => t.Modifier is Node && !t.Discovered) == false)
            {
                ShowDialog("Level Complete", NextLevel);
            }
            else if (Trace > 99 )
            {
                ShowDialog("You have been traced. Game Over!", () => _sceneManager.EndScene());
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
            Reset();
        }

        public void WriteConsoleMessage(string message)
        {
            _consoleMessages.Insert(0, message);
        }

        public void EnterTile(MapTile tile)
        {
            tile.Modifier.Enter(tile);
        }

        public void Hack(MapTile tile)
        {
            if (tile.Modifier is Node)
            {
                WriteConsoleMessage("Node hacked");
                tile.Modifier = new HackedNode();
                tile.Discovered = true;
                MainScene.BitCoin += 5;
            }
            else
            {
                WriteConsoleMessage("Error! No Node detected.");
                Penalties.Add(TracePenalty.HackError);
                ResolveTile(tile);
            }
        }

		public void ResolveTile(MapTile tile)
		{
			if (tile.Discovered || tile.Modifier is Blocked)
				return;

            tile.Discovered = true;

			if (tile.DiscoveredNodes == 0)
				foreach (var next in tile.AdjacentTiles)
					ResolveTile(next);
		}		
	}

    public enum TracePenalty
    {
        NodeFault = 10,
        HackError = 5,
        EncryptionBreak = 3
    }
}
