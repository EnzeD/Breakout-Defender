using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Xp : Sprite
    {
        public Xp(Texture2D texture, Color color, Vector2 pPosition, bool isCentered = true): base(texture, color, isCentered)
        {
            Position = pPosition;
            Velocity = new Vector2(0, 2f);
        }

        public override void Update(GameTime pGameTime)
        {
            Move(Velocity);
            base.Update(pGameTime);
        }

        public override void Draw(SpriteBatch pSpriteBatch)
        {
            base.Draw(pSpriteBatch);
        }
    }
}
