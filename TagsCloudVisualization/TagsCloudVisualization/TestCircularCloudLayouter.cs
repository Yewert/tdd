using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        [Test]
        public void HasCorrectCount_WhenMultipleRectanglesAdded()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center);
            layouter.PutNextRectangle(size);
            layouter.PutNextRectangle(size);
            Assert.AreEqual(2, layouter.Count);
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

        
        //TODO: ЭТО ГОВНОКОД ДЛЯ ВИЗУАЛЬНОЙ ОЦЕНКИ, В ФИНАЛЬНОЙ ВЕРСИИ ВЫПИЛИТЬ
        [Test]
        public void l()
        {
            var center = new Point(0, 0);
            var size = new Size(1, 1);
            var layouter = new CircularCloudLayouter(center);
            var rects = new List<Rectangle>();
            for (int i = 0; i < 100; i++)
            {
                var s = new Size((int)(size.Width + i * 0.5), (int)(size.Height + i * 0.25));
                rects.Add(layouter.PutNextRectangle(s));
            }
            for (int i = 0; i < 100; i++)
            {
                rects[i] = new Rectangle(rects[i].X + center.X- layouter.LeftBound + 10,
                    rects[i].Y + center.Y - layouter.UpperBound + 10, rects[i].Width, rects[i].Height);
            }
            var image = new Bitmap(layouter.Width + 20, layouter.Height + 20);
            var graphics = Graphics.FromImage(image);
            graphics.DrawRectangles(Pens.Black, rects.ToArray());
            graphics.DrawEllipse(Pens.Blue, center.X + center.X - layouter.LeftBound + 10, center.Y + center.Y - layouter.UpperBound + 10, 1, 1);
            image.Save("C:/Users/Денис/Documents/C#/tdd/TagsCloudVisualization/cool3.png", ImageFormat.Png);
            Assert.True(true);
        }
    }
}