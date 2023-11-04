using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace EnzedSpace

{
    public class MainGame : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public GameState gameState;
        public RenderTarget2D render;
        public int TargetWidth = 1280;
        public int TargetHeight = 720;
        KeyboardState oldKBState;
        bool bSampling = false; // Not pixelated when we scale up
        // TO DO: manage margin for mousepos if not a 16:9 aspect ratio
        public static bool isFullScreen;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            // Services Init
            ScreenInfo screenInfo = new ScreenInfo(_graphics);
            ServiceLocator.RegisterService<ScreenInfo>(screenInfo);

            AssetManager assetManager = new AssetManager(Content);
            ServiceLocator.RegisterService<AssetManager>(assetManager);

            Spritebatch spriteBatch = new Spritebatch(_spriteBatch);
            ServiceLocator.RegisterService<Spritebatch>(spriteBatch);

            _graphics.PreferredBackBufferWidth = TargetWidth;
            _graphics.PreferredBackBufferHeight = TargetHeight;
            _graphics.IsFullScreen = false;
            isFullScreen = _graphics.IsFullScreen;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameState = new GameState(this);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Init Render Target
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            render = new RenderTarget2D(_graphics.GraphicsDevice, TargetWidth, TargetHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            // Initial scene
            gameState.ChangeScene(GameState.SceneType.Menu);

            oldKBState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            AssetManager.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (gameState.CurrentScene != null)
            {
                gameState.CurrentScene.Update(gameTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) && !oldKBState.IsKeyDown(Keys.S))
            {
                bSampling = !bSampling;
            }

            // Full Screen
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !oldKBState.IsKeyDown(Keys.F))
            {
                if (!_graphics.IsFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = TargetWidth;
                    _graphics.PreferredBackBufferHeight = TargetHeight;
                }
                isFullScreen = _graphics.IsFullScreen;

                _graphics.ToggleFullScreen();
                _graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ScreenInfo screenInfo = ServiceLocator.GetService<ScreenInfo>();    
            GraphicsDevice.SetRenderTarget(render);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // TODO: Add your drawing code here
            if (gameState.CurrentScene != null)
            {
                gameState.CurrentScene.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            // Drawing the render target, adding marging if aspect ratio different from target and scaling the render

            float ratio = 1;
            int marginV = 0;
            int marginH = 0;
            float currentAspect = screenInfo.Width / (float)screenInfo.Height;
            float virtualAspect = (float)TargetWidth / (float)TargetHeight;
            if (TargetHeight != screenInfo.Height)
            {
                if (currentAspect > virtualAspect)
                {
                    ratio = screenInfo.Height / (float)TargetHeight;
                    marginH = (int)((screenInfo.Width - TargetWidth * ratio) / 2);
                }
                else
                {
                    ratio = screenInfo.Width / (float)TargetWidth;
                    marginV = (int)((screenInfo.Height - TargetHeight * ratio) / 2);
                }
            }
            Rectangle dst = new Rectangle(marginH, marginV, (int)(TargetWidth * ratio), (int)(TargetHeight * ratio));


            if (!bSampling)
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            else
                _spriteBatch.Begin();

            _spriteBatch.Draw(render, dst, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);


        }
    }
}