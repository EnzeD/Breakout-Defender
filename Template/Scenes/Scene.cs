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
        SpriteBatch sb = ServiceLocator.GetService<SpriteBatch>();

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
            // We update all actors
            foreach (IActor actor in listActors) 
            { 
                actor.Update(gameTime); 
            } 
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // We draw all actor, but we draw the borders first to be on top of bricks ;)
            List<IActor> sortedActors = listActors.OrderBy(actor => actor is Border).ToList();
            foreach (IActor actor in sortedActors)
            {
                actor.Draw(mainGame._spriteBatch);
            }
        }
    }
}
