using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mode13h.Logic.Util
{
    /// <summary>
    /// Helper functions for palette entry data
    /// Most of the code here is adapted from https://github.com/canidlogic/vgapal
    /// </summary>
    public class PaletteUtil
    {
        /// <summary>
        /// Adds a color entry to the palette.
        /// All palette entries must use 64-level values.
        /// </summary>
        /// <param name="paletteEntries">The 64-level pallete</param>
        /// <param name="r">the red channel value</param>
        /// <param name="g">the green channel value</param>
        /// <param name="b">the blue channel value</param>
        private static void add64LevelColor(List<PaletteEntry> paletteEntries, byte r, byte g, byte b)
        {
            /* Check ranges */
            if ((r < 0) || (r > 63) ||
                (g < 0) || (g > 63) ||
                (b < 0) || (b > 63))
            {
                throw new ArgumentException();
            }

            /* Check that palette isn't full */
            if (paletteEntries.Count >= 256)
            {
                throw new ArgumentException();
            }

            paletteEntries.Add(new PaletteEntry(r, g, b));
        }

        /*
         * Add a grayscale color to the palette.
         *
         * This calls through to addColor with all channel values equal to
         * the provided v.
         *
         * Parameters:
         *
         *   v - the grayscale value
         */
        private static void add64LevelGrayColor(List<PaletteEntry> paletteEntries, byte v)
        {
            add64LevelColor(paletteEntries, v, v, v);
        }

        /// <summary>
        /// Add the 16 color entries of the standard 16-color palette.
        /// </summary>
        /// <param name="paletteEntries">The 64-level pallete</param>
        /// <param name="lo">the low intensity value to use</param>
        /// <param name="melo">the medium-low intensity value to use</param>
        /// <param name="mehi">the medium-high intensity value to use</param>
        /// <param name="hi">the high intensity value to use</param>
        private static void addTheStandard16Colors(List<PaletteEntry> paletteEntries, int lo, int melo, int mehi, int hi)
        {

            int r = 0;
            int g = 0;
            int b = 0;
            int i = 0;
            int h = 0;
            int l = 0;

            /* The passed values must be in 64-level range */
            if ((lo < 0) || (lo > 63) ||
                (melo < 0) || (melo > 63) ||
                (mehi < 0) || (mehi > 63) ||
                (hi < 0) || (hi > 63))
            {
                throw new ArgumentException();
            }

            /* Generate each of the 16 values */
            for (i = 0; i < 16; i++)
            {
                /* Determine whether low-intensity series or high-intensity */
                if ((i & 8) == 8)
                {
                    /* Intensity bit set -- define intense range */
                    h = hi;
                    l = melo;

                }
                else
                {
                    /* Intensity bit clear -- define low-intensity range */
                    h = mehi;
                    l = lo;
                }

                /* Start by setting each channel to low value */
                r = l;
                g = l;
                b = l;

                /* Activate channels selected by the appropriate bit */
                if ((i & 4) == 4)
                {
                    r = h;
                }
                if ((i & 2) == 2)
                {
                    g = h;
                }
                if ((i & 1) == 1)
                {
                    b = h;
                }

                /* As an exceptional case, for index 6 only, change the green
                   component to medium-low intensity so that we get brown instead
                   of dark yellow */
                if (i == 6)
                {
                    g = melo;
                }

                /* Add this color */
                add64LevelColor(paletteEntries, (byte)r, (byte)g, (byte)b);
            }
        }

        /// <summary>
        /// Adds four colors to the palette corresponding to a "run" within an RGB cycle.
        /// start, ch, and the return value are encoded such that only the three least significant bits are used.  The most significant of these
        /// corresponds to red, the next to green, and the least significant to blue.
        /// Intensity values must be 64-level.
        /// </summary>
        /// <param name="paletteEntries">The 64-level pallete</param>
        /// <param name="start">high and low starting states for each channel at the start of the run</param>
        /// <param name="ch">the channel to change from high to low or low to high (only one bit may be set</param>
        /// <param name="lo">the low intensity value</param>
        /// <param name="melo">the medium-low intensity value</param>
        /// <param name="me">the medium intensity value</param>
        /// <param name="mehi">the medium-high intensity value</param>
        /// <param name="hi">the high intensity value</param>
        /// <returns>the high and low starting states for each channel at the start of the run after this one</returns>
        private static int add64Level4ColorsRun(List<PaletteEntry> paletteEntries, int start, int ch, int lo, int melo, int me, int mehi, int hi)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int i = 0;
            bool up = false;
            int v = 0;

            /* Check parameters */
            if ((start < 0) || (start > 7))
            {
                throw new ArgumentException();
            }
            if ((ch != 1) && (ch != 2) && (ch != 4))
            {
                throw new ArgumentException();
            }
            if ((lo < 0) || (lo > 63) ||
                (melo < 0) || (melo > 63) ||
                (me < 0) || (me > 63) ||
                (mehi < 0) || (mehi > 63) ||
                (hi < 0) || (hi > 63))
            {
                throw new ArgumentException();
            }

            /* Get the starting RGB color and add it */
            r = lo;
            g = lo;
            b = lo;

            if ((start & 4) == 4)
            {
                r = hi;
            }
            if ((start & 2) == 2)
            {
                g = hi;
            }
            if ((start & 1) == 1)
            {
                b = hi;
            }

            add64LevelColor(paletteEntries, (byte)r, (byte)g, (byte)b);

            /* If selected channel starts high, we're going down; else, we're
               going up */
            if ((start & ch) == ch)
            {
                up = false;
            }
            else
            {
                up = true;
            }

            /* Add remaining three colors of the run */
            for (i = 0; i < 3; i++)
            {
                /* Value depends if we're going up or down */
                if (up)
                {
                    /* Going up */
                    if (i == 0)
                    {
                        v = melo;
                    }
                    else if (i == 1)
                    {
                        v = me;
                    }
                    else
                    {
                        v = mehi;
                    }
                }
                else
                {
                    /* Going down */
                    if (i == 0)
                    {
                        v = mehi;
                    }
                    else if (i == 1)
                    {
                        v = me;
                    }
                    else
                    {
                        v = melo;
                    }
                }

                /* Adjust the proper channel */
                if (ch == 4)
                {
                    r = v;
                }
                else if (ch == 2)
                {
                    g = v;
                }
                else
                {
                    b = v;
                }

                /* Add the color */
                add64LevelColor(paletteEntries, (byte)r, (byte)g, (byte)b);
            }

            /* The next run starts at the starting position of this run, with the
               channel we just handled flipped */
            return (start ^ ch);
        }

        /// <summary>
        /// Adds a 24-color RGB cycle to the palette.
        /// A cycle consists of six 4-color runs, each of which transitions from one hue to another until arriving back at starting position.
        /// Intensities must be provided in 64-level scale.
        /// </summary>
        /// <param name="paletteEntries">The 64-level pallete</param>
        /// <param name="lo">low intensity value</param>
        /// <param name="melo">medium-low intensity value</param>
        /// <param name="me">medium intensity value</param>
        /// <param name="mehi">medium-high intensity value</param>
        /// <param name="hi">high intensity value</param>
        private static void addCycle(List<PaletteEntry> paletteEntries, int lo, int melo, int me, int mehi, int hi)
        {

            int hue = 0;

            /* Check parameters */
            if ((lo < 0) || (lo > 63) ||
                (melo < 0) || (melo > 63) ||
                (me < 0) || (me > 63) ||
                (mehi < 0) || (mehi > 63) ||
                (hi < 0) || (hi > 63))
            {
                return;
            }

            /* We start out at blue */
            hue = 1;

            /* Add each run, updating the hue each time */
            hue = add64Level4ColorsRun(paletteEntries, hue, 4, lo, melo, me, mehi, hi);
            hue = add64Level4ColorsRun(paletteEntries, hue, 1, lo, melo, me, mehi, hi);
            hue = add64Level4ColorsRun(paletteEntries, hue, 2, lo, melo, me, mehi, hi);

            hue = add64Level4ColorsRun(paletteEntries, hue, 4, lo, melo, me, mehi, hi);
            hue = add64Level4ColorsRun(paletteEntries, hue, 1, lo, melo, me, mehi, hi);
            hue = add64Level4ColorsRun(paletteEntries, hue, 2, lo, melo, me, mehi, hi);
        }

        /// <summary>
        /// Generates the original VGA 256 colors palette. 
        /// The palette is written here as 64-level.
        /// See more: https://en.wikipedia.org/wiki/Mode_13h
        /// </summary>
        public static List<PaletteEntry> BuildDefaultVga256Palette()
        {
            List<PaletteEntry> paletteEntries = new List<PaletteEntry>();

            /* Add the 16-color palette first */
            addTheStandard16Colors(paletteEntries, 0, 21, 42, 63);

            /* Next we have 16 shades of gray */
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 5);
            add64LevelGrayColor(paletteEntries, 8);
            add64LevelGrayColor(paletteEntries, 11);
            add64LevelGrayColor(paletteEntries, 14);
            add64LevelGrayColor(paletteEntries, 17);
            add64LevelGrayColor(paletteEntries, 20);
            add64LevelGrayColor(paletteEntries, 24);
            add64LevelGrayColor(paletteEntries, 28);
            add64LevelGrayColor(paletteEntries, 32);
            add64LevelGrayColor(paletteEntries, 36);
            add64LevelGrayColor(paletteEntries, 40);
            add64LevelGrayColor(paletteEntries, 45);
            add64LevelGrayColor(paletteEntries, 50);
            add64LevelGrayColor(paletteEntries, 56);
            add64LevelGrayColor(paletteEntries, 63);

            /* Now come the nine RGB cycles, organized in three groups of three
               cycles -- the groups represent high value, medium value, and low
               value, and the cycles within the groups represent high saturation,
               medium saturation, low saturation */
            addCycle(paletteEntries, 0, 16, 31, 47, 63);
            addCycle(paletteEntries, 31, 39, 47, 55, 63);
            addCycle(paletteEntries, 45, 49, 54, 58, 63);

            addCycle(paletteEntries, 0, 7, 14, 21, 28);
            addCycle(paletteEntries, 14, 17, 21, 24, 28);
            addCycle(paletteEntries, 20, 22, 24, 26, 28);

            addCycle(paletteEntries, 0, 4, 8, 12, 16);
            addCycle(paletteEntries, 8, 10, 12, 14, 16);
            addCycle(paletteEntries, 11, 12, 13, 15, 16);

            /* Fill the last eight palette entries with full black */
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);

            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);
            add64LevelGrayColor(paletteEntries, 0);

            return paletteEntries;
        }

        /// <summary>
        /// Converts a value from 64-level to 256-level.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte levelUpValue(byte value)
        {
            /* Verify range */
            if (value > 63)
            {
                throw new ArgumentException();
            }
            /* First, shift value two bits left */
            value = (byte)(value << 2);
            /* Second, duplicate two most significant bits of byte in two least significant bits */
            value = (byte)(value | (value >> 6));
            return value;
        }

        /// <summary>
        /// Converts a palette from 64-level to 256-level. 
        /// All the 256 colors of the palette must have been added already.
        /// </summary>
        /// <param name="paletteEntries">The 64-level palette</param>
        /// <returns></returns>
        public static List<PaletteEntry> LevelUpPalette(List<PaletteEntry> paletteEntries)
        {
            List<PaletteEntry> levelUpPaletteEntries = new List<PaletteEntry>();
            /* Palette must be full */
            if (paletteEntries.Count != 256)
            {
                throw new ArgumentException();
            }

            /* Convert each individual component from 64-level to 256-level */
            foreach (PaletteEntry paletteEntry in paletteEntries)
            {
                /* Convert component values */
                byte red = levelUpValue(paletteEntry.red);
                byte green = levelUpValue(paletteEntry.green);
                byte blue = levelUpValue(paletteEntry.blue);

                /* Write converted value back to palette */
                levelUpPaletteEntries.Add(new PaletteEntry(red, green, blue));
            }
            return levelUpPaletteEntries;
        }

        /// <summary>
        /// Builds a 256-level grayscale palette
        /// </summary>
        /// <returns></returns>
        public static List<PaletteEntry> BuildGrayscalePalette()
        {
            List<PaletteEntry> paletteEntries = new List<PaletteEntry>();
            for (int i = 0; i < 256; i++)
            {
                paletteEntries.Add(new PaletteEntry((byte)i, (byte)i, (byte)i));
            }
            return paletteEntries;
        }
    }
}
