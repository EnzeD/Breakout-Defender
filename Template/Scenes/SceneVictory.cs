using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MainSpace
{
    class SceneVictory : Scene
    {
        KeyboardState oldKBState;
        GamePadState oldGPState;

        static ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        static AssetManager am = ServiceLocator.GetService<AssetManager>();

        // Borders
        public Border topBorder = new Border(am.TexWhiteVerticalBar, Border.BorderSide.Top, Color.White);
        public Border leftBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Left, Color.White);
        public Border rightBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Right, Color.White);
        public SceneVictory(MainGame pGame) : base(pGame)
        {

        }

        public override void Load()
        {
            am.PlayRandomMusic();

            // Borders
            topBorder.LoadBorder(Border.BorderSide.Top);
            leftBorder.LoadBorder(Border.BorderSide.Left);
            rightBorder.LoadBorder(Border.BorderSide.Right);

            listActors.Add(topBorder);
            listActors.Add(leftBorder);
            listActors.Add(rightBorder);

            oldKBState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);

            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
            base.UnLoad();
        }

        public override void Update(GameTime gameTime)
        {
            bool ButA = false;

            // Keyboard 
            KeyboardState newKBState = Keyboard.GetState();
            if ((newKBState.IsKeyDown(Keys.Space) && !oldKBState.IsKeyDown(Keys.Space)) || ButA)
            {
                mainGame.gameState.ChangeScene(GameState.SceneType.Gameplay);
            }
            oldKBState = newKBState;

            // Gamepad
            GamePadState newGPState;
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (capabilities.IsConnected)
            {
                newGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
                if (newGPState.IsButtonDown(Buttons.A) && !oldGPState.IsButtonDown(Buttons.A))
                {
                    ButA = true;
                }
                oldGPState = newGPState;
            }


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(am.TexGameOver, new Vector2(si.targetW / 2 - am.TexGameOver.Width / 2, 65), Color.White);
            string message = "Press SPACE to try again!";
            Vector2 textSize = am.MainFont.MeasureString(message);
            spriteBatch.DrawString(am.MainFont, message, new Vector2(si.targetW / 2 - textSize.X / 2, 600), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
