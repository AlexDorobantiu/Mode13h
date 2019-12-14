using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mode13h.Logic.Util;
using System;

namespace Mode13h.Logic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Mode13hGame : Game
    {
        private const int defaultScreenWidth = 320;
        private const int defaultScreenHeight = 200;
        private const int defaultScreenZoom = 4;
        private const bool defaultFullScreen = false;
        private const byte opaqueAlpha = 255;
        public readonly int screenWidth;
        public readonly int screenHeight;
        private readonly int screenZoom;
        private readonly bool fullScreen;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D canvas;
        private uint[] convertedPixelValues;

        public byte[] pixels;
        public PaletteEntry[] palette;

        public Mode13hGame(int screenWidth = defaultScreenWidth, int screenHeight = defaultScreenHeight, int screenZoom = defaultScreenZoom, bool fullScreen = defaultFullScreen)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.screenZoom = screenZoom;
            this.fullScreen = fullScreen;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = screenWidth * screenZoom;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = screenHeight * screenZoom;   // set this value to the desired height of your window
            graphics.IsFullScreen = fullScreen;
            graphics.ApplyChanges();

            Rectangle deviceBounds = GraphicsDevice.PresentationParameters.Bounds;
            if (deviceBounds.Width != screenWidth * screenZoom || deviceBounds.Height != screenHeight * screenZoom)
            {
                throw new Exception("Screen was not properly initialized!");
            }

            canvas = new Texture2D(GraphicsDevice, screenWidth, screenHeight, false, SurfaceFormat.Color);
            convertedPixelValues = new uint[screenWidth * screenHeight];

            pixels = new byte[screenWidth * screenHeight];
            palette = PaletteUtil.LevelUpPalette(PaletteUtil.BuildDefaultVga256Palette()).ToArray();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black);

            for (int position = 0; position < screenWidth * screenHeight; position++)
            {
                byte paletteIndex = pixels[position];
                PaletteEntry paletteEntry = palette[paletteIndex];
                uint convertedValue = (uint)((opaqueAlpha << 24) + (paletteEntry.blue << 16) + (paletteEntry.green << 8) + paletteEntry.red);
                convertedPixelValues[position] = convertedValue;
            }
            canvas.SetData(convertedPixelValues, 0, screenWidth * screenHeight);

            spriteBatch.Begin();
            spriteBatch.Draw(canvas, new Rectangle(0, 0, screenWidth * screenZoom, screenHeight * screenZoom), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
