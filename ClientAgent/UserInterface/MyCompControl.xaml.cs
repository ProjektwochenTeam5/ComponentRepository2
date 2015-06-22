using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using Core.Network;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MyCompControl.xaml
    /// </summary>
    public partial class MyCompControl : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> OnBGMouseLeftButtonDown;
        public event EventHandler<MouseButtonEventArgs> OnOutputMouseLeftButtonDown;
        public event EventHandler<MouseButtonEventArgs> OnInputMouseLeftButtonDown;
        public event EventHandler<DragEventArgs> OnInputDragOver;
        public event EventHandler<DragEventArgs> OnInputDrop;

        private MyComponent component = null;
        private bool selected = false;

        public MyCompControl(MyComponent component)
        {
            InputControl input;
            OutputControl output; 
            string[] hints = null;
            string[] descs = null;

            this.component = component;
            this.InternalComponentGuid = Guid.NewGuid();
            this.Inputs = new List<InputControl>();
            this.Outputs = new List<OutputControl>();

            InitializeComponent();

            Component childComponent = component.Component;

            this.lblName.Content = childComponent.FriendlyName;
            this.background.MouseLeftButtonDown += FireOnMouseLeftButtonDown;

            if (childComponent.InputHints != null)
            {
                hints = childComponent.InputHints.ToArray();
                descs = childComponent.InputDescriptions.ToArray();

                for (int i = 0; i < hints.Length; i++)
                {
                    input = new InputControl(this, (uint)(i + 1), hints[i], descs[i]);
                    input.AllowDrop = true;
                    input.MouseLeftButtonDown += FireOnInputMouseLeftButtonDown;
                    input.DragOver += FireOnInputDragOver;
                    input.Drop += FireOnInputDrop;

                    this.Inputs.Add(input);
                    this.stkInput.Children.Add(input);
                }
            }

            if (childComponent.OutputHints != null)
            {
                hints = childComponent.OutputHints.ToArray();
                descs = childComponent.OutputDescriptions.ToArray();

                for (int i = 0; i < hints.Length; i++)
                {
                    output = new OutputControl(this, (uint)(i + 1), hints[i], descs[i]);
                    output.MouseLeftButtonDown += FireOnOutputMouseLeftButtonDown;

                    this.Outputs.Add(output);
                    this.stkOutput.Children.Add(output);
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.selected;
            }

            set
            {
                this.selected = value;

                if (this.selected)
                {
                    this.background.Background = Brushes.Tomato;
                }
                else
                {
                    this.background.Background = Brushes.Silver;
                }
            }
        }

        public Guid InternalComponentGuid
        {
            get;
            private set;
        }

        public MyComponent Component
        {
            get
            {
                return this.component;
            }
        }

        public List<InputControl> Inputs
        {
            get;
            private set;
        }

        public List<OutputControl> Outputs
        {
            get;
            private set;
        }

        private void FireOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OnBGMouseLeftButtonDown != null)
            {
                this.OnBGMouseLeftButtonDown(this, e);
            }
        }

        private void FireOnInputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OnInputMouseLeftButtonDown != null)
            {
                this.OnInputMouseLeftButtonDown(this, e);
            }
        }

        private void FireOnOutputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OnOutputMouseLeftButtonDown != null)
            {
                this.OnOutputMouseLeftButtonDown(this, e);
            }
        }

        private void FireOnInputDragOver(object sender, DragEventArgs e)
        {
            if (this.OnInputDragOver != null)
            {
                this.OnInputDragOver(this, e);
            }
        }

        private void FireOnInputDrop(object sender, DragEventArgs e)
        {
            if (this.OnInputDrop != null)
            {
                this.OnInputDrop(this, e);
            }
        }


    }
}
