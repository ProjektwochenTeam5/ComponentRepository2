// --------------------------------------------------------------
// <copyright file="Control.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the abstract <see cref="Control"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a base class for all controls. This class is abstract.
    /// </summary>
    public abstract class Control
        : IRenderable, IInputReceiver
    {
        /// <summary>
        /// The value for the <see cref="BackgroundColor"/> property.
        /// </summary>
        private ConsoleColor backgroundColorProperty;

        /// <summary>
        /// The value for the <see cref="ForegroundColor"/> property.
        /// </summary>
        private ConsoleColor foregroundColorProperty;

        /// <summary>
        /// The value for the <see cref="ZIndex"/> property.
        /// </summary>
        private int zproperty;

        /// <summary>
        /// The value for the <see cref="Rectangle"/> property.
        /// </summary>
        private Rectangle rectangleProperty;

        /// <summary>
        /// The value for the <see cref="Visible"/> property.
        /// </summary>
        private bool visibleProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="outputs">
        ///     The output renderer the control is assigned to.
        /// </param>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown when the specified <see cref="IRenderer"/> is null.
        /// </exception>
        public Control(ICollection<IRenderer> outputs)
        {
            if (outputs.Count == 0)
            {
                throw new ArgumentException();
            }

            this.Controls = new ObservableCollection<IRenderable>();
            this.Controls.CollectionChanged += this.Controls_CollectionChanged;
            this.Renderers = outputs;
        }

        /// <summary>
        /// Raised when the <see cref="BackgroundColor"/> property was changed.
        /// </summary>
        public event EventHandler BackgroundColorChanged;

        /// <summary>
        /// Raised when the <see cref="ForegroundColor"/> property was changed.
        /// </summary>
        public event EventHandler ForegroundColorChanged;

        /// <summary>
        /// Raised when the <see cref="Rectangle"/> property was changed.
        /// </summary>
        public event EventHandler RectangleChanged;

        /// <summary>
        /// Raised when the <see cref="Visible"/> property was changed.
        /// </summary>
        public event EventHandler VisibleChanged;

        /// <summary>
        /// Raised when the <see cref="ZIndex"/> property was changed.
        /// </summary>
        public event EventHandler ZIndexchanged;

        /// <summary>
        /// Raised when the <see cref="Controls"/> property was modified.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ControlsChanged;

        /// <summary>
        /// Gets the list of owned controls.
        /// </summary>
        public ObservableCollection<IRenderable> Controls
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the owner of the control.
        /// </summary>
        /// <value>
        ///     Contains the owner of the control.
        /// </value>
        public Control Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the background color of the control.
        /// </summary>
        /// <value>
        ///     Contains the background color of the control.
        /// </value>
        public ConsoleColor BackgroundColor
        {
            get
            {
                return this.backgroundColorProperty;
            }

            set
            {
                this.backgroundColorProperty = value;
                this.OnBackgroundColorChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <value>
        ///     Contains the foreground color of the control.
        /// </value>
        public ConsoleColor ForegroundColor
        {
            get
            {
                return this.foregroundColorProperty;
            }

            set
            {
                this.foregroundColorProperty = value;
                this.OnForegroundColorChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the <see cref="IRenderer"/> instances rendering this control.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="IRenderer"/> instances rendering this control.
        /// </value>
        public ICollection<IRenderer> Renderers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the z coordinate of the control.
        /// </summary>
        /// <value>
        ///     Contains the z coordinate of the control.
        ///     It indicates which control shall be layered over another.
        /// </value>
        public int Z
        {
            get
            {
                return this.zproperty;
            }

            set
            {
                this.zproperty = value;
                this.OnZIndexChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the rectangle of the control.
        /// </summary>
        /// <value>
        ///     Contains the rectangle of the control.
        /// </value>
        public Rectangle Rectangle
        {
            get
            {
                return this.rectangleProperty;
            }

            set
            {
                this.rectangleProperty = value;
                this.OnRectangleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is visible.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the control is visible
        /// </value>
        public bool Visible
        {
            get
            {
                return this.visibleProperty;
            }

            set
            {
                this.visibleProperty = value;
                this.OnVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the pixels of the control. This method is abstract.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array describing the way the control shall be displayed.
        /// </returns>
        public abstract Pixel[,] GetPixels();

        /// <summary>
        /// Sends a key to the <see cref="Control"/> instance. This method is abstract.
        /// </summary>
        /// <param name="k">
        ///     The key that shall be sent to the control.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the <see cref="Control"/> instance
        ///     accepted the <see cref="ConsoleKeyInfo"/>.
        /// </returns>
        public abstract bool Receive(ConsoleKeyInfo k);

        /// <summary>
        /// Sends a string to the <see cref="Control"/> instance. This method is abstract.
        /// </summary>
        /// <param name="s">
        ///     The string that shall be sent to the <see cref="Control"/> instance.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the <see cref="Control"/> instance
        ///     accepted the <see cref="string"/>.
        /// </returns>
        public abstract bool Receive(string s);

        /// <summary>
        /// Draws the control.
        /// </summary>
        /// <param name="rect">
        ///     The target rectangle.
        /// </param>
        public virtual void Draw(Rectangle rect)
        {
            Pixel[,] px = this.GetPixels();

            foreach (IRenderer r in this.Renderers)
            {
                Thread t = new Thread(this.DrawThread);
                DrawThreadArgs d = new DrawThreadArgs(r, px, rect);
                t.Start(d);
            }
        }

        /// <summary>
        /// Raises the <see cref="BackgroundColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnBackgroundColorChanged(EventArgs e)
        {
            if (this.BackgroundColorChanged != null)
            {
                this.BackgroundColorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ForegroundColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnForegroundColorChanged(EventArgs e)
        {
            if (this.ForegroundColorChanged != null)
            {
                this.ForegroundColorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RectangleChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnRectangleChanged(EventArgs e)
        {
            if (this.RectangleChanged != null)
            {
                this.RectangleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnVisibleChanged(EventArgs e)
        {
            if (this.VisibleChanged != null)
            {
                this.VisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ZIndexChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnZIndexChanged(EventArgs e)
        {
            if (this.ZIndexchanged != null)
            {
                this.ZIndexchanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ControlsChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnControlsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.ControlsChanged != null)
            {
                this.ControlsChanged(this, e);
            }
        }

        /// <summary>
        /// The thread that draws the control.
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        protected virtual void DrawThread(object data)
        {
            DrawThreadArgs args = (DrawThreadArgs)data;
            args.AssignedRenderer.Draw(args.DrawContent, args.TargetRectangle);
        }

        /// <summary>
        /// Called when the <see cref="Controls"/> collection was modified.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void Controls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Draw(this.rectangleProperty);
        }
    }
}
