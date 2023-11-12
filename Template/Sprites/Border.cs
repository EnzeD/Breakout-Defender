using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Border : Sprite
    {
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        public BorderSide Side { get; set; }
        public enum BorderSide
        {
            Top,
            Left,
            Right,
            None
        };
        public Border(Texture2D pTexture, BorderSide pSide, Color color, bool isCentered = true) : base(pTexture, color, isCentered)
        {
            Side = pSide;
        }

        public void LoadBorder(BorderSide pSide)
        {
            switch (pSide)
            {
                case BorderSide.Left:
                    Position = new Vector2(si.targetW / 3, si.targetH / 2);
                    break;
                case BorderSide.Right:
                    Position = new Vector2(si.targetW - si.targetW / 3 + 2, si.targetH / 2);
                    break;
                case BorderSide.Top:
                    Position = new Vector2(si.targetW / 2, 55);
                    break;
                case BorderSide.None:
                    Position = new Vector2(si.targetW / 2 + 1, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 95);
                    break;
                default: break;
            }
        }
    }
}
