using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper.Scenes
{
    public class HowToPlayScene : Scene
    {

        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly ISceneManager _sceneManager;
        private readonly Texture2D[] _images;
        private readonly string[] _pages;
        private int pageIndex = 0;
        private SpriteFont _font;       

        public HowToPlayScene(ISceneManager sceneManager, IInputManager inputManager, ContentManager contentManager)
        {
            _sceneManager = sceneManager;
            _inputManager = inputManager;
            _contentManager = contentManager;            
            _pages = new string[] { text1, text2, text3, text4, text5, text6, text7, text8 };
            _images = new Texture2D[_pages.Length];
        }

        public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);
            using (var spriteBatch = new SpriteBatch(graphicsDevice))
            {
                spriteBatch.Begin();
                DrawLines(spriteBatch, _pages[pageIndex].Split(':'));
                spriteBatch.End();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_inputManager.WasInput(GameInput.MenuBack))
                _sceneManager.EndScene();

            if (_inputManager.WasInput(GameInput.MenuSelect) || _inputManager.WasInput(GameInput.MoveRight))
                NextPage();

            if (_inputManager.WasInput(GameInput.MoveLeft))
                pageIndex = System.Math.Max(0, pageIndex - 1);
        }

        private void NextPage()
        {
            pageIndex++;
            if (pageIndex >= _pages.Length)
                _sceneManager.EndScene();
        }

        public override void Initialise()
        {
            _font = _contentManager.Load<SpriteFont>("MainMenu");
            for (int i = 0; i < _pages.Length; i++)
                _images[i] = _contentManager.Load<Texture2D>("how_to_" + i);
            base.Initialise();
        }

        private void DrawLines(SpriteBatch spriteBatch, string[] lines)
        {
            var posY = 300;
            foreach(var line in lines)
            {
                spriteBatch.Draw(_images[pageIndex], new Vector2(100, 80), Color.White);
                spriteBatch.DrawString(_font, line, new Vector2(100, posY), Color.White);
                posY += 30;
            }
        }

        private string text1 = "Welcome hacker: :So you want to hack the corprorate networks, this handy guide will tell you everything:you need to know. Once you're in the network you can move around using [W],[A],[S],[D]:or the arrow keys or the directional pad on your controller.:Your objective is to find and hack all of the computue nodes in the network.";
        private string text2 = "As you move around your scanning program will detect nodes in the network,:it works automatically and will scan as much of the network as it can:It won't find the nodes directly, because it might set of the anti intrusion software,:instead it marks all of the locations it knows are safe with a number.:This is the number of nodes that are adjacent to that location.:Adjacent includes diagonals.";
        private string text3 = "Once you haved figured out where a node is use the hack tool to access it.:Use the hack tool by moving next to a suspected node and then press [Shift] or [A]:Then press the in the direction of the location you want to hack.::Hacking a location that is not a node will cause corruption,:the failed hack will raise you detection rating::Entering a node before you have hacked will cause a network alert,:you won't be able to hack that node and it will raise your detection level a lot.";
        private string text4 = "Your detection level is the main thing you want to worry about.:If it gets to 100 then you've been detected and it is game over.:Each location you visit will increase your detection rating by 1.:Follow your tracks when you can, moving over a location you have already visited is free.:As mentioned before, failed hacks and network alerts will also raise your detection level.";
        private string text5 = "Your main goal is to find all the compute nodes in the network,:but while you're in there you might also find some bitcoins. Take em'::You can add them to your wallet when you get out,:but you can also use them to activate special nodes.";
        private string text6 = "There are two types of special node::Relay:Use the relay to reset your trail,:this will reduce your detection level by resetting all your visited locations.:This will cost 5 bit coins.::Decryption Key:You will notice some locations that should be showing numbers are showing question marks instead.:If you visit these location they will decrypt, but also raise your detection level.:But using the Decryption Key will remove all of these for 5 bit coins.";
        private string text7 = "The only other thing you need to know is that there are two types of contract.:Standard and Flawless.:In flawless mode you can't make any mistakes,:one failed hack or network alert and it is all over.";
        private string text8 = "You're ready now.:Get to it, BitShifter";
    }    
}
