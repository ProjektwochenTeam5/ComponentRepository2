using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// This class serves as a link between the output of a component and the input of another one.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// Sets the outgoing link of the source and the incoming link of the target to this new instance.
        /// </summary>
        /// <param name="source">The source output of this link.</param>
        /// <param name="target">The target input of this link.</param>
        /// <param name="line">The line which visually represents this link.</param>
        public Link(OutputControl source, InputControl target, Line line)
        {
            this.Source = source;
            this.Target = target;
            this.Line = line;
            source.OutgoingLink = this;
            target.IncomingLink = this;
        }

        /// <summary>
        /// The source output of this link.
        /// </summary>
        public OutputControl Source
        {
            get;
            private set;
        }

        /// <summary>
        /// The target input of this link.
        /// </summary>
        public InputControl Target
        {
            get;
            private set;
        }

        /// <summary>
        /// The line which visually represents this link.
        /// </summary>
        public Line Line
        {
            get;
            private set;
        }
    }
}
