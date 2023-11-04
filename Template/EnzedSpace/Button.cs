using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnzedSpace
{
    public delegate void OnClick(Button pSender);
    public class Button : Sprite
    {
        public bool isHover {  get; private set; }
        private MouseState oldMouseState;
        public OnClick onClick {  get; set; }
        static ScreenInfo screenInfo = ServiceLocator.GetService<ScreenInfo>();
        public static float targetRatioW = screenInfo.Width / screenInfo.targetW;
        public static float targetRatioH = screenInfo.Height/ screenInfo.targetH;

        // Constructor
        public Button(Texture2D pTexture) : base(pTexture) 
        {

        }

        public override void Update(GameTime pGameTime)
        {
            targetRatioW = screenInfo.Width / screenInfo.targetW;
            targetRatioH = screenInfo.Height / screenInfo.targetH;

            MouseState newMouseState = Mouse.GetState();
            Point MousePos = new Point (
                (int)(newMouseState.Position.X / targetRatioW),
                (int)(newMouseState.Position.Y / targetRatioH));

            if (BoundingBox.Contains(MousePos))
            {
                if (!isHover)
                {
                    isHover = true;
                    Trace.WriteLine("The button is now hover");
                }
            }
            else
            {
                if (isHover)
                {
                    Trace.WriteLine("The button is no more hover");
                }
                isHover = false;
            }
            
            if (isHover)
            {
                if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    Trace.WriteLine("Button is clicked");
                    if (onClick != null)
                        onClick(this);
                }
            }

            oldMouseState = newMouseState;
            base.Update(pGameTime);
        }
    }
}
