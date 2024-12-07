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
    class DemoGame : Mode13hGame
    {
        Random random = new Random(123);

        void drawRandomPixel()
        {
            pixels[random.Next(screenWidth * screenHeight)] = (byte)random.Next(byte.MaxValue);
        }

        void putPixel(int x, int y, byte color)
        {
            if (x < 0 || x >= screenWidth)
            {
                return;
            }
            if (y < 0 || y >= screenHeight)
            {
                return;
            }
            pixels[y * screenWidth + x] = color;
        }

        void horizontalLine(int x, int y, int length, byte color)
        {
            int position = y * screenWidth + x;
            for (int i = 0; i < length; i++)
            {
                pixels[position] = color;
                position++;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            drawRandomPixel();
            // putPixel(0, 0, 15); // 15 corresponds to Color white
            // horizontalLine(100, 100, 10, 32); // 32 correspond to Color blue
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

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                palette = PaletteUtil.BuildGrayscalePalette().ToArray();
            }

            base.Update(gameTime);
        }
    }
}
