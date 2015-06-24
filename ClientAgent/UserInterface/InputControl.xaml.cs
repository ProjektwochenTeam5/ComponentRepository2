using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for InputControl.xaml
    /// Visually represents an input.
    /// </summary>
    public partial class InputControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputControl"/> class which
        /// visually represents an input.
        /// </summary>
        /// <param name="parent">The <see cref="MyCompControl"/> instance which contains this input.</param>
        /// <param name="inputValueId">The input port of the represented input.</param>
        /// <param name="hint">The type which the represented input accepts.</param>
        /// <param name="description">The description of the represented input.</param>
        public InputControl(MyCompControl parent, uint inputValueId, string hint, string description)
        {
            this.ParentControl = parent;
            this.InputValueID = inputValueId;
            this.Hint = hint;
            this.Description = description;

            InitializeComponent();

            this.lblDesc.DataContext = this;
        }
                
        /// <summary>
        /// Gets or sets the link which connects to this input.
        /// </summary>
        public Link IncomingLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the input port of the represented input.
        /// </summary>
        public uint InputValueID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="MyCompControl"/> instance which contains this input.
        /// </summary>
        public MyCompControl ParentControl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the represented input.
        /// </summary>
        public string Hint
        {
            get;
            private set;
        }

        /// <summary>
        /// The description of the represented input.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
    }
}
