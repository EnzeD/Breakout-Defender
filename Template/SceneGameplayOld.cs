using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    class Hero : Sprite
    {
        public int Energy;

        // Constructor
        public Hero(Texture2D pTexture, Color color) : base(pTexture, color) 
        {
            Energy = 100;
        }
        public override void TouchedBy(IActor pBy)
        {
            if (pBy is Meteor)
            {
                Energy -= 10;
            }
        }

    }
    class Meteor : Sprite
    {
        // Constructor
        public Meteor(Texture2D texture, Color color) : base(texture, color) 
        {
            do
            {
                Velocity = new Vector2((float)Util.GetInt(-3, 3) / 5, VY);
            } while (VX == 0);

            do
            {
                Velocity = new Vector2(VX, (float)Util.GetInt(-3, 3) / 5);
            } while (VY == 0);
        }
    }
    class SceneGameplayOld : Scene
    {
        private KeyboardState oldKBState;
        private Hero MyShip;
        private Song music;
        private SoundEffect sndExplode;
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();

        // Constructor
        public SceneGameplayOld(MainGame pGame) : base(pGame) 
        {

        }

        public override void Load()
        {
            oldKBState = Keyboard.GetState();
            music = AssetManager.MusicGameplay;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
            MediaPlayer.Volume = 0.2f;

            sndExplode = AssetManager.SoundExplode;       

            Rectangle Screen = mainGame.Window.ClientBounds;

            for (int i = 0; i < 20; i++)
            {
                Meteor m = new Meteor(mainGame.Content.Load<Texture2D>("meteor"), Color.White);
                m.Position = new Vector2(
                    Util.GetInt(1, Screen.Width - m.Texture.Width),
                    Util.GetInt(1, Screen.Height - m.Texture.Height)
                    );
                listActors.Add(m);
            }

            MyShip = new Hero(mainGame.Content.Load<Texture2D>("ship"), Color.White);
            MyShip.Position = new Vector2(
                (Screen.Width / 2 - MyShip.Texture.Width / 2),
                (Screen.Height / 2 - MyShip.Texture.Height / 2)
                );
            listActors.Add(MyShip);

            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
            base.UnLoad();
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle Screen = mainGame.Window.ClientBounds;

            // Meteor Management

            foreach (IActor actor in listActors)
            {
                if (actor is Meteor m)
                {
                    if (m.Position.X <= 0 || m.Position.X + m.BoundingBox.Width >= Screen.Width)
                        m.Velocity *= new Vector2(-1, 1);
                    if (m.Position.Y <= 0 || m.Position.Y + m.BoundingBox.Height >= Screen.Height)
                        m.Velocity *= new Vector2(1,-1);
                    if (Util.CollideByBox(m, MyShip))
                    {
                        MyShip.TouchedBy(m);
                        m.TouchedBy(MyShip);
                        m.ToRemove = true;
                        sndExplode.Play(0.2f,0.0f, 0.0f);
                    }
                m.Move(m.Velocity);
                }
            }

            // Clean sprites being tagged as to remove
            Clean();

            // Input Management

            KeyboardState newKBState = Keyboard.GetState();
            GamePadState newGPState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || newGPState.IsButtonDown(Buttons.B))
            {
                Trace.WriteLine("Acceleration!!!");
            }

            if(newKBState.IsKeyDown(Keys.Space) && !oldKBState.IsKeyDown(Keys.Space))
            {
                Trace.WriteLine("Espace!");
            }

            if (newKBState.IsKeyDown(Keys.Up) || newGPState.IsButtonDown(Buttons.LeftThumbstickUp))
                MyShip.Move(0f, -10f);
            if (newKBState.IsKeyDown(Keys.Left) || newGPState.IsButtonDown(Buttons.LeftThumbstickLeft))
                MyShip.Move(-10f, 0f);
            if (newKBState.IsKeyDown(Keys.Down) || newGPState.IsButtonDown(Buttons.LeftThumbstickDown))
                MyShip.Move(0f, 10f);
            if (newKBState.IsKeyDown(Keys.Right) || newGPState.IsButtonDown(Buttons.LeftThumbstickRight))
                MyShip.Move(10f, 0f);

            oldKBState = newKBState;

            // Game over

            if (MyShip.Energy <= 0)
                mainGame.gameState.ChangeScene(GameState.SceneType.Gameover);
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScreenInfo screenInfo = ServiceLocator.GetService<ScreenInfo>();
            mainGame._spriteBatch.DrawString(am.MainFont, "This is the gameplay! Energie: "+MyShip.Energy, new Vector2(1,1), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "Mouse: " + Mouse.GetState().Position.X + ";" + Mouse.GetState().Position.Y, new Vector2(1, 30), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "target w/h: " + screenInfo.targetW + "/" + screenInfo.targetH, new Vector2(1, 120), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "target width/height: " + screenInfo.Width + "/" + screenInfo.Height, new Vector2(1, 150), Color.White);
            mainGame._spriteBatch.DrawString(am.MainFont, "ratioW/ratioH: " + Button.targetRatioW + "/" + Button.targetRatioH, new Vector2(1, 180), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
