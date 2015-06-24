using System.Windows;
using System.Windows.Input;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for JobCreateWindow.xaml
    /// </summary>
    public partial class JobCreateWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobCreateWindow"/> class.
        /// </summary>
        public JobCreateWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the friendly name of the job entered by the user.
        /// </summary>
        public string JobName
        {
            get;
            private set;
        }

        /// <summary>
        /// Saves the text of the input box in this instance's JobName property and closes the window.
        /// </summary>
        private void btnExecuteJob_Click(object sender, RoutedEventArgs e)
        {
            this.JobName = txtJobName.Text;
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Saves the text of the input box in this instance's JobName property and closes the window if enter key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtJobName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.JobName = txtJobName.Text;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
