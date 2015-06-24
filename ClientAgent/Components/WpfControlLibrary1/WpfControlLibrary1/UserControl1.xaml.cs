﻿namespace WpfInput
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Core.Component;
    
    /// <summary>
    /// Interaction logic for UserControl.
    /// </summary>
    public partial class UserControl1 : UserControl, IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfInput"/> class.
        /// </summary>
        public UserControl1()
        {
            InitializeComponent();            
            this.ComponentGuid = new Guid();
            this.InputHints = new List<string>();
            this.OutputHints = new ReadOnlyCollection<string>(new[] { typeof(string).ToString() });
            this.InputDescriptions = new List<string>();
            this.OutputDescriptions = new List<string>();
        }

        /// <summary>
        /// Gets the input from user.
        /// </summary>
        /// <value>A input string.</value>
        public string Input
        { 
            get;
            private set; 
        }

        /// <summary>
        /// Gets the unique component id.
        /// Must be generated by the Store.
        /// DO NOT REUSE GUIDS.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentGuid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name for the component.
        /// </summary>
        /// <value>A name string.</value>
        public string FriendlyName
        {
            get { return "WPF Input"; }
        }

        /// <summary>
        /// Gets the collection of types that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputHints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of types that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputHints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the collection of strings that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of strings that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the implementation of the component.
        /// </summary>
        /// <param name="values">Collection of input arguments.</param>
        /// <returns>Collection of output arguments.</returns>
        public IEnumerable<object> Evaluate(IEnumerable<object> values)
        {            
            if (values.Count() != 0)
            {
                return new object[] { new ArgumentException() };
            }     
            else
            {
                return new object[] { Input };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {                
                Application.Current.Windows[0].Close(); 
                MessageBox.Show("Value saved!");                
            }
        }

        private bool CheckInput()
        {
            this.Input = txtinput.Text;

            if(string.IsNullOrWhiteSpace(this.Input))
            {
                txtinput.Background = Brushes.Yellow;
                txtinput.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void txtinput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtinput.Background == Brushes.Yellow)
            {
                txtinput.Text = string.Empty;
                txtinput.Background = Brushes.White;
            }
        }
    }
}
