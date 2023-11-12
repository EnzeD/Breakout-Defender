using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    internal class BallsDisplay : GUI
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        private readonly List<IActor> listActors;
        private readonly List<Ball> listBalls = new List<Ball>();
        public BallsDisplay(MainGame pGame, List<IActor> pListActors) : base(pGame)
        {
            listActors = pListActors;

        }
        public void Update(GameTime gameTime)
        {
            // Extract only balls
            listBalls.Clear();
            listBalls.AddRange(listActors.OfType<Ball>());
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = mainGame._spriteBatch;

            // Position of first ball
            Vector2 position = new Vector2(si.targetW - si.targetW / 3 + 25, 65);

            foreach (var ball in listBalls)
            {
                // Draw ball
                spriteBatch.Draw(am.TexWhiteCirle, position, Color.White);

                // Draw cooldown bar is ball is lost
                if (ball.IsLost)
                {
                    // draw red cross on top of ball
                    spriteBatch.Draw(am.TexRedCross, position, Color.White);

                    // cooldown bar length
                    float fillAmount = (float)(1 - ball.relaunchCooldown / Ball.CooldownDuration);
                    Trace.WriteLine("fillAmount:" + fillAmount);
                    Trace.WriteLine("relaunchCooldown:" + ball.relaunchCooldown);
                    Rectangle cooldownBar = new Rectangle((int)position.X, (int)position.Y + am.TexWhiteCirle.Height + 5, am.TexWhiteCirle.Width, 5);
                    spriteBatch.Draw(am.TexWhitePixel, cooldownBar, Color.Black);

                    // filling
                    cooldownBar.Width = (int)(cooldownBar.Width * fillAmount);
                    spriteBatch.Draw(am.TexWhitePixel, cooldownBar, Color.White);
                }

                // Position for next ball
                position.Y += am.TexWhiteCirle.Height + 25; // Espacement entre les balles
            }
        }
    }
}
