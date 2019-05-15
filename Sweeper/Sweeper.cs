using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sweeper.Scenes;

namespace Sweeper
{
    public class Sweeper : Game
    {
        private readonly GraphicsDeviceManager _graphics;
		private readonly SceneManager _sceneManager;
        private readonly InputManager _inputManager;
		private ContainerBuilder _containerBuilder;
		private IContainer _container;
        
        public Sweeper()
        {
            _graphics = new GraphicsDeviceManager(this);
			_sceneManager = new SceneManager(this);
            _inputManager = new InputManager();
			_containerBuilder = new ContainerBuilder();
			_containerBuilder.RegisterInstance<ISceneManager>(_sceneManager);
            _containerBuilder.RegisterInstance<IInputManager>(_inputManager);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            SetResolution(1280, 720);
			_containerBuilder.RegisterModule<SceneModule>();
			_containerBuilder.RegisterInstance(Content);
			_container = _containerBuilder.Build();
			_sceneManager.SetContainer(_container);
			_sceneManager.StartScene<MainMenu>();
            base.Initialize();
        }        

        protected override void Update(GameTime gameTime)
        {
            _inputManager.EarlyUpdate(gameTime);

            _sceneManager.CurrentScene.Update(gameTime);
            base.Update(gameTime);

            _inputManager.LateUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
			_sceneManager.CurrentScene.Draw(gameTime, GraphicsDevice);
            base.Draw(gameTime);
        }

        private void SetResolution(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;            
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }
    }
}
