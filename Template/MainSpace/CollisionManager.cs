using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MainSpace
{
    public class CollisionManager
    {
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();

        private readonly List<IActor> listActors;
        private readonly List<Ball> listBalls = new List<Ball>();
        private readonly List<Brick> listBricks = new List<Brick>();
        private readonly List<Xp> listXp = new List<Xp>();
        private Paddle paddle;
        private Border leftBorder, rightBorder, topBorder, redLine;
        private float soundEffectsVolume = 0.04f;
        public CollisionManager(List<IActor> pListActors)
        {
            listActors = pListActors;

            paddle = listActors.OfType<Paddle>().FirstOrDefault();

            leftBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Left);
            rightBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Right);
            topBorder = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.Top);
            redLine = listActors.OfType<Border>().FirstOrDefault(b => b.Side == Border.BorderSide.None);
        }

        public void Update(ParticleSystem particleSystem)
        {
            // Extract only balls
            listBalls.Clear();
            listBalls.AddRange(listActors.OfType<Ball>());

            // Extract only bricks
            listBricks.Clear();
            listBricks.AddRange(listActors.OfType<Brick>());

            // Extract only xp
            listXp.Clear();
            listXp.AddRange(listActors.OfType<Xp>());

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

                foreach (Brick brick in listBricks)
                {
                    CheckBrickCollision(ball, brick, particleSystem);
                }

                // Collision with bottom screen
                if (ball.Y > si.targetH + 100) // 200 to allow the trail to not disappear
                {
                    ball.OnBallLost();
                }
            }
            // Collisions with game borders
            foreach (Xp xp in listXp)
            {
                CheckXpCollision(paddle, xp, particleSystem);

                // Collision with bottom screen
                if (xp.Y > si.targetH + xp.Width)
                {
                    xp.ToRemove = true;
                }
            }

            // Check bricks collision with red line
            foreach (Brick brick in listBricks)
            {
                CheckBrickCollision(brick, particleSystem);
            }
        }

        public void CheckBorderCollision(Ball ball)
        {
            // Collisions with game borders

            if (Util.CollideByBox(ball, leftBorder))
            {
                ball.Position = new Vector2(leftBorder.BoundingBox.Right + ball.Width / 2, ball.Position.Y);
                ball.Velocity *= new Vector2(-1, 1);
                PlayCollisionSound(soundEffectsVolume); 
            }

            if (Util.CollideByBox(ball, rightBorder))
            {
                ball.Position = new Vector2(rightBorder.BoundingBox.Left - ball.Width / 2, ball.Position.Y);
                ball.Velocity *= new Vector2(-1, 1);
                PlayCollisionSound(soundEffectsVolume);
            }

            if (Util.CollideByBox(ball, topBorder))
            {
                ball.Position = new Vector2(ball.Position.X, topBorder.BoundingBox.Bottom + ball.Height / 2);
                ball.Velocity *= new Vector2(1, -1);
                PlayCollisionSound(soundEffectsVolume);
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
                am.SndPaddleBlip.Play(soundEffectsVolume, 0.0f, 0.0f);
            }
        }

        public void CheckBrickCollision(Ball ball, Brick brick, ParticleSystem particleSystem)
        {
            if (Util.CollideByBox(ball, brick))
            {

                // Check for side collision first
                bool sideCollision = ball.PreviousPosition.X < brick.BoundingBox.Left || ball.PreviousPosition.X > brick.BoundingBox.Right;

                // Check for top or bottom collision
                bool topBottomCollision = ball.PreviousPosition.Y < brick.BoundingBox.Top ||
                                          ball.PreviousPosition.Y > brick.BoundingBox.Bottom;

                // Check for corner collision
                bool cornerCollision = sideCollision && topBottomCollision;
                if (cornerCollision)
                {
                    // Determine the direction of the ball
                    bool movingLeft = ball.Velocity.X < 0;
                    bool movingRight = ball.Velocity.X > 0;
                    bool movingUp = ball.Velocity.Y < 0;
                    bool movingDown = ball.Velocity.Y > 0;

                    // Top left corner
                    if (ball.Position.X < brick.BoundingBox.Left && ball.Position.Y < brick.BoundingBox.Top)
                    {
                        if (movingRight && movingDown)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingRight && movingUp)
                            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                        else if (movingLeft && movingDown)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                    }

                    // Top right corner
                    else if (ball.Position.X > brick.BoundingBox.Right && ball.Position.Y < brick.BoundingBox.Top)
                    {
                        if (movingLeft && movingDown)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingRight && movingDown)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingLeft && movingUp)
                            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                    }
                    // Bottom left corner
                    else if (ball.Position.X < brick.BoundingBox.Left && ball.Position.Y > brick.BoundingBox.Bottom)
                    {
                        if (movingRight && movingUp)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingLeft && movingDown)
                            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                        else if (movingLeft && movingUp)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                    }
                    // Bottom right corner
                    else if (ball.Position.X > brick.BoundingBox.Right && ball.Position.Y > brick.BoundingBox.Bottom)
                    {
                        if (movingLeft && movingUp)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingRight && movingUp)
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                        else if (movingRight && movingDown)
                            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                    }
                    // Reposition ball based on corner hit
                    ball.Position = AdjustBallPositionAfterCornerCollision(ball, brick);
                }
                else if (sideCollision)

                {
                    ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);

                    // Reposition the ball outside of the bounding box
                    if (ball.Position.X < brick.BoundingBox.Left)
                    {
                        ball.Position = new Vector2(brick.BoundingBox.Left - ball.Width / 2, ball.Position.Y);
                    }
                    else
                    {
                        ball.Position = new Vector2(brick.BoundingBox.Right + ball.Width / 2, ball.Position.Y);
                    }
                }
                // Top or bottom collision
                else if (topBottomCollision)
                {
                    ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);

                    // Reposition the ball outside of the bounding box
                    if (ball.Position.Y < brick.BoundingBox.Top)
                    {
                        ball.Position = new Vector2(ball.Position.X, brick.BoundingBox.Top - ball.Height / 2);
                    }
                    else
                    {
                        ball.Position = new Vector2(ball.Position.X, brick.BoundingBox.Bottom + ball.Height / 2);
                    }
                }
                PlayCollisionSound(soundEffectsVolume);
                // Remove the brick
                brick.ToRemove = true;

                if (brick.ToRemove)
                {
                    // Explosion
                    Vector2 ExplositionLocation = new Vector2(brick.X, brick.Y);
                    particleSystem.CreateExplosion(brick.Color, ExplositionLocation, numberOfParticles: 20, ParticleSystem.ParticleEmitterType.Brick);

                    // Xp
                    Random random = new Random();
                    int rnd = random.Next(0, 2);
                    if (rnd == 0)
                    {
                        Xp xp = new Xp(am.TexYellowSquare, Color.White, ExplositionLocation, true);
                        listActors.Add(xp);
                    }
                    am.SndBrickExplode.Play(soundEffectsVolume, 0.0f, 0.0f);
                }
                else
                {
                    am.SndBrickHit.Play(soundEffectsVolume, 0.0f, 0.0f);
                }
            }
        }

        // A helper method to adjust the ball's position after a corner collision
        Vector2 AdjustBallPositionAfterCornerCollision(Ball ball, Brick brick)
        {
            // Calculate half dimensions
            float halfBallWidth = ball.Width / 2;
            float halfBallHeight = ball.Height / 2;

            // Determine the correct position adjustment
            if (ball.Position.X < brick.BoundingBox.Left && ball.Position.Y < brick.BoundingBox.Top)
            {
                return new Vector2(brick.BoundingBox.Left - halfBallWidth, brick.BoundingBox.Top - halfBallHeight);
            }
            else if (ball.Position.X > brick.BoundingBox.Right && ball.Position.Y < brick.BoundingBox.Top)
            {
                return new Vector2(brick.BoundingBox.Right + halfBallWidth, brick.BoundingBox.Top - halfBallHeight);
            }
            else if (ball.Position.X < brick.BoundingBox.Left && ball.Position.Y > brick.BoundingBox.Bottom)
            {
                return new Vector2(brick.BoundingBox.Left - halfBallWidth, brick.BoundingBox.Bottom + halfBallHeight);
            }
            else if (ball.Position.X > brick.BoundingBox.Right && ball.Position.Y > brick.BoundingBox.Bottom)
            {
                return new Vector2(brick.BoundingBox.Right + halfBallWidth, brick.BoundingBox.Bottom + halfBallHeight);
            }
            return ball.Position; // Default return, in case no corner collision was detected
        }

        public void CheckBrickCollision(Brick brick, ParticleSystem particleSystem)
        {
            if (Util.CollideBottomWithTop(redLine, brick)) // Check if the bottom of the line collides with the top of the brick
            {
                ServiceLocator.LoseHealth();
                brick.ToRemove = true;

                // Explosion
                Vector2 ExplositionLocation = new Vector2(brick.X, brick.Y);
                particleSystem.CreateExplosion(brick.Color, ExplositionLocation, numberOfParticles: 20, ParticleSystem.ParticleEmitterType.Brick);
            }
        }

        public void CheckXpCollision(Paddle paddle, Xp xp, ParticleSystem particleSystem)
        {
            if (Util.CollideByBox(paddle, xp))
            {
                ServiceLocator.Xp += 5;
                xp.ToRemove = true;
                am.SndXp.Play(soundEffectsVolume, 0.0f, 0.0f);
                // Explosion
                xp.Color = new Color(255f, 206f, 0f, 256f);
                Vector2 ExplositionLocation = new Vector2(xp.X, xp.Y);
                particleSystem.CreateExplosion(xp.Color, ExplositionLocation, numberOfParticles: 20, ParticleSystem.ParticleEmitterType.Brick);
            }
        }

        private void PlayCollisionSound(float volume)
        {
            // Collision Sound
            SoundEffect collisionSound = am.SndBlip;
            collisionSound.Play(volume, 0.0f, 0.0f);
        }
    }
}



