using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for InputControl.xaml
    /// </summary>
    public partial class InputControl : UserControl
    {
        public InputControl(MyCompControl parent, uint inputValueId, string hint, string description)
        {
            this.ParentControl = parent;
            this.InputValueID = inputValueId;
            this.Hint = hint;
            this.Description = description;

            InitializeComponent();

            this.lblDesc.DataContext = this;
        }
                
        public Link IncomingLink
        {
            get;
            set;
        }

        public uint InputValueID
        {
            get;
            private set;
        }

        public MyCompControl ParentControl
        {
            get;
            private set;
        }

        public string Hint
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}
