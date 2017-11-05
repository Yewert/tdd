using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private const double DeltaAngle = Math.PI / 15;
        
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
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException();
            var rectangle = PlaceNewRectangle(rectangleSize);
            if (rectangle is null)
            {
                // Не знаю, как протестировать/искоренить случай не нахождения места прямоугольника,
                // когда пробежал углы [0; int.Max * PI / 30] в полярной системе координат
                throw new NotSupportedException();
            }
            LeftBound = rectangle.Value.X;
            RightBound = rectangle.Value.X + rectangleSize.Width;
            UpperBound = rectangle.Value.Y;
            LowerBound = rectangle.Value.Y + rectangleSize.Height;
            rectangles.Add(rectangle.Value);
            return rectangle.Value;
        }

        private Rectangle? PlaceNewRectangle(Size rectangleSize)
        {
            var square = unchecked(rectangleSize.Height * rectangleSize.Width);
            square = square < 0 ? int.MaxValue : square;
            var stretchCoefficient = (1.0 / square);
            var i = 0;
            while (true)
            {
                if (i == int.MaxValue)
                    return null;
                var angle = i * DeltaAngle
                            *(0.75 / (i * stretchCoefficient + 1) + 0.5);
                var shiftFromCenter = TransformCoordinatesFromPolarToCartesian(angle, angle);
                var upperLeftCorner = new Point(center.X + shiftFromCenter.X - rectangleSize.Width / 2,
                    center.Y + shiftFromCenter.Y - rectangleSize.Height / 2);
                var rectangle = new Rectangle(upperLeftCorner, rectangleSize);
                if (!IntersectsWithAnyOtherRectangle(rectangle))
                    return rectangle;
                i++;
            }
        }

        private bool IntersectsWithAnyOtherRectangle(Rectangle rect)
        {
            return rectangles.Any(r => r.IntersectsWith(rect));
        }

        private static (int X, int Y) TransformCoordinatesFromPolarToCartesian(double angle, double length)
        {
            var x = (int) (length * Math.Cos(angle));
            var y = (int) (length * Math.Sin(angle));
            return (x, y);
        }
    }
}