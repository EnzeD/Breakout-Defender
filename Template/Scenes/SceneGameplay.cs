using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static MainSpace.GameState;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MainSpace
{  
    class SceneGameplay : Scene
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        // Controllers
        KeyboardState oldKBState;
        GamePadState oldGPState;

        // Paddle
        public static Paddle paddle = new Paddle(am.TexWhiteRectangle, Color.White, true);

        // Ball
        private Ball ball = new Ball(am.TexWhiteCirle, paddle, Color.White, true);

        // Borders
        public Border topBorder = new Border(am.TexWhiteVerticalBar, Border.BorderSide.Top, Color.White);
        public Border leftBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Left, Color.White);
        public Border rightBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Right, Color.White);
        public Border redLine = new Border(am.TexRedLine, Border.BorderSide.None, Color.White * 0.5f);

        // Bricks
        public Brick brick1 = new Brick(am.TexWhiteBrick, Color.White, true);
        public Brick brick2 = new Brick(am.TexWhiteBrick, Color.White, true);
        public List<Brick> listBricks;
        private int nbColumns = 9;
        private int nbStartingRows = 3;
        private int nbTotalRows = 72;
        private int spacing = 5;
        private float percentBricksToDisplay = 0.1f;

        // Particles
        List<Texture2D> listParticleTextures = new List<Texture2D>();
        public ParticleSystem particleSystem;

        // Collision Manager
        public CollisionManager CollisionManager;

        // GUI
        private Health health;
        private Upgrader upgrader;
        private Background background;
        private bool isPaused = false;
        private bool drawUpgrades = false;

        // Music
        private Song music = am.Music3Loop;

        public SceneGameplay(MainGame pGame) : base(pGame)
        {
            // Particles
            listParticleTextures.Add(am.TexCircleParticle);
            listParticleTextures.Add(am.TexStarParticle);
            listParticleTextures.Add(am.TexDiamondParticle);
            particleSystem = new ParticleSystem(listParticleTextures, Vector2.Zero);
            ServiceLocator.Level = 1;
        }

        public override void Load()
        {
            // Paddle Loading
            paddle.Load();
            listActors.Add(paddle);

            // Ball Loading
            ball.Load();
            listActors.Add(ball);

            // Game Borders Loading
            topBorder.LoadBorder(Border.BorderSide.Top);
            leftBorder.LoadBorder(Border.BorderSide.Left);
            rightBorder.LoadBorder(Border.BorderSide.Right);
            redLine.LoadBorder(Border.BorderSide.None);

            listActors.Add(topBorder);
            listActors.Add(leftBorder);
            listActors.Add(rightBorder);
            listActors.Add(redLine);

            // Bricks Loading
            listBricks = Brick.CreateBricks(
                listActors, am.TexWhiteBrick, 
                leftBorder.X + leftBorder.Width / 2 + am.TexWhiteBrick.Width / 2, // to start just below the borders
                topBorder.Y + topBorder.Height + am.TexWhiteBrick.Height / 2,
                spacing, // Spacing in pixels between bricks
                nbColumns, // Number of columns
                nbStartingRows, // Number of starting rows
                nbTotalRows); // Total number of rows

            Brick.RemoveRandomBricks(listActors, (int)(nbTotalRows * nbColumns * (1 -percentBricksToDisplay)));

            // Health GUI
            health = new Health(mainGame);
            ServiceLocator.InitializeHealth(health);
            upgrader = new Upgrader(mainGame, listActors);
            background = new Background(mainGame);

            // Collision Manager Loading
            CollisionManager = new CollisionManager(listActors);

            // Music
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
            MediaPlayer.Volume = 0.1f;

            // Keyboard & Gamepad old states
            oldKBState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);

            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update(GameTime gameTime)
        {
            // Controller
            bool ButLeft = false;
            bool ButRight = false;
            bool ButA = false;
            KeyboardState newKBState = Keyboard.GetState();
            GamePadState newGPState;
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            // Pause Management
            if (newKBState.IsKeyDown(Keys.U) && !oldKBState.IsKeyDown(Keys.U))
            {
                ServiceLocator.Level++;
                upgrader.ResetCloseMenuState();
                upgrader.ShowLevelUpOptions();
                drawUpgrades = true;
                isPaused = true;
            }

            // Pause Management
            if (newKBState.IsKeyDown(Keys.P) && !oldKBState.IsKeyDown(Keys.P))
            {
                if(isPaused)
                {
                    isPaused = false;
                }
                else
                {
                    isPaused = true;
                }
            }
            if (isPaused == false)
            {
                // Paddle movement
                if (newKBState.IsKeyDown(Keys.Left) || ButLeft)
                {
                    if (paddle.BoundingBox.Left >= leftBorder.Position.X + leftBorder.Width / 2)
                        paddle.Move(-paddle.Velocity);
                }
                if (newKBState.IsKeyDown(Keys.Right) || ButRight)
                {
                    if (paddle.BoundingBox.Right <= rightBorder.Position.X - rightBorder.Width / 2)
                        paddle.Move(paddle.Velocity);
                }
                if (newKBState.IsKeyDown(Keys.Enter) || ButA)
                {
                    foreach (IActor actor in listActors)
                    {
                        if (actor is Ball ball)
                        {
                            ball.bIsLaunched = true;
                        }
                    }

                }
                if (newKBState.IsKeyDown(Keys.S) && !oldKBState.IsKeyDown(Keys.S))
                {
                    Ball b = new Ball(am.TexWhiteCirle, paddle, Color.White, true);
                    //b.Position = new Vector2(paddle.X, paddle.Y - 30);
                    listActors.Add(b);
                }
                if (newKBState.IsKeyDown(Keys.D) /*&& !oldKBState.IsKeyDown(Keys.D)*/)
                {
                    Ball b = new Ball(am.TexWhiteCirle, paddle, Color.White, true);
                    b.bIsLaunched = true;
                    listActors.Add(b);
                }
                

                // Gamepad Management
                if (capabilities.IsConnected)
                {
                    newGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
                    if (newGPState.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldGPState.IsButtonDown(Buttons.LeftThumbstickLeft))
                        ButLeft = true;
                    if (newGPState.IsButtonDown(Buttons.LeftThumbstickRight) && !oldGPState.IsButtonDown(Buttons.LeftThumbstickRight))
                        ButRight = true;
                    if (newGPState.IsButtonDown(Buttons.A) && !oldGPState.IsButtonDown(Buttons.A))
                        ButA = true;

                    oldGPState = newGPState;
                }

                // Manage Collisions
                if (particleSystem != null)
                {
                    particleSystem.UpdateBricksParticle(ParticleSystem.ParticleEmitterType.Brick);
                }
                CollisionManager.Update(particleSystem);

                // Heart update
                health.UpdateHearts(gameTime);

                // Lost balls
                foreach (var ball in listActors.OfType<Ball>())
                {
                    ball.UpdateRelaunchCooldown(gameTime);
                }

                // Bricks
                Brick.UpdateBrickSpeed(gameTime);

                // Clean sprites being tagged as to remove
                Clean();

                base.Update(gameTime);
            }
            if (upgrader.ShouldCloseMenu())
            {
                isPaused = false;
                drawUpgrades = false;
            }
            
            if (drawUpgrades)
            {
                upgrader.Update(gameTime);
            }

            oldKBState = newKBState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mainGame.GraphicsDevice.Clear(Color.Black);
            /*
            
            spriteBatch.DrawString(am.MainFont, "nb actors: " + listActors.Count().ToString(), new Vector2(1, 30), Color.White);
            spriteBatch.DrawString(am.MainFont, "paddle.x :" + paddle.X, new Vector2(1, 60), Color.White);
            */
            if (!drawUpgrades)
                background.Draw(gameTime);


            // TO DO : manage XP display elsewhere
            float xpNeeded = 20;
            float xpRatio = (float)ServiceLocator.Xp / xpNeeded;
            if (xpRatio >= 1)
            {
                ServiceLocator.Xp -= (int)xpNeeded;
                ServiceLocator.Level++;
                upgrader.ResetCloseMenuState();
                upgrader.ShowLevelUpOptions();
                drawUpgrades = true;
                isPaused = true;
            }

            // Health
            health.DrawHearts();

            // Red background
            //spriteBatch.Draw(am.TexRedFlash, new Vector2(si.targetW / 3 + 20, 65), Color.White * 0.25f);

            // Test abstract lines at the bottom
            //spriteBatch.Draw(am.TextAbstractLines, new Vector2(si.targetW / 3 + 20, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 95), Color.White * 0.5f);


            //spriteBatch.DrawString(am.MainFont, "nb Xp on screen: " + listActors.OfType<Xp>().Count().ToString(), new Vector2(1, 1), Color.White);
            spriteBatch.DrawString(am.MainFont, "Level " + ServiceLocator.Level, new Vector2(200, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 70), Color.White);
            spriteBatch.DrawString(am.MainFont, "Xp : " + ServiceLocator.Xp + " / " + (int)xpNeeded, new Vector2(200, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - 40), Color.White);
            if (particleSystem != null)
                particleSystem.Draw(spriteBatch);

            Rectangle xpBarBorder = new Rectangle(200, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - am.TexXpBarBorder.Height / 2, am.TexXpBarBorder.Width, am.TexXpBarBorder.Height);
            spriteBatch.Draw(am.TexXpBarBorder, xpBarBorder, Color.White);

           
            Rectangle xpBar = new Rectangle(204, si.targetH - ServiceLocator.DIST_FROM_BOTTOM_SCREEN - am.TexXpBarBorder.Height / 2, (int)(am.TexXpBarBorder.Width * xpRatio), am.TexXpBarGreen.Height);
            spriteBatch.Draw(am.TexXpBarGreen, xpBar, Color.White);

            base.Draw(gameTime, spriteBatch);

            if (drawUpgrades)
            {
                upgrader.Draw(gameTime);
            }
        }
      
    }
}
