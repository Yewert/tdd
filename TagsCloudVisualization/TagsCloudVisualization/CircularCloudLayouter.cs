using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private const double deltaAngle = Math.PI / 16;
        
        private readonly Point center;

        public int LeftBound
        {
            get => leftBound;
            private set => leftBound = Math.Min(leftBound, value);
        }

        public int RightBound
        {
            get => rightBound;
            private set => rightBound = Math.Max(rightBound, value);
        }

        public int UpperBound
        {
            get => upperBound;
            private set => upperBound = Math.Min(upperBound, value);
        }

        public int LowerBound
        {
            get => lowerBound;
            private set => lowerBound = Math.Max(lowerBound, value);
        }

        private int leftBound = 0;
        private int rightBound = 0;
        private int upperBound = 0;
        private int lowerBound = 0;
        
        private readonly List<Rectangle> rectangles;

        public int Count => rectangles.Count;

        public int Width => RightBound - LeftBound;
        public int Height => LowerBound - UpperBound;
        
        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            this.rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var rectangle = PlaceNewRectangle(rectangleSize);
            LeftBound = rectangle.X;
            RightBound = rectangle.X + rectangleSize.Width;
            UpperBound = rectangle.Y;
            LowerBound = rectangle.Y + rectangleSize.Height;
            rectangles.Add(rectangle);
            return rectangle;
        }

        private Rectangle PlaceNewRectangle(Size rectangleSize)
        {
            var i = 0;
            while (true)
            {
                var shiftFromCenter = TransformCoordinatesFromPolarToCartesian(i * deltaAngle, i * deltaAngle);
                var upperLeftCorner = new Point(center.X + shiftFromCenter.X - rectangleSize.Width / 2,
                    center.Y + shiftFromCenter.Y - rectangleSize.Height / 2);
                var rectangle = new Rectangle(upperLeftCorner, rectangleSize);
                if (!IntersectsWithAnyOtherRectangle(rectangle))
                    return rectangle;
                i++;
            }
        }

        public bool IntersectsWithAnyOtherRectangle(Rectangle rect)
        {
            return rectangles.Any(r => r.IntersectsWith(rect));
        }

        private (int X, int Y) TransformCoordinatesFromPolarToCartesian(double angle, double length)
        {
            var x = (int) (length * Math.Cos(angle));
            var y = (int) (length * Math.Sin(angle));
            return (x, y);
        }
    }
}