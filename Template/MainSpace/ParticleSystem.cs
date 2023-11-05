using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 
 TO DO: améliorer en rajoutant une liste de liste de particuliers + une méthode event  
 TO DO: améliorer en si EmitterType = Brick, pour que les particules occupent toute la brique  
 Fix: brick particle not deleting themselves*
*/

namespace MainSpace
{
    public class ParticleSystem
    {
        public enum ParticleEmitterType
        {
            Brick,
            Ball
        }
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> listParticles;
        private List<Texture2D> listTextures;
        public ParticleSystem(List<Texture2D> pListTextures, Vector2 pLocation)
        {
            EmitterLocation = pLocation;
            this.listTextures = pListTextures;
            this.listParticles = new List<Particle>();
            random = new Random();
        }
        // Method to generate particles
        private Particle GenerateNewParticle(ParticleEmitterType emitterType)
        {
            Texture2D texture = listTextures[random.Next(listTextures.Count)];
            Vector2 position;
            Vector2 velocity;
            Color color;
            Vector2 size;
            float angle;
            float angularVelocity;
            int ttl;

            if (emitterType == ParticleEmitterType.Brick)
            {
                position = EmitterLocation + new Vector2(random.Next(-20, 20), random.Next(-10,10));
                velocity = new Vector2(
                        (float)(random.NextDouble() * 2 - 1) / 4,
                        (float)(random.NextDouble() * 2 - 1) / 4);
                angle = 0;
                angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
                color *= (float)random.NextDouble(); // to add some transparency
                float scale = (float)random.NextDouble();
                size = new Vector2(scale, scale);
                ttl = 3 + random.Next(3);
            }
            else // (emitterType == ParticleEmitterType.Ball) 
            {
                position = EmitterLocation;
                velocity = new Vector2(
                        (float)(random.NextDouble() * 2 - 1) / 4,
                        (float)(random.NextDouble() * 2 - 1) / 4);
                angle = 0;
                angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
                color *= (float)random.NextDouble(); // to add some transparency
                float scale = (float)random.NextDouble();
                size = new Vector2(scale, scale);
                ttl = 5 + random.Next(20);
            }
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }
        public void CreateParticles()
        {
                listParticles.Add(GenerateNewParticle(ParticleEmitterType.Brick));
        }
        // Update particles
        public void Update(ParticleEmitterType pType)
        {
            int total = 3;

            for (int i = 0; i < total; i++)
            {
                listParticles.Add(GenerateNewParticle(pType));
            }

            for (int particle = 0; particle < listParticles.Count; particle++)
            {
                listParticles[particle].Update();
                if (listParticles[particle].TimeToLive <= 0)
                {
                    listParticles.RemoveAt(particle);
                    particle--;
                }
            }
        }
        // Draw particles
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < listParticles.Count; index++)
            {
                listParticles[index].Draw(spriteBatch);
            }
        }
    }
}
