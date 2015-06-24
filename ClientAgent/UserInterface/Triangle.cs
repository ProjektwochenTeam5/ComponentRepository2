using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// This class represents a triangle shape which points to the right.
    /// </summary>
    public class Triangle : Shape
    {
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(Triangle));

        /// <summary>
        /// Gets or sets the size (height and width) of this <see cref="Triangle"/> instance.
        /// </summary>
        public double Size
        {
            get { return (double)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Defines the triangle geometry.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                Point p1 = new Point(0, 0);
                Point p2 = new Point(0, this.Size);
                Point p3 = new Point(this.Size, this.Size / 2);

                List<PathSegment> segments = new List<PathSegment>(3);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));

                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p1, segments, true);
                figures.Add(pf);

                return new PathGeometry(figures, FillRule.EvenOdd, null);
            }
        }

    }
}
