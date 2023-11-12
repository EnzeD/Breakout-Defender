using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Background : GUI
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        public Background(MainGame pGame) : base(pGame)
        {

        }
        public void Draw(GameTime gameTime)
        {
            // Level Up Background
            Texture2D rectTexture = am.TexBackground;
            Color texColor = Color.DarkViolet * 0.25f;

            Vector2 position = new Vector2(si.targetW / 3 + 10, 65);
            mainGame._spriteBatch.Draw(rectTexture, position, texColor);
        }
    }

}
