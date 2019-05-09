using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public LevelCompleteScene(MainScene mainScene, IInputManager inputManager, ISceneManager sceneManager, ContentManager contentManager)
            : base(mainScene, inputManager, sceneManager, contentManager)
        {

        }

        protected override void DrawInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, $"Level Complete", new Vector2(150, 200), Color.Yellow);
        }

        protected override void Continue()
        {
            MainScene.Difficulty++;
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
}
