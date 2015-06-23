using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for StaticStringInputWindow.xaml
    /// </summary>
    public partial class StaticStringInputWindow : Window
    {
        public StaticStringInputWindow()
        {
            InitializeComponent();
        }

        public string Description
        {
            get;
            private set;
        }

        private void txtDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtDescription.Background == Brushes.Yellow)
            {
                this.txtInfo.Text = string.Empty;
                this.txtDescription.Background = Brushes.White;
            }
        }
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Checks if the input read from this window's Description text box is not empty
        /// and saves the input in the Description property.
        /// </summary>
        /// <returns>True, if the input is not empty.</returns>
        private bool CheckInput()
        {
            this.Description = this.txtDescription.Text;

            if (string.IsNullOrWhiteSpace(this.Description))
            {
                this.txtInfo.Text = "The input must not be empty.";
                this.txtDescription.Background = Brushes.Yellow;
                this.txtDescription.Focus();
                return false;
            }

            return true;
        }
    }
}
