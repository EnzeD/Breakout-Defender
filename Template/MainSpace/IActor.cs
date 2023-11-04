using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public interface IActor
    {
        Vector2 Position { get; }
        Rectangle BoundingBox { get; }
        void Update(GameTime pGameTime);
        void Draw(SpriteBatch pSpriteBatch);
        void TouchedBy(IActor pBy);
        bool ToRemove {  get; set; }

    }
}
