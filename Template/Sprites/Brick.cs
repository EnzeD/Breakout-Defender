using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MainSpace
{
    public class Brick : Sprite
    {
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();

        private static readonly Random random = new Random();

        public static float brickSpeed = 0.3f;
        /*
        private static float speedIncrement = 0.01f; 
        private static double timeSinceLastSpeedIncrease = 0; 
        private static double speedIncreaseInterval = 10; 
        */

        public Brick(Texture2D pTexture, Color color, bool isCentered = true) : base(pTexture, color, isCentered)
        {
            
        }
        public static List<Brick> CreateBricks(List<IActor> listActors, Texture2D pTexture, int pStartX, int pStartY, int pSpacing, int pBricksPerRow, int pStartNumberOfRows,int pNumberOfRows, float pBrickSpeed)
        {
            List<Brick> bricks = new List<Brick>();
            int spacing = pSpacing;
            int brickWidth = pTexture.Width;
            int brickHeight = pTexture.Height;
            int startX = pStartX + pSpacing;
            int startY = pStartY + pSpacing + (pStartNumberOfRows - 1) * (brickHeight + spacing);
            int bricksPerRow = pBricksPerRow;
            int numberOfRows = pNumberOfRows;
            brickSpeed = pBrickSpeed;
            Color color;

            Random rand = new Random();

            for (int row = 0; row < numberOfRows; row++)
            {
                for (int col = 0; col < bricksPerRow; col++)
                {
                    int x = startX + col * (brickWidth + spacing);
                    int y = startY - row * (brickHeight + spacing);

                    int randomNumber = rand.Next(0, 4);
                    switch (randomNumber)
                    {
                        case 0:
                            color = Color.Yellow;
                            break;
                        case 1:
                            color = Color.Cyan;
                            break;
                        case 2:
                            color = Color.LawnGreen;
                            break;
                        case 3:
                            color = Color.DeepPink;
                            break;
                        default: 
                            color = Color.White;
                            break;
                    }
                    Brick brick = new Brick(pTexture, color, true);
                    brick.Position = new Vector2(x, y);
                    brick.isVisible = false;
                    listActors.Add(brick);
                    bricks.Add(brick);
                }
            }
            return bricks;
        }
        public static void RemoveRandomBricks(List<IActor> listActors, int numberOfBricksToRemove)
        {
            List<Brick> listBricks = listActors.OfType<Brick>().ToList();
            int numberOfBricks = listBricks.Count;

            // To avoid removing more than the number of bricks
            numberOfBricksToRemove = Math.Min(numberOfBricksToRemove, numberOfBricks);

            for (int i = 0; i < numberOfBricksToRemove; i++)
            {
                if (numberOfBricks > 0)
                {
                    int randomIndex = random.Next(listBricks.Count);
                    Brick brickToRemove = listBricks[randomIndex];

                    listActors.Remove(brickToRemove);

                    listBricks.RemoveAt(randomIndex);
                }
            }
        }
        public override void Update(GameTime pGameTime)
        {
            Position += new Vector2(0, brickSpeed);

            if (Position.Y >= 55)
            {
                isVisible = true;
            }

            base.Update(pGameTime);
        }
        /*
        public static void UpdateBrickSpeed(GameTime pGameTime)
        {
            timeSinceLastSpeedIncrease += pGameTime.ElapsedGameTime.TotalSeconds;

            if (timeSinceLastSpeedIncrease >= speedIncreaseInterval)
            {
                brickSpeed += speedIncrement; 
                timeSinceLastSpeedIncrease -= speedIncreaseInterval; 
            }
        }
        */
    }

}
