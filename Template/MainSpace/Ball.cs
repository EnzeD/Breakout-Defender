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
        public float Speed { get; private set; }
        public Vector2 PreviousPosition { get; private set; }
        private static readonly Random random = new Random();
        public Ball(Texture2D pTexture, Paddle pPaddle, Color color, bool isCentered) : base(pTexture, color, isCentered)
        {
            paddle = pPaddle;
            Position = new Vector2(paddle.Position.X, paddle.Position.Y - Height);
            Speed = 8;
            Velocity = new Vector2(1, -1);
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
            particleSystem.Update(ParticleSystem.ParticleEmitterType.Ball);

            base.Update(pGameTime);
            PreviousPosition = Position;
        }

        public override void Draw(SpriteBatch pSpriteBatch)
        {
            particleSystem.Draw(pSpriteBatch);
            base.Draw(pSpriteBatch);
        }
    }
}
