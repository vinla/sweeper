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
    public class CreditsScene : Scene
    {
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly ISceneManager _sceneManager;
        private SpriteFont _menuFont;
        private Texture2D _background;

        public CreditsScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
        {
            _sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
        }

        public override void Initialise()
        {
            _menuFont = _contentManager.Load<SpriteFont>("MainMenu");
            _background = _contentManager.Load<Texture2D>("title");            
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                int startY = 280;
                spriteBatch.Begin();
                spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
                startY = DrawCreditsSection(spriteBatch, startY, "Game Design", "Vincent Crowe") + 10;
                startY = DrawCreditsSection(spriteBatch, startY, "Code", "Vincent Crowe") + 10;
                startY = DrawCreditsSection(spriteBatch, startY, "Graphics", "Vincent Crowe") + 10;
                startY = DrawCreditsSection(spriteBatch, startY, "Music", "Nobody") + 10;
                startY = DrawCreditsSection(spriteBatch, startY, "Testing", "Kim Crowe", "Corin Crowe", "Robin Derwent");
                spriteBatch.End();
            }
        }

        private int DrawCreditsSection(SpriteBatch spriteBatch, int startY, string title, params string[] credits)
        {
            spriteBatch.DrawString(_menuFont, title, new Vector2(180, startY), Color.Yellow);
            foreach(var credit in credits)
            {
                startY += 30;
                spriteBatch.DrawString(_menuFont, credit, new Vector2(180, startY), Color.White);
            }
            return startY + 30;
        }

        public override void Update(GameTime gameTime)
        {
            if (_inputManager.WasInput(GameInput.MenuBack) || _inputManager.WasInput(GameInput.MenuSelect))
                _sceneManager.EndScene();
        }
    }
}
