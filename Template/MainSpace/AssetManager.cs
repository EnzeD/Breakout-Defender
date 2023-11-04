using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    class AssetManager
    {

        private ContentManager _Content;
        public AssetManager(ContentManager pContent)
        {
            _Content = pContent;
        }
        public static Song MusicGameplay { get; private set; }
        public static SoundEffect SoundExplode { get; private set; }

        public static void Load(ContentManager pContent)
        {
            MusicGameplay = pContent.Load<Song>("techno");
            SoundExplode = pContent.Load<SoundEffect>("explode");
        }

        public Song MusicMenu { get { return _Content.Load<Song>("cool"); } }
        public SpriteFont MainFont { get { return _Content.Load<SpriteFont>("mainfont"); } }
        public Texture2D TexWhiteRectangle { get { return _Content.Load<Texture2D>("white rectangle"); } }
        public Texture2D TexWhiteCirle { get { return _Content.Load<Texture2D>("white circle"); } }
        public Texture2D TexWhiteVerticalBar { get { return _Content.Load<Texture2D>("white vertical bar"); } }
        public Texture2D TexWhiteLateralBar { get { return _Content.Load<Texture2D>("white lateral bar"); } }

    }
}
