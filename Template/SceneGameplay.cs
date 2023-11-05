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

        // Bricks
        public Brick brick1 = new Brick(am.TexWhiteBrick, Color.White, true);
        public Brick brick2 = new Brick(am.TexWhiteBrick, Color.White, true);
        public List<Brick> listBricks;

        // Particles
        List<Texture2D> listParticleTextures = new List<Texture2D>();
        public ParticleSystem particleSystem;

        // Collision Manager
        public CollisionManager CollisionManager;

        public SceneGameplay(MainGame pGame) : base(pGame)
        {
            // Particles
            listParticleTextures.Add(am.TexCircleParticle);
            listParticleTextures.Add(am.TexStarParticle);
            listParticleTextures.Add(am.TexDiamondParticle);
            particleSystem = new ParticleSystem(listParticleTextures, Vector2.Zero);
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
            listActors.Add(topBorder);
            listActors.Add(leftBorder);
            listActors.Add(rightBorder);

            // Bricks Loading
            listBricks = Brick.CreateBricks(
                listActors, am.TexWhiteBrick, 
                leftBorder.X + leftBorder.Width + am.TexWhiteBrick.Width / 2, // to start below the borders
                topBorder.Y + topBorder.Height + am.TexWhiteBrick.Height / 2, 5, 
                9, // Number of columns
                9, // Number of starting rows
                18); // Total number of rows

            //Brick.RemoveRandomBricks(listActors,0);

            // Collision Manager Loading
            CollisionManager = new CollisionManager(listActors);

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

            // Paddle movement
            if (newKBState.IsKeyDown(Keys.Left) || ButLeft)
            {
                if (paddle.BoundingBox.Left >= leftBorder.Position.X + leftBorder.Width)
                    paddle.Move(- paddle.Velocity);
            }
            if (newKBState.IsKeyDown(Keys.Right) || ButRight)
            {
                if (paddle.BoundingBox.Right <= rightBorder.Position.X)
                    paddle.Move(paddle.Velocity);
            }
            if (newKBState.IsKeyDown(Keys.Enter) || ButA)
            {
                foreach(IActor actor in listActors)
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
                Trace.WriteLine("new ball generated");
                listActors.Add(b);
            }
            if (newKBState.IsKeyDown(Keys.D) /*&& !oldKBState.IsKeyDown(Keys.D)*/)
            {
                Ball b = new Ball(am.TexWhiteCirle, paddle, Color.White, true);
                b.bIsLaunched = true;
                Trace.WriteLine("new ball generated");
                listActors.Add(b);
            }
            oldKBState = newKBState;

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
                particleSystem.Update(ParticleSystem.ParticleEmitterType.Brick);
            }
            CollisionManager.Update(particleSystem);


            // Clean sprites being tagged as to remove
            Clean();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            spriteBatch.DrawString(am.MainFont, "nb balls: " + listActors.OfType<Ball>().Count().ToString(), new Vector2(1, 1), Color.White);
            spriteBatch.DrawString(am.MainFont, "nb actors: " + listActors.Count().ToString(), new Vector2(1, 30), Color.White);
            spriteBatch.DrawString(am.MainFont, "paddle.x :" + paddle.X, new Vector2(1, 60), Color.White);
            */
            if (particleSystem != null)
                particleSystem.Draw(spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
