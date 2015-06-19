using System.Windows.Shapes;

namespace UserInterface
{
    public class Link
    {
        public Link(OutputControl source, InputControl target, Line line)
        {
            this.Source = source;
            this.Target = target;
            this.Line = line;
            source.OutgoingLink = this;
            target.IncomingLink = this;
        }

        public OutputControl Source
        {
            get;
            private set;
        }

        public InputControl Target
        {
            get;
            private set;
        }

        public Line Line
        {
            get;
            private set;
        }
    }
}
