using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sweeper.Scenes
{
    public class DialogController : BaseController<MainScene>
    {
        private readonly string _message;
        private readonly Action _action;
        public DialogController(MainScene scene, string message, Action action) : base(scene)
        {
            _action = action;
            _message = message;
        }

        [InputAction(GameInput.MenuSelect)]
        public void Continue()
        {
            _action();
        }

        public override void DrawOverlay(SpriteBatch spriteBatch)
        {
            var dialogBox = spriteBatch.GraphicsDevice.CreateRectangeTexture(480, 320, 4, Color.Black, Color.Red);
            spriteBatch.Draw(dialogBox, new Vector2(280, 192), Color.White);
            spriteBatch.DrawString(Scene.Font, _message, new Vector2(340, 280), Color.Yellow);
            base.DrawOverlay(spriteBatch);
        }
    }
}
