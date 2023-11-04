using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static MainSpace.GameState;

namespace MainSpace
{  
    class SceneGameplay : Scene
    {
        public SceneGameplay(MainGame pGame) : base(pGame) { }

        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        // Controllers
        KeyboardState oldKBState;
        GamePadState oldGPState;

        // Paddle
        public static Paddle paddle = new Paddle(am.TexWhiteRectangle, true);

        // Ball
        private Ball ball = new Ball(am.TexWhiteCirle, paddle, true);

        // Borders
        public Border topBorder = new Border(am.TexWhiteVerticalBar, Border.BorderSide.Top);
        public Border leftBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Left);
        public Border rightBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Right);

        // Collision Manager
        public CollisionManager CollisionManager;

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
                Ball b = new Ball(am.TexWhiteCirle, paddle, true);
                //b.Position = new Vector2(paddle.X, paddle.Y - 30);
                Trace.WriteLine("new ball generated");
                listActors.Add(b);
            }
            if (newKBState.IsKeyDown(Keys.D) /*&& !oldKBState.IsKeyDown(Keys.D)*/)
            {
                Ball b = new Ball(am.TexWhiteCirle, paddle, true);
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
            CollisionManager.Update();

            // Clean sprites being tagged as to remove
            Clean();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(am.MainFont, "nb balls: " + listActors.OfType<Ball>().Count().ToString(), new Vector2(1, 1), Color.White);
            spriteBatch.DrawString(am.MainFont, "nb actors: " + listActors.Count().ToString(), new Vector2(1, 30), Color.White);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
