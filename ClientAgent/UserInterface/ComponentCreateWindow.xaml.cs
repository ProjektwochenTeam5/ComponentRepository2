using Core.Network;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for ComponentCreateWindow.xaml
    /// </summary>
    public partial class ComponentCreateWindow : Window
    {
        IEnumerable<Component> availableComponents = null;

        public ComponentCreateWindow(IEnumerable<Component> alreadyAvailableComponents)
        {
            this.availableComponents = alreadyAvailableComponents;

            InitializeComponent();
        }

        public string FriendlyName
        {
            get;
            private set;
        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void txtFriendlyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtFriendlyName.Background == Brushes.Yellow)
            {
                this.txtInfo.Text = string.Empty;
                this.txtFriendlyName.Background = Brushes.White;
            }
        }

        private void txtFriendlyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CheckInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

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

            foreach (Component c in this.availableComponents)
            {
                if (c.FriendlyName == this.FriendlyName)
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
