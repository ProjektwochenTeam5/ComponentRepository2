
namespace ConsoleGUI.Controls
{
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a way to communicate with a drawing thread.
    /// </summary>
    internal class DrawThreadArgs
        : ThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawThreadArgs"/> class.
        /// </summary>
        /// <param name="r">
        ///     The renderer that shall render the content.
        /// </param>
        /// <param name="cnt">
        ///     The content that shall be rendered.
        /// </param>
        /// <param name="target">
        ///     The target rectangle.
        /// </param>
        public DrawThreadArgs(IRenderer r, Pixel[,] cnt, Rectangle target)
            : base()
        {
            this.AssignedRenderer = r;
            this.DrawContent = cnt;
            this.TargetRectangle = target;
        }

        /// <summary>
        /// Gets the renderer that shall render the content.
        /// </summary>
        /// <value>
        ///     Contains the renderer that shall render the content.
        /// </value>
        public IRenderer AssignedRenderer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the content that shall be drawn.
        /// </summary>
        /// <value>
        ///     Contains the content that shall be drawn.
        /// </value>
        public Pixel[,] DrawContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the target rectangle where to render.
        /// </summary>
        /// <value>
        ///     Contains the target rectangle where to render.
        /// </value>
        public Rectangle TargetRectangle
        {
            get;
            private set;
        }
    }
}
