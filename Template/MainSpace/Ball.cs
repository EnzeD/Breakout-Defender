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
    public class Ball : Sprite
    {
        Paddle paddle;
        public bool bIsLaunched = false;
        public float Speed { get; private set; }
        private static readonly Random random = new Random();
        public Ball(Texture2D pTexture, Paddle pPaddle, bool isCentered) : base(pTexture, isCentered)
        {
            paddle = pPaddle;
            Position = new Vector2(paddle.Position.X, paddle.Position.Y - Height);
            Speed = 8;
            Velocity = new Vector2(1, -1);
            float speedRatio = Speed / Velocity.Length(); // To make sure Velocity = Speed
            Velocity *= speedRatio;
        }

        public void Load()
        {

        }
        public override void Update(GameTime pGameTime)
        {
            // Ball position
            if (bIsLaunched == false)
            {
                Position = new Vector2(paddle.Position.X, paddle.Position.Y - Height);
            }
            else
            {
                Move(Velocity);
            }
            

            base.Update(pGameTime);
        }
    }
}
