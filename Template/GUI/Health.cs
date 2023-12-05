using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Health : GUI
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        public int PlayerHealth { get; set; } = 100;
        public const int HEART_VALUE = 20;
        private int heartSpacing = 3;
        int fullHearts;
        bool halfHeart;
        private const float BLINK_INTERVAL = 0.1f;
        private float blinkTimer = 0f;
        private bool blinkOn = true;
        private bool showRedFlash;
        private float flashTimer;

        public Health(MainGame pGame) : base(pGame) 
        {
            ServiceLocator.OnLoseHealth += TriggerRedFlash;
        }
        private void TriggerRedFlash()
        {
            showRedFlash = true;
            flashTimer = 0.5f;
        }

        public void UpdateHearts(GameTime gameTime)
        {
            fullHearts = PlayerHealth / HEART_VALUE;
            halfHeart = PlayerHealth % HEART_VALUE != 0;

            // if Health = 10 then blink
            if (PlayerHealth == 10)
            {
                blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (blinkTimer >= BLINK_INTERVAL)
                {
                    blinkOn = !blinkOn;
                    blinkTimer = 0f;
                }
            }
            else
            {
                blinkOn = true;
                blinkTimer = 0f;
            }
            // Flash red onLoseHealth
            if (showRedFlash)
            {
                flashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (flashTimer <= 0)
                {
                    showRedFlash = false;
                }
            }
        }

        public void DrawHearts()
        {
            if (PlayerHealth > 0)
            {
                for (int i = 0; i < fullHearts; i++)
                {
                    DrawFullHeart(i);
                }
                if (halfHeart)
                {
                    if (PlayerHealth == 10)
                    {
                        if (blinkOn)
                        {
                            DrawHalfHeart(fullHearts);
                        }
                        // else draw nothing
                    }
                    else
                    {
                        DrawHalfHeart(fullHearts);
                    }
                }
            }

            if (showRedFlash)
            {
                mainGame._spriteBatch.Draw(am.TexRedFlash, new Vector2(si.targetW / 2 - am.TexRedFlash.Width /2, 65), Color.White * 0.25f);
            }
        }
        private void DrawFullHeart(int position)
        {
            int quotient = position / 7;
            int xPosition = position % 7;
            int x = 200 + xPosition * (am.TexHeart.Width + heartSpacing);
            int y = si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 100 - quotient*(30);
            mainGame._spriteBatch.Draw(am.TexHeart, new Vector2(x,y), Color.White);
        }

        private void DrawHalfHeart(int position)
        {
            int quotient = position / 7;
            int xPosition = position % 7;
            int x = 200 + xPosition * (am.TexHeart.Width + heartSpacing);
            int y = si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 100 - quotient * (30);

            Rectangle halfHeartSource = new Rectangle(0, 0, am.TexHeart.Width / 2, am.TexHeart.Height);

            // Draw half heart
            mainGame._spriteBatch.Draw(am.TexHeart, new Vector2(x, y), halfHeartSource, Color.White);
        }



    }
}
