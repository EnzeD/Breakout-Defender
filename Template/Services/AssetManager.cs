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
        private Song[] musics;
        private Random random = new Random();
        private int lastMusicIndex = -1;
        public void LoadMusics()
        {
            musics = new Song[7];
            musics[0] = Music1Loop;
            musics[1] = Music2Loop;
            musics[2] = Music3Loop;
            musics[3] = Music4Loop;
            musics[4] = Music5Loop;
            musics[5] = Music6Loop;
            musics[6] = Music7Loop;
        }
        public void PlayRandomMusic()
        {
            // To not play the same musique when we try a new random
            int musicIndex;
            do
            {
                musicIndex = random.Next(musics.Length);
            }
            while (musics.Length > 1 && musicIndex == lastMusicIndex);

            lastMusicIndex = musicIndex;

            Song musicToPlay = musics[musicIndex];

            MediaPlayer.Play(musicToPlay);
            MediaPlayer.Volume = 0.02f;
            MediaPlayer.IsRepeating = true;
        }

        public static void Load(ContentManager pContent)
        {
            MusicGameplay = pContent.Load<Song>("musics/techno");
            SoundExplode = pContent.Load<SoundEffect>("sounds/explode");
        }

        // Musics
        public Song MusicMenu { get { return _Content.Load<Song>("musics/cool"); } }
        public Song Music1Loop { get { return _Content.Load<Song>("musics/music1loop"); } }
        public Song Music2Loop { get { return _Content.Load<Song>("musics/music2loop"); } }
        public Song Music3Loop { get { return _Content.Load<Song>("musics/music3loop"); } }
        public Song Music4Loop { get { return _Content.Load<Song>("musics/music4loop"); } }
        public Song Music5Loop { get { return _Content.Load<Song>("musics/music5loop"); } }
        public Song Music6Loop { get { return _Content.Load<Song>("musics/music6loop"); } }
        public Song Music7Loop { get { return _Content.Load<Song>("musics/music7loop"); } }

        // SoundEffects
        public SoundEffect SndBlip { get { return _Content.Load<SoundEffect>("sounds/blip"); } }
        public SoundEffect SndPaddleBlip { get { return _Content.Load<SoundEffect>("sounds/paddleblip"); } }
        public SoundEffect SndXp { get { return _Content.Load<SoundEffect>("sounds/xp"); } }
        public SoundEffect SndBrickHit { get { return _Content.Load<SoundEffect>("sounds/brickhit"); } }
        public SoundEffect SndBrickExplode { get { return _Content.Load<SoundEffect>("sounds/brickexplode"); } }

        // Fonts
        public SpriteFont MainFont { get { return _Content.Load<SpriteFont>("fonts/mainfont"); } }
        public SpriteFont TitleFont { get { return _Content.Load<SpriteFont>("fonts/titlefont"); } }
        public SpriteFont DescriptionFont { get { return _Content.Load<SpriteFont>("fonts/descriptionfont"); } }

        // Textures
        public Texture2D TexWhiteRectangle { get { return _Content.Load<Texture2D>("textures/white rectangle"); } }
        public Texture2D TexWhitePixel { get { return _Content.Load<Texture2D>("textures/pixel texture"); } }
        public Texture2D TexWhiteCirle { get { return _Content.Load<Texture2D>("textures/white circle"); } }
        public Texture2D TexWhiteVerticalBar { get { return _Content.Load<Texture2D>("textures/white vertical bar"); } }
        public Texture2D TexWhiteLateralBar { get { return _Content.Load<Texture2D>("textures/white lateral bar"); } }
        public Texture2D TexWhiteBrick { get { return _Content.Load<Texture2D>("textures/white brick"); } }
        public Texture2D TexCircleParticle { get { return _Content.Load<Texture2D>("textures/circle"); } }
        public Texture2D TexStarParticle { get { return _Content.Load<Texture2D>("textures/star"); } }
        public Texture2D TexDiamondParticle { get { return _Content.Load<Texture2D>("textures/diamond"); } }
        public Texture2D TexYellowSquare { get { return _Content.Load<Texture2D>("textures/yellow square"); } }
        public Texture2D TexXpBarBorder { get { return _Content.Load<Texture2D>("textures/white xp bar border"); } }
        public Texture2D TexXpBarGreen { get { return _Content.Load<Texture2D>("textures/green xp bar"); } }
        public Texture2D TexRedLine { get { return _Content.Load<Texture2D>("textures/red line"); } }
        public Texture2D TexHeart { get { return _Content.Load<Texture2D>("textures/heart"); } }
        public Texture2D TexRedFlash { get { return _Content.Load<Texture2D>("textures/red flash"); } }
        public Texture2D TexAbstractLines { get { return _Content.Load<Texture2D>("textures/abstractlines"); } }
        public Texture2D TexBackground { get { return _Content.Load<Texture2D>("textures/background"); } }
        public Texture2D TexAddBall { get { return _Content.Load<Texture2D>("textures/addball"); } }
        public Texture2D TexExtendPaddle { get { return _Content.Load<Texture2D>("textures/extendpaddle"); } }
        public Texture2D TexSpeedBall { get { return _Content.Load<Texture2D>("textures/speedball"); } }
        public Texture2D TexUpgradeBorders { get { return _Content.Load<Texture2D>("textures/powerup rectangle"); } }
        public Texture2D TexUpgradeBackground { get { return _Content.Load<Texture2D>("textures/upgrade rectangle"); } }
        public Texture2D TexPaddleSpeed { get { return _Content.Load<Texture2D>("textures/speedpaddle"); } }
        public Texture2D TexRedCross { get { return _Content.Load<Texture2D>("textures/red cross"); } }
        public Texture2D TexLogo { get { return _Content.Load<Texture2D>("textures/breakout logo"); } }
        public Texture2D TexGameOver { get { return _Content.Load<Texture2D>("textures/gameover"); } }
        public Texture2D TexVictory { get { return _Content.Load<Texture2D>("textures/victory"); } }


    }
}
