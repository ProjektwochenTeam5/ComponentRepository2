using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for ComponentCreateWindow.xaml
    /// Provides the input elements for creating a new MyComponent.
    /// </summary>
    public partial class ComponentCreateWindow : Window
    {
        IEnumerable<MyComponent> availableComponents = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCreateWindow"/> class.
        /// </summary>
        /// <param name="alreadyAvailableComponents">
        /// A collection of the already available components used to determine if a friendly name is already in use.
        /// </param>
        public ComponentCreateWindow(IEnumerable<MyComponent> alreadyAvailableComponents)
        {
            this.availableComponents = alreadyAvailableComponents;

            InitializeComponent();
        }

        /// <summary>
        /// The entered friendly name for the new component.
        /// </summary>
        public string FriendlyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Closes this window if the input is valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Resets the text of this window's info textblock and the background of the input box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFriendlyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtFriendlyName.Background == Brushes.Yellow)
            {
                this.txtInfo.Text = string.Empty;
                this.txtFriendlyName.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Closes this window if 'Enter'-key is pressed and input is valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFriendlyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Checks if the input read from this window's FriendlyName text box is neither empty nor already in use
        /// and saves the input in the FriendlyName property.
        /// </summary>
        /// <returns>True, if the input is not empty and the friendly name is not already in use.</returns>
        private bool CheckInput()
        {
            this.FriendlyName = this.txtFriendlyName.Text;

            if (string.IsNullOrWhiteSpace(this.FriendlyName))
            {
                this.txtInfo.Text = "The input must not be empty.";
                this.txtFriendlyName.Background = Brushes.Yellow;
                this.txtFriendlyName.Focus();
                return false;
            }

            foreach (MyComponent c in this.availableComponents)
            {
                if (c.Component.FriendlyName == this.FriendlyName)
                {
                    this.txtInfo.Text = "This name is already in use.";
                    this.txtFriendlyName.Background = Brushes.Yellow;
                    this.txtFriendlyName.Focus();
                    return false;
                }
            }

            return true;
        }
    }
}
