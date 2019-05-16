using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeper
{
    public class SoundManager
    {
        private readonly ContentManager _contentManager;
        private Song _mainTheme;

        public SoundManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void Initialise()
        {
            _mainTheme = _contentManager.Load<Song>("bit_shifter");
        }

        public void Play()
        {
            if(MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(_mainTheme);
        }
    }
}
