using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mode13h.Logic;
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
    class Lines : Mode13hGame
    {
        Random random = new Random(123);

        void putPixel(int x, int y, byte color)
        {
            pixels[y * screenWidth + x] = color;
        }

        void drawLineBresenham(int x1, int y1, int x2, int y2, byte color)
        {
            int x, y, xe, ye;
            int deltaX = x2 - x1;
            int deltaY = y2 - y1;
            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);
            int px = 2 * absDeltaY - absDeltaX;
            int py = 2 * absDeltaX - absDeltaY;
            if (absDeltaY <= absDeltaX)
            {
                if (deltaX >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    xe = x1;
                }
                putPixel(x, y, color);

                for (int i = 0; x < xe; i++)
                {
                    x = x + 1;
                    if (px < 0)
                    {
                        px = px + 2 * absDeltaY;
                    }
                    else
                    {
                        if ((deltaX < 0 && deltaY < 0) || (deltaX > 0 && deltaY > 0))
                        {
                            y = y + 1;
                        }
                        else
                        {
                            y = y - 1;
                        }
                        px = px + 2 * (absDeltaY - absDeltaX);
                    }

                    putPixel(x, y, color);
                }
            }
            else
            {
                if (deltaY >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    ye = y1;
                }
                putPixel(x, y, color);
                for (int i = 0; y < ye; i++)
                {
                    y = y + 1;
                    if (py <= 0)
                    {
                        py = py + 2 * absDeltaX;
                    }
                    else
                    {
                        if ((deltaX < 0 && deltaY < 0) || (deltaX > 0 && deltaY > 0))
                        {
                            x = x + 1;
                        }
                        else
                        {
                            x = x - 1;
                        }
                        py = py + 2 * (absDeltaX - absDeltaY);
                    }

                    putPixel(x, y, color);
                }
            }
        }

        void drawLine(int x1, int y1, int x2, int y2, byte color)
        {
            int deltaX = x2 - x1;
            int deltaY = y2 - y1;
            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);
            if (absDeltaX >= absDeltaY)
            {
                int x = x1;
                int xIncrement = Math.Sign(deltaX);
                int y = y1 * 65536;
                deltaY = deltaY * 65536 / absDeltaX;
                for (int i = 0; i < absDeltaX; i++)
                {
                    putPixel(x, (y + 32768) >> 16, color);
                    x += xIncrement;
                    y += deltaY;
                }
            }
            else
            {
                int x = x1 * 65536;
                deltaX = deltaX * 65536 / absDeltaY;
                int y = y1;
                int yIncrement = Math.Sign(deltaY);
                for (int i = 0; i < absDeltaY; i++)
                {
                    putPixel((x + 32768) >> 16, y, color);
                    x += deltaX;
                    y += yIncrement;
                }
            }
        }


        void drawPolygon(int centerX, int centerY, int numberOfSides, int radius, int roughness, byte color, double angle = 0)
        {
            double angleIncrement = Math.PI * 2 / numberOfSides;
            double firstX;
            double firstY;
            int distance = getRandomDistance(radius, roughness);
            getRotation(centerX, centerY, angle, distance, out firstX, out firstY);
            for (int i = 0; i < numberOfSides; i++)
            {
                angle += angleIncrement;
                distance = getRandomDistance(radius, roughness);
                double secondX;
                double secondY;
                getRotation(centerX, centerY, angle, distance, out secondX, out secondY);
                drawLine((int)(firstX + 0.5), (int)(firstY + 0.5), (int)(secondX + 0.5), (int)(secondY + 0.5), color);
                firstX = secondX;
                firstY = secondY;
            }
        }

        private int getRandomDistance(int distance, int roughness)
        {
            int halfRoughness = roughness / 2;
            return distance - halfRoughness + random.Next(roughness);
        }

        private static void getRotation(int centerX, int centerY, double angle, int distance, out double x, out double y)
        {
            x = Math.Cos(angle) * distance + centerX;
            y = Math.Sin(angle) * distance + centerY;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            drawLineBresenham(30, 30, 60, 20, 15);
            drawLineBresenham(60, 20, 80, 70, 15);
            drawLineBresenham(80, 70, 30, 30, 15);


            drawPolygon(100, 100, 6, 30, 0, 32);
            drawPolygon(150, 150, 3, 20, 8, 32);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            for (int i = 0; i < 3; i++)
            {
                int x1 = random.Next(320);
                int y1 = random.Next(200);
                int x2 = random.Next(320);
                int y2 = random.Next(200);
                if (x1 == x2 && y1 == y2)
                {
                    continue;
                }
                drawLine(x1, y1, x2, y2, (byte)random.Next(256));
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            base.Update(gameTime);
        }
    }
}
