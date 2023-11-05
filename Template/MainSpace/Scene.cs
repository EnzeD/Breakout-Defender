using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public abstract class Scene
    {
        protected MainGame mainGame;
        protected List<IActor> listActors;

        public Scene(MainGame pGame)
        {
            mainGame = pGame;
            listActors = new List<IActor>();
        }

        public void Clean()
        {
            listActors.RemoveAll(item => item.ToRemove == true);
        }

        public virtual void Load()
        {

        }

        public virtual void UnLoad()
        {

        }

        public virtual void Update(GameTime gameTime) 
        {


            foreach (IActor actor in listActors) 
            { 
                actor.Update(gameTime); 
            } 
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            List<IActor> sortedActors = listActors.OrderBy(actor => actor is Border).ToList();
            foreach (IActor actor in sortedActors)
            {
                actor.Draw(mainGame._spriteBatch);
            }
        }
    }
}
