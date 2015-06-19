// --------------------------------------------------------------
// <copyright file="Position.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Position"/> struct.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI
{
    /// <summary>
    /// Provides information about a 2-dimensional position.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// The value for the <see cref="X"/> property.
        /// </summary>
        private int xproperty;

        /// <summary>
        /// The value for the <see cref="Y"/> property.
        /// </summary>
        private int yproperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="x">
        ///     The x coordinate of the position.
        /// </param>
        /// <param name="y">
        ///     The y coordinate of the position.
        /// </param>
        public Position(int x, int y)
        {
            this.xproperty = x;
            this.yproperty = y;
        }

        /// <summary>
        /// Gets the x coordinate of the position.
        /// </summary>
        /// <value>
        ///     Contains the x coordinate of the position.
        /// </value>
        public int X
        {
            get
            {
                return this.xproperty;
            }

            private set
            {
                this.xproperty = value;
            }
        }

        /// <summary>
        /// Gets the y coordinate of the position.
        /// </summary>
        /// <value>
        ///     Contains the y coordinate of the position.
        /// </value>
        public int Y
        {
            get
            {
                return this.yproperty;
            }

            private set
            {
                this.yproperty = value;
            }
        }
    }
}
