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

namespace MainSpace
{
    class SceneMenu : Scene
    {
        KeyboardState oldKBState;
        GamePadState oldGPState;
        //private Button MyButton;
        static ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();
        static AssetManager am = ServiceLocator.GetService<AssetManager>();
        // Borders
        public Border topBorder = new Border(am.TexWhiteVerticalBar, Border.BorderSide.Top, Color.White);
        public Border leftBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Left, Color.White);
        public Border rightBorder = new Border(am.TexWhiteLateralBar, Border.BorderSide.Right, Color.White);
        public SceneMenu(MainGame pGame) : base(pGame) 
        {

        }

        public void onClickPlay(Button pSender)
        {
            mainGame.gameState.ChangeScene(GameState.SceneType.Gameplay);
        }

        public override void Load()
        {
            am.LoadMusics();
            am.PlayRandomMusic();

            // Borders
            topBorder.LoadBorder(Border.BorderSide.Top);
            leftBorder.LoadBorder(Border.BorderSide.Left);
            rightBorder.LoadBorder(Border.BorderSide.Right);

            listActors.Add(topBorder);
            listActors.Add(leftBorder);
            listActors.Add(rightBorder);

            Rectangle Screen = mainGame.Window.ClientBounds;

            oldKBState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            /*
            MyButton = new Button(mainGame.Content.Load<Texture2D>("textures/button"), Color.White);
            MyButton.Position = new Vector2(
                Screen.Width / 2 - MyButton.Texture.Width/2, 
                Screen.Height / 2 - MyButton.Texture.Height/2);

            MyButton.onClick = onClickPlay;
            
            listActors.Add(MyButton);
            */
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

            // Mouse
            MouseState newMState = Mouse.GetState();
            if (newMState.LeftButton == ButtonState.Pressed)
            {

            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(am.TexLogo, new Vector2(si.targetW / 2 - am.TexLogo.Width / 2, 65), Color.White);
            string message = "Press SPACE to start!";
            Vector2 textSize = am.MainFont.MeasureString(message);
            spriteBatch.DrawString(am.MainFont, message, new Vector2(si.targetW / 2 - textSize.X / 2, 600), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
