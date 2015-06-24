using System.Windows.Controls;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for OutputControl.xaml
    /// Visually represents an output.
    /// </summary>
    public partial class OutputControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputControl"/> class which
        /// visually represents an input.
        /// </summary>
        /// <param name="parent">The <see cref="MyCompControl"/> instance which contains this output.</param>
        /// <param name="inputValueId">The output port of the represented output.</param>
        /// <param name="hint">The type which the represented output accepts.</param>
        /// <param name="description">The description of the represented output.</param>
        public OutputControl(MyCompControl parent, uint outputValueId, string hint, string description)
        {
            this.ParentControl = parent;
            this.OutputValueID = outputValueId;
            this.Hint = hint;
            this.Description = description;

            InitializeComponent();

            this.lblDesc.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the link which connects to this output.
        /// </summary>
        public Link OutgoingLink
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the output port of the represented output.
        /// </summary>
        public uint OutputValueID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="MyCompControl"/> instance which contains this output.
        /// </summary>
        public MyCompControl ParentControl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the represented output.
        /// </summary>
        public string Hint
        {
            get;
            private set;
        }

        /// <summary>
        /// The description of the represented output.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
    }
}
