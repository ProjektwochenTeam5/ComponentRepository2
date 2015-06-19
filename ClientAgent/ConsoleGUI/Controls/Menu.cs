// --------------------------------------------------------------
// <copyright file="Menu.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Menu"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a menu.
    /// </summary>
    public class Menu : IRenderable
    {
        /// <summary>
        /// Contains the value for the <see cref="Menu.Rectangle"/> property.
        /// </summary>
        private Rectangle rectangleProperty;

        /// <summary>
        /// Contains the value for the <see cref="Menu.Visible"/> property.
        /// </summary>
        private bool visibleProperty;

        /// <summary>
        /// Contains the value for the <see cref="Menu.Focused"/> property.
        /// </summary>
        private bool focusedProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="renderer">
        ///     The renderer used to render the menu.
        /// </param>
        /// <param name="src">
        ///     The input source.
        /// </param>
        /// <param name="parent">
        ///     The menu that opened the new instance.
        /// </param>
        public Menu(IRenderer renderer, IInputSource src, Menu parent = null)
        {
            this.Buttons = new List<MenuButton>(12);
            this.Controls = new List<IRenderable>();
            this.InputReceivers = new List<IInputReceiver>();
            this.BackgroundColor = ConsoleColor.Blue;
            this.BorderForegroundColor = ConsoleColor.White;
            this.ForegroundColor = ConsoleColor.White;
            this.rectangleProperty = new Rectangle(0, 0, renderer.Width, 2);
            this.Renderer = renderer;
            this.Parent = parent;
            this.Input = src;
            src.InputReceived += this.Receive;
        }

        /// <summary>
        /// Raised when the <see cref="Menu.Visible"/> property was changed.
        /// </summary>
        public event EventHandler VisibleChanged;

        /// <summary>
        /// Raised when the <see cref="Menu.Focused"/> property was changed.
        /// </summary>
        public event EventHandler FocusedChanged;

        /// <summary>
        /// Gets the menu that opened the current menu.
        /// </summary>
        /// <value>
        ///     Contains the menu that opened the current menu.
        /// </value>
        public Menu Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the title of the menu.
        /// </summary>
        /// <value>
        ///     Contains the title of the menu.
        /// </value>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of controls in the menu.
        /// </summary>
        /// <value>
        ///     Contains the list of controls in the menu.
        /// </value>
        public List<IRenderable> Controls
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of menu buttons of the current menu.
        /// </summary>
        /// <value>
        ///     Contains the list of menu buttons of the current menu.
        /// </value>
        public List<MenuButton> Buttons
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of input receivers.
        /// </summary>
        /// <value>
        ///     Contains the list of input receivers.
        /// </value>
        public List<IInputReceiver> InputReceivers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the menu's background color.
        /// </summary>
        /// <value>
        ///     Contains the menu's background color.
        /// </value>
        public ConsoleColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the menu's foreground color.
        /// </summary>
        /// <value>
        ///     Contains the menu's foreground color.
        /// </value>
        public ConsoleColor ForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the menu's border color.
        /// </summary>
        /// <value>
        ///     Contains the menu's border color.
        /// </value>
        public ConsoleColor BorderForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the z position of the menu.
        /// </summary>
        /// <value>
        ///     Contains the z position of the menu.
        /// </value>
        public int Z
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu is focused.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the menu is focused.
        /// </value>
        public bool Focused
        {
            get
            {
                return this.focusedProperty;
            }

            set
            {
                this.focusedProperty = value;
                this.OnFocusedChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu is visible.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the menu is visible.
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
        /// Gets or sets the rectangle of the menu.
        /// </summary>
        /// <value>
        ///     Contains the rectangle of the menu.
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
            }
        }

        /// <summary>
        /// Gets the renderer for this menu.
        /// </summary>
        /// <value>
        ///     Contains the renderer for this menu.
        /// </value>
        public IRenderer Renderer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input source for this menu.
        /// </summary>
        /// <value>
        ///     Contains the input source for this menu.
        /// </value>
        public IInputSource Input
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pixels of the menu.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered menu.
        /// </returns>
        public Pixel[,] GetPixels()
        {
            Pixel[,] px = new Pixel[this.Renderer.Width, this.rectangleProperty.Height];

            // upper border
            for (int x = 0; x < this.rectangleProperty.Width; x++)
            {
                px[x, this.rectangleProperty.Height - 2] = new Pixel(this.BackgroundColor, this.BorderForegroundColor, '-');
            }

            // menu buttons
            int curX = 1;

            foreach (MenuButton btn in this.Buttons)
            {
                char c = ' ';

                for (int n = 0; n < btn.Text.Length && curX < this.rectangleProperty.Width; n++)
                {
                    if (btn.Visible)
                    {
                        c = btn.Text[n];
                    }

                    px[curX, this.rectangleProperty.Height - 1] = new Pixel(this.BackgroundColor, this.ForegroundColor, c);
                    curX++;
                }

                if (curX >= this.rectangleProperty.Width)
                {
                    break;
                }

                px[curX++, this.rectangleProperty.Height - 1] = new Pixel(this.BackgroundColor, this.BorderForegroundColor, '|');
            }

            for (; curX < this.rectangleProperty.Width - 1; curX++)
            {
                px[curX, this.rectangleProperty.Height - 1] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');
            }

            // left and right border
            px[0, this.rectangleProperty.Height - 1] = new Pixel(this.BackgroundColor, this.BorderForegroundColor, '|');
            px[this.rectangleProperty.Width - 1, this.rectangleProperty.Height - 1] = new Pixel(this.BackgroundColor, this.BorderForegroundColor, '|');

            return px;
        }

        /// <summary>
        /// Closes the current menu.
        /// </summary>
        public virtual void Close()
        {
            this.Focused = false;
            this.Visible = false;

            if (this.Parent != null)
            {
                this.Parent.Focused = true;
                this.Parent.Visible = true;
            }
        }

        /// <summary>
        /// Shows the current menu and renders its controls.
        /// </summary>
        public void Show()
        {
            this.Visible = true;
            this.Focused = true;

            this.Renderer.Clear(ConsoleColor.DarkBlue);

            this.Renderer.Draw(this.Renderer.Render(this.Controls.ToArray()), new Rectangle(0, 0, this.Renderer.Width, this.Renderer.Height - 2));
            this.Renderer.Draw(this.GetPixels(), new Rectangle(0, this.Renderer.Height - 2, this.Renderer.Width, 2));
        }

        /// <summary>
        /// Waits until the menu is closed.
        /// </summary>
        public void WaitForClose()
        {
            while (this.Visible)
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Receives and processes an input.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        public void Receive(object sender, InputReceivedEventArgs e)
        {
            if (!this.Focused || e.Processed)
            {
                return;
            }

            foreach (MenuButton btn in this.Buttons)
            {
                if (btn.LinkedKey == e.ReceivedKey.Key)
                {
                    btn.PressKey(e);

                    if (!e.Processed)
                    {
                        e.Process();
                    }

                    return;
                }
            }

            foreach (IInputReceiver r in this.InputReceivers)
            {
                if (r.Receive(e.ReceivedKey))
                {
                    e.Process();
                    return;
                }
            }

            this.ProcessMenuSpecific(e);
        }

        /// <summary>
        /// Processes menu-specific input.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event.
        /// </param>
        protected virtual void ProcessMenuSpecific(InputReceivedEventArgs e)
        {
        }

        /// <summary>
        /// Raises the <see cref="Menu.VisibleChanged"/> event.
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
        /// Raises the <see cref="Menu.FocusedChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnFocusedChanged(EventArgs e)
        {
            if (this.FocusedChanged != null)
            {
                this.FocusedChanged(this, e);
            }
        }
    }
}
