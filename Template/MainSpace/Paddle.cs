using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Paddle : Sprite
    {
        protected const int DIST_FROM_BOTTOM_SCREEN = 55;
        public Paddle(Texture2D pTexture) : base(pTexture)
        {

        }
        public Paddle(Texture2D pTexture, bool isCentered = true) : base(pTexture, isCentered)
        {

        }
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        public void Load()
        {
            // Paddle Loading
            Position = new Vector2(si.Width / 2, si.Height - DIST_FROM_BOTTOM_SCREEN);
            Velocity = new Vector2(8, 0);
        }
    }
}
