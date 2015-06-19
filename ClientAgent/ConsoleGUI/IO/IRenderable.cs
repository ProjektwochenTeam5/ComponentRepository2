// --------------------------------------------------------------
// <copyright file="IRenderable.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="IRenderable"/> interface.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for 2d objects that can be rendered.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Gets the renderers used for rendering this <see cref="IRenderable"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the renderers used for rendering this <see cref="IRenderable"/> instance.
        /// </value>
        ICollection<IRenderer> Renderers { get; }

        /// <summary>
        /// Gets the z coordinate of this <see cref="IRenderable"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the z coordinate of this <see cref="IRenderable"/> instance.
        /// </value>
        int Z { get; }

        /// <summary>
        /// Gets the rectangle of this <see cref="IRenderable"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the rectangle of this <see cref="IRenderable"/> instance.
        /// </value>
        Rectangle Rectangle { get; }

        /// <summary>
        /// Gets the pixels of this <see cref="IRenderable"/> instance.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="Server.Pixel"/> instance representing an abstract display of this <see cref="IRenderable"/> instance.
        /// </returns>
        Pixel[,] GetPixels();
    }
}
