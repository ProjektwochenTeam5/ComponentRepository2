// --------------------------------------------------------------
// <copyright file="IRenderer.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="IRenderer"/> interface.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;

    /// <summary>
    /// Provides an interface for 2d render engines.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Gets the width of the render surface.
        /// </summary>
        /// <value>
        ///     Contains the width of the render surface.
        /// </value>
        int Width { get; }

        /// <summary>
        /// Gets the height of the render surface.
        /// </summary>
        /// <value>
        ///     Contains the height of the render surface.
        /// </value>
        int Height { get; }

        /// <summary>
        /// Renders an array of <see cref="IRenderable"/> objects to a 2-dimensional <see cref="Pixel"/> array.
        /// </summary>
        /// <param name="objects">
        ///     The objects that shall be rendered.
        /// </param>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered pixels.
        /// </returns>
        Pixel[,] Render(params IRenderable[] objects);

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
        Pixel[,] Render(Rectangle rect, params IRenderable[] objects);

        /// <summary>
        /// Clears the render surface with a color.
        /// </summary>
        /// <param name="color">
        ///     The color used for clearing the drawing surface.
        /// </param>
        void Clear(ConsoleColor color);

        /// <summary>
        /// Draws a 2-dimensional <see cref="Pixel"/> array to the drawing surface.
        /// </summary>
        /// <param name="buffer">
        ///     The <see cref="Pixel"/> array that shall be drawn.
        /// </param>
        /// <param name="dest">
        ///     The destination rectangle to the drawing surface.
        /// </param>
        void Draw(Pixel[,] buffer, Rectangle dest);
    }
}
