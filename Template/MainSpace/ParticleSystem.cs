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
            Ball,
            Xp
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
        private Particle GenerateNewParticle(Vector2 location, ParticleEmitterType emitterType, Color pColor)
        {
            Texture2D texture = listTextures[random.Next(listTextures.Count)];
            if (emitterType == ParticleEmitterType.Brick)
            {
                Vector2 EmitterLocation = location;
            }
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
                        (float)(random.NextDouble() * 2 - 1),
                        (float)(random.NextDouble() * 2 - 1));
                angle = 0;
                angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                color = pColor * (float)random.NextDouble();
                color *= (float)random.NextDouble(); // to add some transparency
                float scale = (float)random.NextDouble();
                size = new Vector2(scale, scale);
                ttl = 5 + random.Next(5);
            }
            else if (emitterType == ParticleEmitterType.Xp)
            {
                position = EmitterLocation + new Vector2(random.Next(-5, 5), random.Next(-5, 5));
                velocity = new Vector2(
                        (float)(random.NextDouble() * 2 - 1),
                        (float)(random.NextDouble() * 2 - 1));
                angle = 0;
                angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                color = pColor * (float)random.NextDouble();
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
                color = pColor * (float)random.NextDouble();
                color *= (float)random.NextDouble(); // to add some transparency
                float scale = (float)random.NextDouble();
                size = new Vector2(scale, scale);
                ttl = 5 + random.Next(20);
            }
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }
        public void CreateParticles(Color color, Vector2 location, ParticleEmitterType emitterType)
        {
            listParticles.Add(GenerateNewParticle(location, emitterType, color));
        }

        public void CreateExplosion(Color color, Vector2 explosionLocation, int numberOfParticles, ParticleEmitterType emitterType)
        {
            this.EmitterLocation = explosionLocation;

            for (int i = 0; i < numberOfParticles; i++)
            {
                CreateParticles(color, explosionLocation, emitterType);
            }
        }

        // Update particles
        public void UpdateBricksParticle(ParticleEmitterType pType)
        {
            for (int particle = listParticles.Count - 1; particle >= 0; particle--)
            {
                listParticles[particle].Update();
                if (listParticles[particle].TimeToLive <= 0)
                {
                    listParticles.RemoveAt(particle);
                }
            }
        }
        public void UpdateBallsParticle(Vector2 pLocation, ParticleEmitterType pType)
        {
            int total = 3;
            Vector2 location = pLocation;

            for (int i = 0; i < total; i++)
            {
                listParticles.Add(GenerateNewParticle(location, pType, Color.White));
            }
            for (int particle = listParticles.Count - 1; particle >= 0; particle--)
            {
                listParticles[particle].Update();
                if (listParticles[particle].TimeToLive <= 0)
                {
                    listParticles.RemoveAt(particle);
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
