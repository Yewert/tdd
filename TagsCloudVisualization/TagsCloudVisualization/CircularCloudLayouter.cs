using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
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

        public int Width => RightBound - LeftBound;
        public int Height => LowerBound - UpperBound;
        
        public CircularCloudLayouter(Point center)
        {
            this.center = center;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var upperLeftCorner = new Point(center.X - rectangleSize.Width / 2, center.Y - rectangleSize.Height / 2);
            LeftBound = upperLeftCorner.X;
            RightBound = upperLeftCorner.X + rectangleSize.Width;
            UpperBound = upperLeftCorner.Y;
            LowerBound = upperLeftCorner.Y + rectangleSize.Height;
            return new Rectangle(upperLeftCorner, rectangleSize);
        }
    }
}