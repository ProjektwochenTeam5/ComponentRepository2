using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for StaticStringInputWindow.xaml
    /// Enables the user to enter a string.
    /// </summary>
    public partial class StaticStringInputWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticStringInputWindow"/> class.
        /// </summary>
        public StaticStringInputWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticStringInputWindow"/> class.
        /// </summary>
        /// <param name="placeholder">The string which is filled in the input box on window startup.</param>
        public StaticStringInputWindow(string placeholder)
            : this()
        {
            this.Input = placeholder;
            this.txtInput.Text = placeholder;
            this.txtInput.SelectAll();
        }

        /// <summary>
        /// Gets the input set by the user or the value of the placeholder if the user did not change the value.
        /// </summary>
        public string Input
        {
            get;
            private set;
        }

        /// <summary>
        /// If the background of the input box is set to warning color, resets the color
        /// and empties the info box text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtInput.Background == Brushes.Yellow)
            {
                this.tbInfo.Text = string.Empty;
                this.txtInput.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Checks the input and closes the window if the input is valid.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// If enter key is pressed checks the input and closes the window if the input is valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Checks if the input read from this window's input box is not empty
        /// and saves the input in the Input property.
        /// If the input is not valid sets the background of the input box and writes a warning in the info box.
        /// </summary>
        /// <returns>True, if the input is not empty.</returns>
        private bool CheckInput()
        {
            this.Input = this.txtInput.Text;

            if (string.IsNullOrWhiteSpace(this.Input))
            {
                this.tbInfo.Text = "The input must not be empty.";
                this.txtInput.Background = Brushes.Yellow;
                this.txtInput.Focus();
                return false;
            }

            return true;
        }
    }
}
