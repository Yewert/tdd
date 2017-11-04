using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class TestCircularCloudLayouter
    {
        [TestCase(0, 0, 10, 10, -5, -5)]
        [TestCase(10, 10, 10, 10, 5, 5)]
        [TestCase(10, 10, 11, 11, 5, 5)]
        public void PutsInCenter_WhenFirstRectangleGiven(
            int centerX, int centerY, int rectangleWidth, int rectangleHeight, int rectangleX, int rectangleY)
        {
            var center = new Point(centerX, centerY);
            var size = new Size(rectangleWidth, rectangleHeight);
            var rectangleCoords = new Point(rectangleX, rectangleY);
            var rectangle = new Rectangle(rectangleCoords, size);
            var layouter = new CircularCloudLayouter(center);
            layouter.PutNextRectangle(size).ShouldBeEquivalentTo(rectangle);
        }

        [Test]
        public void HasCorrectBounds_AfterPuttingFirstRectangle()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center);
            layouter.PutNextRectangle(size);
            (int left, int right, int top, int bottom, int width, int height) actualDimensions =
                (layouter.LeftBound, layouter.RightBound,
                layouter.UpperBound, layouter.LowerBound,
                layouter.Width, layouter.Height);
            actualDimensions.ShouldBeEquivalentTo((-5, 5, -5, 5, 10, 10));
        }
    }
}