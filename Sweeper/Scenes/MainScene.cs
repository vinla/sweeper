using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sweeper
{
    public class MainScene : Scene
	{
		private readonly ISceneManager _sceneManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private Texture2D _playerSprite;
        private Point _playerPosition;

		public MainScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
		{
			_sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;
            _playerPosition = new Point(0, 0);
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
                        spriteBatch.Draw(gridSprite, new Vector2(i * 48, j * 48), Color.White);
                    }
                }

                spriteBatch.Draw(_playerSprite, new Rectangle(_playerPosition.X * 48, _playerPosition.Y * 48, 48, 48), Color.White);

                spriteBatch.End();
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
		}
	}
}
