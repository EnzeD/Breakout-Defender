using EnzedSpace;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace EnzedSpace
{
    public class CollisionManager
    {
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        private readonly List<IActor> listActors;
        private readonly List<Ball> listBalls = new List<Ball>();
        private Paddle paddle;
        private Border leftBorder, rightBorder, topBorder;
        public CollisionManager(List<IActor> pListActors)
        {
            listActors = pListActors;

            paddle = listActors.OfType<Paddle>().FirstOrDefault();

            leftBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Left);
            rightBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Right);
            topBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Top);
        }

        public void Update()
        {
            listBalls.Clear();

            // Extract only balls
            listBalls.AddRange(listActors.OfType<Ball>());

            // Collisions with game borders
            foreach (Ball ball in listBalls)
            {
                // Collision with borders
                CheckBorderCollision(ball);

                // Collision with paddle
                if (ball.bIsLaunched)
                {
                    CheckPaddleCollision(ball);
                }

                // Collision with bottom screen
                if (ball.Y > si.Height)
                {
                    ball.ToRemove = true;
                }
            }
        }

        public void CheckBorderCollision(Ball ball)
        {
            // Collisions with game borders

            if (Util.CollideByBox(ball, leftBorder))
            {
                ball.Position = new Vector2(leftBorder.BoundingBox.Right + ball.Width / 2, ball.Position.Y);
                ball.Velocity *= new Vector2(-1, 1);
            }

            if (Util.CollideByBox(ball, rightBorder))
            {
                ball.Position = new Vector2(rightBorder.BoundingBox.Left - ball.Width / 2, ball.Position.Y);
                ball.Velocity *= new Vector2(-1, 1);
            }

            if (Util.CollideByBox(ball, topBorder))
            {
                ball.Position = new Vector2(ball.Position.X, topBorder.BoundingBox.Bottom + ball.Height / 2);
                ball.Velocity *= new Vector2(1, -1);
            }
        }

        public void CheckPaddleCollision(Ball ball)
        {
            // Collision with paddle

            if (Util.CollideByBox(ball, paddle))
            {
                paddle.TouchedBy(ball); // for later
                ball.TouchedBy(paddle); // for later

                // When collide, put the ball outside of the boundingbox
                ball.Position = new Vector2(ball.Position.X, paddle.Position.Y - ball.Height);

                // Relative position between ball and paddle
                float distance = ball.Position.X - paddle.Position.X;

                // Normalize to get between - 1 and 1
                float normalizedDistance = distance / (paddle.Width / 2);
                normalizedDistance = MathHelper.Clamp(normalizedDistance, -1, 1);

                // Angle definition
                float maxAngle = MathHelper.ToRadians(60);

                // Calculate angle based on impact point
                float angle = normalizedDistance * maxAngle;

                // Update Velocity
                float vx = ball.Speed * (float)Math.Sin(angle);
                float vy = -ball.Speed * (float)Math.Cos(angle);
                ball.Velocity = new Vector2(vx, vy);
                float speedRatio = ball.Speed / ball.Velocity.Length(); // To keep the same speed at all time
                ball.Velocity *= speedRatio;
            }
        }
    }
}



