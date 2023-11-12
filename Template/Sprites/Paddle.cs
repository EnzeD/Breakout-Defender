using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Paddle : Sprite
    {
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        private static float baseSpeed = 5;
        public static float BaseSpeed
        {
            get { return baseSpeed; }
            set
            {
                baseSpeed = value;
            }
        }

        public float Speed { get; set; }

        public Paddle(Texture2D pTexture, Color color, bool isCentered = true) : base(pTexture, color, isCentered)
        {
            WidthScale = 1f;
            Speed = BaseSpeed;
        }
        public void Load()
        {
            // Paddle Loading
            Position = new Vector2(si.targetW / 2, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN);
            Velocity = new Vector2(BaseSpeed, 0);
        }
        public void ExtendWidth(float percentage)
        {
            WidthScale += WidthScale * percentage;
        }
        public void ResetWidth()
        {
            WidthScale = 1f;
        }
        public static void IncreaseBaseSpeed(float percentage)
        {
            BaseSpeed += BaseSpeed * percentage;
        }
        public void UpdateSpeed()
        {
            Speed = BaseSpeed;
            Velocity = new Vector2(Speed, 0);
        }
    }
}
