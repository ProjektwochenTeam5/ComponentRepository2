using System.Windows.Controls;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for OutputControl.xaml
    /// </summary>
    public partial class OutputControl : UserControl
    {
        public OutputControl(MyCompControl parent, uint outputValueId, string hint, string description)
        {
            this.ParentComponent = parent;
            this.OutputValueID = outputValueId;
            this.Hint = hint;
            this.Description = description;

            InitializeComponent();

            this.lblDesc.DataContext = this;
        }

        public Link OutgoingLink
        {
            get;
            set;
        }

        public uint OutputValueID
        {
            get;
            private set;
        }

        public MyCompControl ParentComponent
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
