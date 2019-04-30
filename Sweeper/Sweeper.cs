using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sweeper
{
    public class Sweeper : Game
    {
        private readonly GraphicsDeviceManager _graphics;
		private readonly SceneManager _sceneManager;
		private ContainerBuilder _containerBuilder;
		private IContainer _container;
        
        public Sweeper()
        {
            _graphics = new GraphicsDeviceManager(this);
			_sceneManager = new SceneManager(this);
			_containerBuilder = new ContainerBuilder();
			_containerBuilder.RegisterInstance<ISceneManager>(_sceneManager);			
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
			_containerBuilder.RegisterModule<SceneModule>();
			_containerBuilder.RegisterInstance(Content);
			_container = _containerBuilder.Build();
			_sceneManager.SetContainer(_container);
			_sceneManager.StartScene<MainMenu>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
			
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _sceneManager.CurrentScene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
			_sceneManager.CurrentScene.Draw(gameTime, GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}
