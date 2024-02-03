using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Stats : GUI
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        List<IActor> listActors;
        Paddle paddle;

        // Constructor
        public Stats(MainGame pGame, List<IActor> pListActors) : base(pGame)
        {
            listActors = pListActors;
            paddle = listActors.OfType<Paddle>().FirstOrDefault();
        }

        public void Draw()
        {
            int StartX = si.targetW - si.targetW / 3 + 25;
            int StartY = si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 10;

            mainGame._spriteBatch.DrawString(am.MainFont, "Total Balls : " + Ball.totalBallsDesired.ToString(), new Vector2(StartX, StartY), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "Ball Speed : " + ((int)(Ball.BaseSpeed*10)).ToString(), new Vector2(StartX, StartY - 30), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "Brick Speed : " + (Brick.brickSpeed*10).ToString("0.0"), new Vector2(StartX, StartY - 60), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "Paddle Speed : " + ((int)(Paddle.BaseSpeed*10)).ToString(), new Vector2(StartX, StartY - 90), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "Paddle Width : " + ((int)paddle.GetWidth()).ToString(), new Vector2(StartX, StartY - 120), Color.White);
        }
    }
}
