using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    class Border : Sprite
    {
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        public BorderSide Side { get; set; }
        public enum BorderSide
        {
            Top,
            Left,
            Right
        };
        public Border(Texture2D pTexture, BorderSide pSide) : base(pTexture)
        {
            Side = pSide;
        }

        public void LoadBorder(BorderSide pSide)
        {
            switch (pSide)
            {
                case BorderSide.Left:
                    Position = new Vector2(si.Width / 3, 45);
                    break;
                case BorderSide.Right:
                    Position = new Vector2(si.Width - si.Width / 3 - 1, 45);
                    break;
                case BorderSide.Top:
                    Position = new Vector2(si.Width / 3, 45);
                    break;
                default: break;
            }
        }
    }
}
