using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sweeper.GameObjects;

namespace Sweeper.Scenes
{
    public abstract class EndLevelScene : Scene
    {
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly MainScene _mainScene;
        private float _scale = 0;

        public EndLevelScene(MainScene mainScene, IInputManager inputManager, ISceneManager sceneManager, ContentManager contentManager)
        {
            _mainScene = mainScene;
            _inputManager = inputManager;
            SceneManager = sceneManager;
            _contentManager = contentManager;
        }

        protected ISceneManager SceneManager { get; }

        protected SpriteFont Font { get; private set; }

        protected SpriteFont SmallFont { get; private set; }

        protected MainScene Scene => _mainScene;

        public override void Initialise()
        {
            Font = _contentManager.Load<SpriteFont>("Readout");
            SmallFont = _contentManager.Load<SpriteFont>("MainMenu");
            base.Initialise();
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            _mainScene.Draw(gameTime, graphicsDevice);
            var pixel = graphicsDevice.CreateRectangeTexture(1, 1, 0, Color.Black, Color.Black);
            var size = System.Math.Min(48, 48 * _scale);
            var off = (int)((48 - size) / 2);
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                var offsetTransform = Matrix.CreateTranslation(320, 0, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, offsetTransform);

                foreach (var tile in _mainScene.Map.Tiles)
                {
                    spriteBatch.Draw(pixel, new Rectangle((tile.Location.X * 48) + off, (tile.Location.Y * 48) - off, (int)(size), (int)(size)), Color.Black);
                }

                
                if (_scale > 1)
                {
                    DrawInfo(spriteBatch);
                }                                   

                spriteBatch.End();
            }            
        }

        protected abstract void DrawInfo(SpriteBatch spriteBatch);

        protected abstract void Continue();        

        public override void Update(GameTime gameTime)
        {
            _scale += (float)gameTime.ElapsedGameTime.TotalSeconds / .75f;
            if (_inputManager.WasInput(GameInput.MenuSelect))
            {
                Continue();                
            }
        }
    }

    public class LevelCompleteScene : EndLevelScene
    {
        private readonly List<string> _scoreInfo;
        private int _score = 0;

        public LevelCompleteScene(MainScene mainScene, IInputManager inputManager, ISceneManager sceneManager, ContentManager contentManager)
            : base(mainScene, inputManager, sceneManager, contentManager)
        {
            _scoreInfo = new List<string>();
        }

        public override void Initialise()
        {
            base.Initialise();
            _scoreInfo.Add($"Nodes hacked ({Scene.NodesHacked}) x {Scoring.HackedModeMultiplier} = {Scene.NodesHacked * Scoring.HackedModeMultiplier}");
            _score += Scene.NodesHacked * Scoring.HackedModeMultiplier;
            _scoreInfo.Add($"Bit coins ({Scene.BitCoin}) x {Scoring.BitCoinMultiplier} = {Scene.BitCoin* Scoring.BitCoinMultiplier}");
            _score += Scene.BitCoin * Scoring.BitCoinMultiplier;

            if (Scene.Penalties.Count(p => p == TracePenalty.HackError || p == TracePenalty.Alert) == 0)
            {
                _scoreInfo.Add($"No mistakes = {Scoring.NoMistakes}");
                _score += Scoring.NoMistakes;
            }
            
            if (Scene.RemainingBitCoin == 0)
            {
                _scoreInfo.Add($"All coins collected = {Scoring.AllCoins}");
                _score += Scoring.AllCoins;
            }

            if (MainScene.Difficulty >= 15 && Scene.ResetUsed == false)
            {
                _scoreInfo.Add($"No resets (15 or more nodes) = {Scoring.NoResets}");
                _score += Scoring.NoResets;
            }
        }

        protected override void DrawInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, $"Level Complete", new Vector2(150, 200), Color.Yellow);

            int ypos = 250;

            foreach(var line in _scoreInfo)
            {
                spriteBatch.DrawString(SmallFont, line, new Vector2(160, ypos), Color.White);
                ypos += 30;
            }

            spriteBatch.DrawString(SmallFont, $"Total Score = {_score}", new Vector2(160, ypos), Color.Yellow);
        }

        protected override void Continue()
        {
            MainScene.Difficulty = Math.Min(50, MainScene.Difficulty += MainScene.Optons.Ramp);
            MainScene.Score += _score;
            if (MainScene.Score > MainScene.HighScore)
            {
                DataManager.SaveHighScore(MainScene.Score, MainScene.Optons.GameType);
                MainScene.HighScore = MainScene.Score;
            }

            SceneManager.EndScene();
            SceneManager.StartScene<MainScene>();
        }
    }

    public class GameOverScene : EndLevelScene
    {
        public GameOverScene(MainScene mainScene, IInputManager inputManager, ISceneManager sceneManager, ContentManager contentManager)
            : base(mainScene, inputManager, sceneManager, contentManager)
        {

        }

        protected override void DrawInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, $"Game Over", new Vector2(150, 200), Color.Red);
            spriteBatch.DrawString(SmallFont, $"You were traced.", new Vector2(150, 260), Color.Yellow);
            spriteBatch.DrawString(SmallFont, $"Keep an eye on your detection level.", new Vector2(150, 290), Color.Yellow);
        }

        protected override void Continue()
        {
            SceneManager.EndScene();
        }
    }

    public static class Scoring
    {
        public static int HackedModeMultiplier => 100;

        public static int BitCoinMultiplier => 20;

        public static int NoMistakes => 500;

        public static int AllCoins => 200;

        public static int NoResets => 250;
    }
}
