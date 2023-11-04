using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Spritebatch
    {
        private SpriteBatch _spriteBatch;

        public Spritebatch(SpriteBatch pSpriteBatch)
        {
            _spriteBatch = pSpriteBatch;
        }
        
        public void Begin()
        {
            _spriteBatch.Begin();
        }
        public void Draw(Texture2D pTex, Vector2 pVec, Color pCol)
        {
            _spriteBatch.Draw(pTex, pVec, pCol);
        }
        public void End()
        {
            _spriteBatch.End();
        }
    }
}
