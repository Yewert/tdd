using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public static class CoordinatesTransformer
    {
        public static (int X, int Y) TransformCoordinatesFromPolarToCartesian(double angle, double length)
        {
            var x = (int) (length * Math.Cos(angle));
            var y = (int) (length * Math.Sin(angle));
            return (x, y);
        }

        public static Point TransformRelativeToAbsoluteBounded(Point actualPoint, Point center, int boundX, int boundY)
        {
            return new Point(actualPoint.X + center.X - boundX, actualPoint.Y + center.Y - boundY);
        }
    }
}