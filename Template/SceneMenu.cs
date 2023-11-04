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

namespace EnzedSpace
{
    class SceneMenu : Scene
    {
        KeyboardState oldKBState;
        GamePadState oldGPState;
        private Button MyButton;
        private Song music;
        static ScreenInfo screenInfo = ServiceLocator.GetService<ScreenInfo>();
        static AssetManager am = ServiceLocator.GetService<AssetManager>();
        public SceneMenu(MainGame pGame) : base(pGame) 
        {

        }

        public void onClickPlay(Button pSender)
        {
            mainGame.gameState.ChangeScene(GameState.SceneType.Gameplay);
        }

        public override void Load()
        {
            music = am.MusicMenu;
            MediaPlayer.IsRepeating = true;
            // MediaPlayer.Play(music); // TODO add music menu
            MediaPlayer.Volume = 0.2f;

            Rectangle Screen = mainGame.Window.ClientBounds;

            oldKBState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);

            MyButton = new Button(mainGame.Content.Load<Texture2D>("button"));
            MyButton.Position = new Vector2(
                Screen.Width / 2 - MyButton.Texture.Width/2, 
                Screen.Height / 2 - MyButton.Texture.Height/2);

            MyButton.onClick = onClickPlay;

            listActors.Add(MyButton);

            base.Load();
        }

        public override void UnLoad()
        {
            Trace.WriteLine("New SceneMenu.UnLoad");
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
                mainGame.gameState.ChangeScene(GameState.SceneType.Gameplay2);
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
            spriteBatch.DrawString(am.MainFont, "Press 'Space' to start", new Vector2(1, 1), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
