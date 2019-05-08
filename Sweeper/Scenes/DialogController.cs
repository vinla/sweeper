using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

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
            spriteBatch.DrawString(Scene.Fonts["Readout"], _message, new Vector2(230, 280), Color.Yellow);
            base.DrawOverlay(spriteBatch);
        }
    }
}
