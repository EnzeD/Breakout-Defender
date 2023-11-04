﻿using Microsoft.Xna.Framework;
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
        // Sprite Properties
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; }
        public bool ToRemove { get; set; }
        public Vector2 Origin { get; private set; }
        public bool IsCentered { get; set; }
        public Rectangle BoundingBox { get; set; }
        public Vector2 Velocity { get; set; }

        // To make it easier to get values
        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public float VX { get { return Velocity.X; } }
        public float VY { get { return Velocity.Y; } }

        // Constructors
        public Sprite(Texture2D pTexture)
        {
            Texture = pTexture;
            ToRemove = false;
            IsCentered = false;
        }
        public Sprite(Texture2D pTexture, bool isCentered = true)
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
                IsCentered ? (int)Position.X - Texture.Width / 2 : (int)Position.X,
                IsCentered ? (int)Position.Y - Texture.Height / 2 : (int)Position.Y,
                Texture.Width,
                Texture.Height
                );
        }

        // Draw
        public virtual void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(Texture, Position, null, Color.White, 0, Origin, 1, SpriteEffects.None, 0);
        }
    }
}