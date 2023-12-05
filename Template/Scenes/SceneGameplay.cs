using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using static MainSpace.GameState;

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
        public List<Brick> listBricks;
        private int nbColumns = 9;
        private int spacing = 5;
        private int stageStartX;
        private int stageStartY;

        // Stages
        private Queue<Stage> stages;
        private Stage currentStage;
        private string stageMessage;
        private bool stageMessageToDiplay = false;
        private const int StageMessageDisplayTime = 2000;
        private float stageMessageAlpha = 1.0f;
        private int stageMessageTimer = StageMessageDisplayTime;

        // Particles
        List<Texture2D> listParticleTextures = new List<Texture2D>();
        public ParticleSystem particleSystem;

        // Collision Manager
        public CollisionManager CollisionManager;

        // GUI
        private Health health;
        private Upgrader upgrader;
        private Background background;
        private Stats stats;
        private BallsDisplay ballsDisplay;
        private bool isPaused = false;
        private bool drawUpgrades = false;
        private Color backgroundColor;
        float xpNeeded;

        public SceneGameplay(MainGame pGame) : base(pGame)
        {
            // Particles
            listParticleTextures.Add(am.TexCircleParticle);
            listParticleTextures.Add(am.TexStarParticle);
            listParticleTextures.Add(am.TexDiamondParticle);
            particleSystem = new ParticleSystem(listParticleTextures, Vector2.Zero);
            ServiceLocator.Level = 1;
            ServiceLocator.Xp = 0;
            Ball.totalBallsDesired = 1;
            Brick.brickSpeed = 0.3f;
            Paddle.BaseSpeed = 5;
            Ball.BaseSpeed = 5;
            xpNeeded = 20;

            stages = new Queue<Stage>();

            // Stages
            stages.Enqueue(new Stage(1, 0, 10, 0.12f, 0.3f, Color.Cyan));
            stages.Enqueue(new Stage(2, 0, 20, 0.15f, 0.32f, Color.DarkViolet));
            stages.Enqueue(new Stage(3, 0, 30, 0.20f, 0.34f, Color.Tomato));
            stages.Enqueue(new Stage(4, 0, 40, 0.25f, 0.36f, Color.LimeGreen));
            stages.Enqueue(new Stage(5, 0, 50, 0.30f, 0.38f, Color.Black));
            stages.Enqueue(new Stage(6, 0, 60, 0.35f, 0.45f, Color.Cyan));
            stages.Enqueue(new Stage(7, 0, 70, 0.40f, 0.50f, Color.DarkViolet));
            stages.Enqueue(new Stage(8, 0, 80, 0.45f, 0.55f, Color.Tomato));
            stages.Enqueue(new Stage(9, 0, 90, 0.50f, 0.60f, Color.LimeGreen));
            stages.Enqueue(new Stage(10, 0, 200, 0.55f, 0.80f, Color.Black));
            // More to add
        }

        public override void Load()
        {
            // Paddle Loading
            paddle.Load();
            listActors.Add(paddle);
            paddle.ResetWidth();

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
            stageStartX = leftBorder.X + leftBorder.Width / 2 + am.TexWhiteBrick.Width / 2;  // to start just below the borders
            stageStartY = topBorder.Y + topBorder.Height - am.TexWhiteBrick.Height / 2;


            // Health GUI
            health = new Health(mainGame);
            ServiceLocator.InitializeHealth(health);
            upgrader = new Upgrader(mainGame, listActors);
            background = new Background(mainGame);
            stats = new Stats(mainGame, listActors);
            ballsDisplay = new BallsDisplay(mainGame, listActors);

            // Collision Manager Loading
            CollisionManager = new CollisionManager(listActors);

            // Keyboard & Gamepad old states
            oldKBState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            
            // Load first stage
            LoadNextStage(); 

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
                ballsDisplay.Update(gameTime);

                // Bricks
                //Brick.UpdateBrickSpeed(gameTime);

                // Clean sprites being tagged as to remove
                Clean();

                // Next Stage
                if (listActors.OfType<Brick>().All(brick => brick.ToRemove))
                {
                    LoadNextStage();
                }

                if (stageMessageToDiplay)
                {
                    stageMessageTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (stageMessageTimer <= 0)
                    {
                        stageMessageToDiplay = false;
                        stageMessageTimer = 0;
                    }
                    else
                    {
                        stageMessageAlpha = stageMessageTimer / (float)StageMessageDisplayTime;
                    }
                }

                // Game over Scene
                if (ServiceLocator.Health <= 0)
                {
                    mainGame.gameState.ChangeScene(GameState.SceneType.Gameover);
                }

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
        private void LoadNextStage()
        {
            if (stages.Any())
            {
                currentStage = stages.Dequeue();
                GenerateBrickList(currentStage); 
                am.PlayRandomMusic(); // change the beat ;)
                stageMessage = "STAGE " + currentStage.Number;
                stageMessageAlpha = 1.0f;
                stageMessageTimer = StageMessageDisplayTime;
                stageMessageToDiplay = true;
            }
            else
            {   
                // If no more stages: VICTORY!!
                mainGame.gameState.ChangeScene(SceneType.Victory);
            }
        }

        private void GenerateBrickList(Stage stage)
        {
            listBricks = Brick.CreateBricks(
            listActors, am.TexWhiteBrick,
            stageStartX,
            stageStartY,
            spacing, // Spacing in pixels between bricks
            nbColumns,
            stage.NbStartingRows,
            stage.NbTotalRows,
            stage.BrickSpeed
            );
            backgroundColor = stage.Color;
            Brick.RemoveRandomBricks(listActors, (int)(stage.NbTotalRows * nbColumns * (1 - stage.PercentBricksToDisplay)));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!drawUpgrades)
                background.Draw(gameTime, backgroundColor);

            if (stageMessageToDiplay)
            {
                Vector2 textSize = am.TitleFont.MeasureString(stageMessage);
                spriteBatch.DrawString(
                    am.TitleFont,
                    stageMessage,
                    new Vector2(si.targetW / 2 - textSize.X / 2, si.targetH / 2 - textSize.Y / 2),
                    Color.White * stageMessageAlpha
                );
            }

            // TO DO : manage XP display elsewhere

            float xpRatio = (float)ServiceLocator.Xp / xpNeeded;
            if (xpRatio >= 1)
            {
                ServiceLocator.Xp -= (int)xpNeeded;
                ServiceLocator.Level++;
                xpNeeded += ServiceLocator.Level * 2;
                upgrader.ResetCloseMenuState();
                upgrader.ShowLevelUpOptions();
                drawUpgrades = true;
                isPaused = true;
            }

            // GUI
            health.DrawHearts();
            stats.Draw();
            ballsDisplay.Draw(gameTime);

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
