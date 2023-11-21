using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public  class Upgrader : GUI
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        // Class fields
        private int selectedIndex = -1;
        KeyboardState previousKeyboardState;
        MouseState previousMouseState;
        int padding = 10;
        int upgradeHeight = 100;
        int upgradeWidth = 400;
        private bool shouldCloseMenu = false;
        private List<IActor> listActors;
        UpgradeOption[] upgrades;
        UpgradeOption[] selectedPowerUps;
        Paddle paddle;

        // Constructor
        public Upgrader(MainGame pGame, List<IActor> pListActors) : base(pGame)
        {
            listActors = pListActors;

            upgrades = new UpgradeOption[]
            {
                new UpgradeOption
                {
                    Image = am.TexAddBall,
                    Title = "New Ball",
                    Description = "Add a new ball to your arsenal!",
                    BonusAction = AddBall
                },
                new UpgradeOption
                {
                    Image = am.TexSpeedBall,
                    Title = "Faster Ball",
                    Description = "+ 10% faster balls!",
                    BonusAction = IncreaseBallSpeed
                },
                new UpgradeOption
                {
                    Image = am.TexExtendPaddle,
                    Title = "Wider Paddle",
                    Description = "+10% wider!",
                    BonusAction = ExtendPaddle
                },
                new UpgradeOption
                {
                    Image = am.TexPaddleSpeed,
                    Title = "Faster Paddle",
                    Description = "+10% faster!",
                    BonusAction = IncrasePaddleSpeed
                }
                // more upgrades to come
                /* lower danger zone
                 * type: explosive balls
                 * lateral portals
                 * fire projectiles on rebound
                 * ball damages
                 * type: ball splits
                 * Attrack XP
                 * More Health
                 * Ball cooldown
                 */
            };

            paddle = listActors.OfType<Paddle>().FirstOrDefault();
        }

        // Rainbow color for level up
        private Color[] rainbowColors = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Indigo,
            Color.Violet
        };
        private double colorLerpProgress = 0;
        private double colorLerpStep = 2; // Speed of color change
        private int currentColorIndex = 0;

        // Upgrade list TO DO: to import via Json
        public struct UpgradeOption
        {
            public Texture2D Image;
            public string Title;
            public string Description;
            public Action BonusAction;
        }


        public void ShowLevelUpOptions()
        {
            Random rnd = new Random();
            selectedPowerUps = new UpgradeOption[3];

            // List to keep track of selected options
            HashSet<int> selectedIndices = new HashSet<int>();

            for (int i = 0; i < selectedPowerUps.Length; i++)
            {
                int randomIndex;
                // to avoid picked twice an upgrade
                do
                {
                    randomIndex = rnd.Next(upgrades.Length);
                }
                while (selectedIndices.Contains(randomIndex));

                selectedIndices.Add(randomIndex); 
                selectedPowerUps[i] = upgrades[randomIndex];
            }

            // Reset for next time
            selectedIndex = -1;
        }

        public void Update(GameTime gameTime)
        {
            // TODO: Add Selection system

            // Rainbow color for level up management
            colorLerpProgress += colorLerpStep * gameTime.ElapsedGameTime.TotalSeconds;

            if (colorLerpProgress >= 1)
            {
                colorLerpProgress = 0;
                currentColorIndex = (currentColorIndex + 1) % rainbowColors.Length; 
            }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = mainGame._spriteBatch;

            // Level Up Background
            Color startColor = rainbowColors[currentColorIndex];
            Color endColor = rainbowColors[(currentColorIndex + 1) % rainbowColors.Length];
            Color currentColor = Color.Lerp(startColor, endColor, (float)colorLerpProgress) * 0.25f; // transparence de 75%
            Texture2D rectTexture = am.TexBackground;
            Vector2 position = new Vector2(si.targetW / 3 + 10, 65);
            spriteBatch.Draw(rectTexture, position, currentColor);

            string message = "Level Up!";
            Vector2 textSize = am.TitleFont.MeasureString(message);
            spriteBatch.DrawString(am.TitleFont, message, new Vector2(si.targetW / 2 - textSize.X /2, 100), Color.White);

            DrawUpgrades(selectedPowerUps, selectedIndex);

            // Upgrade Selection
            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            // Through Keyboard
            if (state.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
            {
                selectedIndex++;
                if (selectedIndex >= selectedPowerUps.Length) selectedIndex = 0;
            }
            else if (state.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = selectedPowerUps.Length - 1;
            }

            Vector2 basePosition = new Vector2(si.targetW / 3 + 10, 250);

            // Through mouse
            for (int i = 0; i < selectedPowerUps.Length; i++)
            {
                int upgradeDrawY = (int)(basePosition.Y + i * (upgradeHeight + padding));
                Rectangle upgradeRect = new Rectangle((int)basePosition.X, upgradeDrawY, upgradeWidth, upgradeHeight);

                if (upgradeRect.Contains(mouseState.Position))
                {
                    selectedIndex = i;

                    if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        ApplyUpgrade(selectedIndex);
                        shouldCloseMenu = true;

                    }
                }
            }
            if (state.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter) && selectedIndex!=-1)
            {
                ApplyUpgrade(selectedIndex);
                shouldCloseMenu = true;
            }

            previousKeyboardState = state;
            previousMouseState = mouseState;
        }
        public bool ShouldCloseMenu()
        {
            return shouldCloseMenu;
        }
        public void ResetCloseMenuState()
        {
            shouldCloseMenu = false;
        }


        public void DrawUpgrades(UpgradeOption[] upgrades, int selectedIndex)
        {
            SpriteBatch spriteBatch = mainGame._spriteBatch;

            // Position of first upgrade
            Vector2 position = new Vector2(si.targetW / 3 + 15, 250);

            for (int i = 0; i < upgrades.Length; i++)
            {
                var upgrade = upgrades[i];

                // Bordercolor depending if selected
                Color borderColor = i == selectedIndex ? Color.Cyan : Color.White;

                // Draw background rectangle with border color
                Texture2D backgroundRect = am.TexUpgradeBackground;
                spriteBatch.Draw(backgroundRect, position, i == selectedIndex ? Color.Black*0.7f : Color.DarkSlateGray*0.7f);

                // Draw background rectangle with border color
                Texture2D rectBorders = am.TexUpgradeBorders;
                spriteBatch.Draw(rectBorders, position, borderColor);

                // Draw upgrade image on the left
                Rectangle imageRect = new Rectangle((int)position.X + 5, (int)position.Y + 5, 90, 90);
                spriteBatch.Draw(upgrade.Image, imageRect, Color.White);

                // Draw text Title on the right of the image
                Vector2 titlePosition = new Vector2(position.X + imageRect.Width + padding + 10, position.Y + padding + 15);
                spriteBatch.DrawString(am.MainFont, upgrade.Title, titlePosition, Color.White);

                // Draw text description below title
                Vector2 descriptionPosition = new Vector2(titlePosition.X, titlePosition.Y + 30); // 30 pixels below title
                spriteBatch.DrawString(am.DescriptionFont, upgrade.Description, descriptionPosition, Color.White);

                // Next upgrade position
                position.Y += backgroundRect.Height + padding /2;
            }
        }
        public void ApplyUpgrade(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < selectedPowerUps.Length)
            {
                selectedPowerUps[selectedIndex].BonusAction.Invoke();
            }
        }
        private void AddBall()
        {
            Ball ball = new Ball(am.TexWhiteCirle, paddle, Color.White, true);
            listActors.Add(ball);
            Ball.totalBallsDesired++;
            ball.bIsLaunched = true;
        }

        private void IncreaseBallSpeed()
        {
            Ball.IncreaseBaseSpeed(0.10f);
            foreach (IActor actor in listActors)
            {
                if (actor is Ball ball)
                {
                    ball.Speed = Ball.BaseSpeed;
                    ball.SetVelocityDirection();
                }
            }
        }

        private void ExtendPaddle()
        {
            paddle.ExtendWidth(0.10f);
        }

        private void IncrasePaddleSpeed()
        {
            Paddle.IncreaseBaseSpeed(0.10f);
            paddle.Speed = Paddle.BaseSpeed;
            paddle.UpdateSpeed(); // Mettez à jour la vitesse et la vélocité du paddle
        }
    }
}
