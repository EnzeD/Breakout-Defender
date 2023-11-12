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
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();

        // Particles
        ParticleSystem particleSystem;
        
        Paddle paddle;
        public bool bIsLaunched = false;
        public bool IsLost { get; private set; }
        public bool IsRelaunchScheduled { get; private set; }
        public double relaunchCooldown;
        public static int totalBallsDesired = 1;
        private static float baseSpeed = 5;
        public static float CooldownDuration = 2.0f;
        public static float BaseSpeed
        {
            get { return baseSpeed; }
            set
            {
                baseSpeed = value;
            }
        }
        public float Speed { get; set; }
        public Vector2 PreviousPosition { get; private set; }
        public Ball(Texture2D pTexture, Paddle pPaddle, Color color, bool isCentered) : base(pTexture, color, isCentered)
        {
            paddle = pPaddle;
            Position = new Vector2(paddle.Position.X, paddle.Position.Y - Height);
            Velocity = new Vector2(1, -1);
            Speed = BaseSpeed;
            float speedRatio = Speed / Velocity.Length(); // To make sure Velocity = Speed
            Velocity *= speedRatio;
            // Particles Loading
            List<Texture2D> listParticleTextures = new List<Texture2D>();
            listParticleTextures.Add(am.TexCircleParticle);
            listParticleTextures.Add(am.TexStarParticle);
            listParticleTextures.Add(am.TexDiamondParticle);
            particleSystem = new ParticleSystem(listParticleTextures, new Vector2(400, 240));
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

            // Update particles
            particleSystem.EmitterLocation = Position;
            particleSystem.UpdateBallsParticle(Position, ParticleSystem.ParticleEmitterType.Ball);


            base.Update(pGameTime);
            PreviousPosition = Position;
        }

        public override void Draw(SpriteBatch pSpriteBatch)
        {
            particleSystem.Draw(pSpriteBatch);
            base.Draw(pSpriteBatch);
        }
        public static void IncreaseBaseSpeed(float percentage)
        {
            BaseSpeed += BaseSpeed * percentage;
        }
        public void SetVelocityDirection()
        {
            Velocity = Vector2.Normalize(Velocity) * Speed;
        }
        public void Relaunch()
        {
            IsRelaunchScheduled = false;
            IsLost = false;
            bIsLaunched = true;
            isVisible = true;
            Position = new Vector2(paddle.Position.X, paddle.Position.Y - Height);
                                                                                  
        }
        public void ScheduleRelaunch(double delay)
        {
            IsRelaunchScheduled = true;
            relaunchCooldown = delay;
        }
        public void UpdateRelaunchCooldown(GameTime gameTime)
        {
            if (IsRelaunchScheduled)
            {
                relaunchCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (relaunchCooldown <= 0)
                {
                    Relaunch();
                }
            }
        }
        public void OnBallLost()
        {
            if (!IsRelaunchScheduled)
            {
                ScheduleRelaunch(CooldownDuration);
                IsLost = true;
                isVisible = false;
            }
        }
    }
}
