using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnzedSpace
{
    class SceneGameover : Scene
    {
        private Song music;
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        public SceneGameover(MainGame pGame) : base(pGame)
        {

        }

        public override void Load()
        {
            music = mainGame.Content.Load<Song>("cool");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
            MediaPlayer.Volume = 0.2f;
            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
            base.UnLoad();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(am.MainFont, "Game Over!", new Vector2(1,1), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
