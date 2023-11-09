using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Particle : Sprite
    {
        public float Angle { get; set; }           
        public float AngularVelocity { get; set; }
        public Vector2 Size { get; set; }
        public int TimeToLive { get; set; }              
        public Particle(Texture2D pTexture, Vector2 pPosition, Vector2 pVelocity,
            float pAngle, float pAngularVelocity, Color pColor, Vector2 pSize, int pTtl, bool isCentered = true) : base(pTexture, pColor, isCentered)
        {
            Texture = pTexture;
            Position = pPosition;
            Velocity = pVelocity;
            Angle = pAngle;
            AngularVelocity = pAngularVelocity;
            Color = pColor;
            Size = pSize;
            TimeToLive = pTtl;
        }
    public void Update()
        {
            float scaleReductionRate = 0.995f;

            TimeToLive--;
            if (Size.X > 0.1f && Size.Y > 0.1f) // to avoid reducing it to 0
            {
                Size *= scaleReductionRate; 
            }
            Move(Velocity);
            Angle += AngularVelocity;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, Origin, Size, SpriteEffects.None, 0f);
        }
    }
}
