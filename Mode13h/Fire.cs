using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mode13h.Logic;
using Mode13h.Logic.Util;
using System;

namespace Mode13h
{
    /// <summary>
    /// Demo class which extends Mode13hGame
    /// Inside the class one has access to:
    /// int screenWidth, screenHeight - the size of the screen
    /// byte[] pixels - the 8bpp indexed color channel which will be displayed on the screen
    /// PaletteEntry[] palette - the palette which is used when converting the indexed values into 24bpp colors
    /// </summary>
    class Fire : Mode13hGame
    {
        Random random = new Random(123);

        const int bufferSizeX = 320;
        const int bufferSizeY = 200 + 2;

        byte[,] buffer = new byte[bufferSizeY, bufferSizeX];

        void putPixel(int x, int y, byte color, byte[,] destination)
        {
            destination[y, x] = color;
        }

        byte getPixel(int x, int y, byte[,] source)
        {
            return source[y, x];
        }

        void setPalette()
        {
            for (int i = 0; i <= 63; i++)
            {
                palette[i] = new PaletteEntry((byte)(i * 4), 0, 0);
            }
            for (int i = 64; i <= 127; i++)
            {
                palette[i] = new PaletteEntry(255, (byte)((i - 64) * 4), 0);
            }
            for (int i = 128; i <= 255; i++)
            {
                palette[i] = new PaletteEntry(255, 255, (byte)((i - 128) * 2));
            }
        }

        void drawBottomLine(int numberOfChanges = 100)
        {
            for (int i = 0; i < numberOfChanges; i++)
            {
                int j = random.Next(1, bufferSizeX - 1);
                int probability = random.Next(100);
                if (probability >= 0 && probability <= 14)
                {
                    putPixel(j, bufferSizeY - 1, 255, buffer);
                    break;
                }
                if (probability >= 15 && probability <= 30)
                {
                    putPixel(j, bufferSizeY - 1, 0, buffer);
                    break;
                }
                if (probability >= 31 && probability <= 65)
                {
                    putPixel(j, bufferSizeY - 1, getPixel(j - 1, bufferSizeY - 1, buffer), buffer);
                    break;
                }
                if (probability >= 66)
                {
                    putPixel(j, bufferSizeY - 1, getPixel(j + 1, bufferSizeY - 1, buffer), buffer);
                    break;
                }
            }
        }

        void updateFire()
        {
            for (int i = bufferSizeY / 3; i < bufferSizeY - 1; i++)
            {
                for (int j = 1; j < bufferSizeX - 1; j++)
                {
                    byte color = (byte)((getPixel(j - 1, i + 1, buffer) + getPixel(j, i + 1, buffer)
                               + getPixel(j + 1, i + 1, buffer) + getPixel(j, i, buffer)
                               + getPixel(j + 1, i, buffer) + getPixel(j - 1, i, buffer) + /*3*/ 1) / 6);
                    if ((color > 0) && random.Next(2) == 0)
                    {
                        color--;
                    }
                    putPixel(j, i, color, buffer);
                }
            }
        }

        void copyBufferToScreen()
        {
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    this.pixels[y * screenWidth + x] = buffer[y, x];
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            setPalette();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            copyBufferToScreen();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                palette = PaletteUtil.BuildGrayscalePalette().ToArray();
            }

            drawBottomLine();
            int milliseconds = gameTime.ElapsedGameTime.Milliseconds;
            for (int i = 0; i < milliseconds / 10 + 1; i++)
            {   
                updateFire();
            }

            base.Update(gameTime);
        }
    }
}
