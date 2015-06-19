// --------------------------------------------------------------
// <copyright file="ConsoleRenderer.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ConsoleRenderer"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an advanced thread-safe way for writing to a console window.
    /// </summary>
    public class ConsoleRenderer : IRenderer
    {
        /// <summary>
        /// The locker object that prevents multiple threads from writing to the console.
        /// </summary>
        private static object writeLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRenderer"/> class.
        /// </summary>
        public ConsoleRenderer()
        {
            Console.WindowWidth = 80;
            Console.WindowHeight = 27;
        }

        /// <summary>
        /// Gets the width of the console's render surface.
        /// </summary>
        /// <value>
        ///     Contains the width of the console's render surface.
        /// </value>
        public int Width
        {
            get { return Console.WindowWidth; }
        }

        /// <summary>
        /// Gets the height of the console's render surface.
        /// </summary>
        /// <value>
        ///     Contains the height of the console's render surface.
        /// </value>
        public int Height
        {
            get { return Console.WindowHeight; }
        }

        /// <summary>
        /// Renders an array of <see cref="IRenderable"/> objects to a 2-dimensional <see cref="Pixel"/> array.
        /// </summary>
        /// <param name="objects">
        ///     The objects that shall be rendered.
        /// </param>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered pixels.
        /// </returns>
        public Pixel[,] Render(params IRenderable[] objects)
        {
            return this.Render(new Rectangle(0, 0, Console.WindowWidth, Console.WindowHeight), objects);
        }

        /// <summary>
        /// Renders an array of <see cref="IRenderable"/> objects to a 2-dimensional <see cref="Pixel"/> array.
        /// </summary>
        /// <param name="rect">
        ///     The rectangle that limits the rendered area.
        /// </param>
        /// <param name="objects">
        ///     The objects that shall be rendered.
        /// </param>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered pixels.
        /// </returns>
        public Pixel[,] Render(Rectangle rect, params IRenderable[] objects)
        {
            Pixel[,] px = GetFilled(ConsoleColor.Black, ConsoleColor.White, ' ', rect.Width, rect.Height);
            IRenderable[,] objmask = new IRenderable[rect.Width, rect.Height];

            foreach (IRenderable r in objects)
            {
                Pixel[,] rp = r.GetPixels();
                for (int y = r.Rectangle.Y; y < r.Rectangle.RangeY && y < rect.Height; y++)
                {
                    if (y < 0)
                    {
                        continue;
                    }

                    for (int x = r.Rectangle.X; x < r.Rectangle.RangeX && x < rect.Width; x++)
                    {
                        if (x < 0)
                        {
                            continue;
                        }

                        if ((objmask[x, y] != null && objmask[x, y].Z < r.Z) || objmask[x, y] == null)
                        {
                            px[x, y] = rp[x - r.Rectangle.X, y - r.Rectangle.Y];
                        }
                    }
                }
            }

            return px;
        }

        /// <summary>
        /// Draws a 2-dimensional <see cref="Pixel"/> array to the console.
        /// </summary>
        /// <param name="buffer">
        ///     The <see cref="Pixel"/> array that shall be drawn.
        /// </param>
        /// <param name="dest">
        ///     The destination rectangle in the console window.
        /// </param>
        public void Draw(Pixel[,] buffer, Rectangle dest)
        {
            lock (writeLock)
            {
                Console.CursorVisible = false;

                for (int y = dest.Y; y < dest.RangeY && y < Console.WindowHeight; y++)
                {
                    for (int x = dest.X; x < dest.RangeX && x < Console.WindowWidth; x++)
                    {
                        Pixel current = buffer[x - dest.X, y - dest.Y];

                        Console.SetCursorPosition(x, y);
                        Console.BackgroundColor = current.BackgroundColor;
                        Console.ForegroundColor = current.ForegroundColor;
                        Console.Write(current.Char);
                    }
                }

                Console.SetCursorPosition(0, 0);
            }
        }

        /// <summary>
        /// Clears the console by using a color.
        /// </summary>
        /// <param name="color">
        ///     The color the console window shall be filled with.
        /// </param>
        public void Clear(ConsoleColor color = ConsoleColor.Black)
        {
            lock (writeLock)
            {
                Console.BackgroundColor = color;
                Console.Clear();
            }
        }

        /// <summary>
        /// Builds a 2-dimensional <see cref="Pixel"/> array where all pixels have the same background and foreground color and the same character.
        /// </summary>
        /// <param name="background">
        ///     The background color of the pixels.
        /// </param>
        /// <param name="foreground">
        ///     The foreground color of the pixels.
        /// </param>
        /// <param name="c">
        ///     The character of the pixels.
        /// </param>
        /// <param name="width">
        ///     The width of the array.
        /// </param>
        /// <param name="height">
        ///     The height of the array.
        /// </param>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array.
        /// </returns>
        private static Pixel[,] GetFilled(ConsoleColor background, ConsoleColor foreground, char c, int width, int height)
        {
            Pixel[,] ret = new Pixel[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ret[x, y] = new Pixel(background, foreground, c);
                }
            }

            return ret;
        }
    }
}
