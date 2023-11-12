using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public abstract class Sprite : IActor
    {
        // Service Locator
        SpriteBatch sb = ServiceLocator.GetService<SpriteBatch>();

        // Sprite Properties
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public bool ToRemove { get; set; }
        public Vector2 Origin { get; private set; }
        public bool IsCentered { get; set; }
        public Rectangle BoundingBox { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public bool isVisible = true;
        public float WidthScale { get; set; } = 1f;

        // To make it easier to get values
        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public float VX { get { return Velocity.X; } }
        public float VY { get { return Velocity.Y; } }

        // Constructors
        public Sprite(Texture2D pTexture, Color color)
        {
            Texture = pTexture;
            ToRemove = false;
            IsCentered = false;
            Color = color;
        }
        public Sprite(Texture2D pTexture, Color color, bool isCentered)
        {
            Texture = pTexture;
            ToRemove = false;

            IsCentered = isCentered;
            if (IsCentered)
            {
                Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
            else
            {
                Origin = Vector2.Zero;
            }
            Color = color;
        }

        // Methods
        public void Move(float pX, float pY)
        {
            Position += new Vector2(pX, pY);
        }
        public void Move(Vector2 pVelocity)
        {
            Position += pVelocity;
        }
        public virtual void TouchedBy(IActor pBy)
        {
            // TO DO
        }

        // Update
        public virtual void Update(GameTime pGameTime)
        {
            BoundingBox = new Rectangle(
                IsCentered ? (int)Position.X - (int)(Texture.Width * WidthScale) / 2 : (int)Position.X,
                IsCentered ? (int)Position.Y - Texture.Height / 2 : (int)Position.Y,
                (int)(Texture.Width * WidthScale),
                Texture.Height
            );
        }

        // Draw
        public virtual void Draw(SpriteBatch pSpriteBatch)
        {
            if (isVisible)
            {
                Vector2 scale = new Vector2(WidthScale, 1);
                pSpriteBatch.Draw(Texture, Position, null, Color, 0, Origin, scale, SpriteEffects.None, 0);
            }

        }
    }
}
