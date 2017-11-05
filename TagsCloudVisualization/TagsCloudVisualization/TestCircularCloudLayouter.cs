﻿using System;
using System.Drawing;
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

        [TestCase(2)]
        [TestCase(100)]
        [TestCase(1000)]
        public void HasCorrectCount_WhenMultipleRectanglesAdded(int count)
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center);
            for (int i = 0; i < count; i++)
            {
                layouter.PutNextRectangle(size);
            }
            Assert.AreEqual(count, layouter.Count);
        }

        [Test]
        public void RectanglesDoNotIntersect_WhenPutTwoRectangles()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center);
            var rect1 = layouter.PutNextRectangle(size);
            var rect2 = layouter.PutNextRectangle(size);
            Assert.False(rect1.IntersectsWith(rect2));
        }

        [Test, Timeout(2000)]
        public void WorksFast_When500RectanglesArePutIn()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center);
            for (int i = 0; i < 500; i++)
            {
                layouter.PutNextRectangle(size);
            }
        }

        [TestCase(-1, 1)]
        [TestCase(0, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 0)]
        [TestCase(0, 0)]
        [TestCase(-1, -1)]
        public void ThrowsArgumentException_WhenSizeHasAtLeastOneNonPositivDimension(int width, int height)
        {
            var center = new Point(0, 0);
            var size = new Size(width, height);
            var layouter = new CircularCloudLayouter(center);
            Assert.Throws<ArgumentException>(() => layouter.PutNextRectangle(size));
        }
    }
}