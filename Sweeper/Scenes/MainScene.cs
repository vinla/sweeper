using Sweeper.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Sweeper.Controllers;

namespace Sweeper.Scenes
{
	public class MainScene : Scene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly Dictionary<string, Texture2D> _textures;
		private readonly Stack<BaseController> _controllerStack;
		private readonly List<FloatText> _floatingText;
        
        private Texture2D _playerSprite;
		private Dictionary<string, SpriteFont> _fonts;
     
		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
			_controllerStack = new Stack<BaseController>();
			_floatingText = new List<FloatText>();
            _textures = new Dictionary<string, Texture2D>();
            _fonts = new Dictionary<string, SpriteFont>();
            
            var playerController = new PlayerController(this);
			playerController.Initialise();
			_controllerStack.Push(playerController);

            Player = new Hacker();
            Map = MapGenerator.Generate(20, 15, Difficulty, this);
            Penalties = new List<TracePenalty>();
		}

        public static int Difficulty = 0;

        public static int HighScore = 0;

        public static RunOptions Optons;

        public static int Score = 0;

        public int BitCoin = 0;

        public List<TracePenalty> Penalties { get; }

        public int Trace => System.Math.Max(0, Penalties.Cast<int>().Select(p => MainScene.Optons.Penalties[p]).Sum() + Player.Trail.Where(t => t.Modifier is HackedNode == false).Distinct().Count());

		public Map Map { get; }

		public Hacker Player { get; }

		public int RemainingNodes => Map.Tiles.Count(t => t.Modifier is Node && !t.Discovered);

        public int NodesHacked => Map.Tiles.Count(t => t.Modifier is HackedNode);

        public int RemainingBitCoin => Map.Tiles.Count(t => t.Modifier is BitCoin);

        public bool ResetUsed { get; set; }

		public Stack<BaseController> Controllers => _controllerStack;

		public Dictionary<string, SpriteFont> Fonts => _fonts;		

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
            _playerSprite = _contentManager.Load<Texture2D>("hacker");
            _fonts.Add("Menu", _contentManager.Load<SpriteFont>("MainMenu"));
            _fonts.Add("Console", _contentManager.Load<SpriteFont>("Console"));
            _fonts.Add("Readout", _contentManager.Load<SpriteFont>("Readout"));
            _textures.Add("Player", _playerSprite);
            _textures.Add("Node", _contentManager.Load<Texture2D>("node"));
            _textures.Add("HackedNode", _contentManager.Load<Texture2D>("hacked_node"));
            _textures.Add("Corrupt", _contentManager.Load<Texture2D>("corrupt"));
            _textures.Add("Cypher", _contentManager.Load<Texture2D>("cypher"));
            Player.MoveTo(Map.GetTileAt(0, 0));
            EnterTile(Map.GetTileAt(Player.Location));
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.Clear(Color.Black);

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
                        tile.Draw(spriteBatch, _textures, _fonts.Values.First());
                    }
                }
				
				spriteBatch.Draw(_playerSprite, new Rectangle(Player.Location.X * 48, Player.Location.Y * 48, 48, 48), Color.White);

                var playerTile = Map.GetTileAt(Player.Location);
                if(playerTile.Modifier is Empty && playerTile.AdjacentTiles.Count(t => t.Modifier.Detectable) > 0)
                {
                    var offset = new Vector2(5, 5);
                    var gridPosition = new Vector2(playerTile.Location.X * 48, playerTile.Location.Y * 48);
                    spriteBatch.DrawString(_fonts.Values.First(), playerTile.AdjacentTiles.Count(t => t.Modifier.Detectable).ToString(), gridPosition + offset, Color.Black);
                }
				_controllerStack.Peek().DrawOverlay(spriteBatch);                

                spriteBatch.End();
            }

			DrawHUD(graphicsDevice);
			DrawConsoleMessages(graphicsDevice);
		}

        private void DrawHUD(GraphicsDevice graphicsDevice)
        {
            var pixel = graphicsDevice.CreateRectangeTexture(1, 1, 0, Color.White, Color.White);
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                var offset = Matrix.CreateTranslation(0, 20, 0);
                var color = Color.Green;
                if (Trace > 50)
                    color = Color.Orange;
                if (Trace > 75)
                    color = Color.Red;

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offset);
                spriteBatch.DrawString(_fonts["Console"], "VINDOS_1.24.77 [STABLE]", new Vector2(10, 5), Color.Green);
                spriteBatch.DrawString(_fonts["Console"], "VPC connected...", new Vector2(10, 35), Color.Green);

                spriteBatch.DrawString(_fonts["Console"], "Bits Coins", new Vector2(10, 70), Color.LightGreen);
                spriteBatch.DrawString(_fonts["Console"], $"{BitCoin}", new Vector2(220, 70), Color.White);

                spriteBatch.DrawString(_fonts["Console"], "Remaining Nodes", new Vector2(10, 105), Color.LightGreen);
                spriteBatch.DrawString(_fonts["Console"], $"{RemainingNodes}", new Vector2(220, 105), Color.White);

                spriteBatch.DrawString(_fonts["Console"], "Detection Level", new Vector2(10, 140), Color.LightGreen);
                spriteBatch.Draw(pixel, new Rectangle(215, 135, Trace > 99 ? 52 : 36, 30), color);
                spriteBatch.DrawString(_fonts["Console"], $"{Trace}", new Vector2(220, 140), Color.White);

                spriteBatch.DrawString(_fonts["Console"], $"Hi Score {MainScene.HighScore}", new Vector2(62, 645), Color.Green);
                spriteBatch.DrawString(_fonts["Console"], $"Score {MainScene.Score}", new Vector2(100, 675), Color.Yellow);
                
                spriteBatch.End();
            }
        }

        private void DrawConsoleMessages(GraphicsDevice graphicsDevice)
        {
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
				var offsetTransform = Matrix.CreateTranslation(320, 0, 0);
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offsetTransform);
                
                foreach(var floatText in _floatingText)
				{
					floatText.Draw(spriteBatch);
				}

                spriteBatch.End();
            }
        }        

		public override void Update(GameTime gameTime)
		{
            if(RemainingNodes == 0)
            {
                LevelComplete(false);
            }
            else if (Trace > 99 )
            {
                LevelComplete(true); 
            }

            _controllerStack.Peek().ProcessInput(gameTime, _inputManager);

			for (int i = 0; i < _floatingText.Count;)
			{
				var floatingText = _floatingText[i];
				if (floatingText.IsActive)
				{
					floatingText.Update(gameTime);
					i++;
				}
				else
					_floatingText.RemoveAt(i);
			}
		}

        private void LevelComplete(bool gameOver)
        {
            _sceneManager.EndScene();
            var levelEnd = gameOver ? (EndLevelScene) new GameOverScene(this, _inputManager, _sceneManager, _contentManager) 
                : (EndLevelScene) new LevelCompleteScene(this, _inputManager, _sceneManager, _contentManager);
            levelEnd.Initialise();
            _sceneManager.RunScene(levelEnd);
        }

		public void FloatText(string text, MapTile tile, Color color)
		{
			var location = new Vector2(tile.Location.X * 48 - 20, tile.Location.Y * 48 - 16);
			FloatText(text, location, color);
		}

		public void FloatText(string text, Vector2 location, Color color)
		{
			_floatingText.Add(new FloatText(text, color, _fonts.Values.First(), location, new Vector2(0, -45), .7f));
		}

        public void EnterTile(MapTile tile)
        {
            tile.Modifier.Enter(tile);
        }

        public void Hack(MapTile tile)
        {
            if (tile.Modifier is Node)
            {
				FloatText("Node hacked!", tile, Color.Yellow);
                tile.Modifier = new HackedNode();
                tile.Discovered = true;                
            }
            else
            {
                FloatText("No Node detected!", tile, Color.OrangeRed);
                Penalties.Add(TracePenalty.HackError);
                tile.Modifier = new Corrupted();
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
}
