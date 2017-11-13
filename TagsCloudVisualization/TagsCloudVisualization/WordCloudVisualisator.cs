using System;
using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class WordCloudVisualisator
    {
        private readonly int margin;
        private readonly bool debug;

        public WordCloudVisualisator(bool debug) : this(10, debug)
        {
        }
        
        public WordCloudVisualisator(int margin, bool debug)
        {
            this.margin = margin;
            this.debug = debug;
        }
        public Bitmap DrawWorldCloud(IEnumerable<KeyValuePair<string, (Rectangle rectangle, Font font)>> wordCloud)
        {
            if(wordCloud is null)
                throw new ArgumentNullException();

            // ReSharper disable once PossibleMultipleEnumeration
            var (width, height) = CalculateWidthAndHeight(wordCloud);
            
            var image = new Bitmap(width + 2 * margin, height + 2 * margin);
            using (var graphics = Graphics.FromImage(image))
            {
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var kvp in wordCloud)
                {
                    graphics.DrawString(kvp.Key, kvp.Value.font, Brushes.Black,
                        new PointF(kvp.Value.rectangle.X + margin,
                        kvp.Value.rectangle.Y + margin));
                    if (debug)
                    {
                        graphics.DrawRectangle(Pens.Blue, new Rectangle(
                            new Point(kvp.Value.rectangle.X + margin,
                                kvp.Value.rectangle.Y +  margin),
                            kvp.Value.rectangle.Size));
                    }
                }
            }
            return image;
        }

        private static (int width, int height) CalculateWidthAndHeight(IEnumerable<KeyValuePair<string, (Rectangle rectangle, Font font)>> wordCloud)
        {
            var leftBound = 0;
            var rightBound = 0;
            var upperBound = 0;
            var lowerBound = 0;
            foreach (var keyValuePair in wordCloud)
            {
                leftBound = Math.Min(leftBound, keyValuePair.Value.rectangle.X);
                upperBound = Math.Min(upperBound, keyValuePair.Value.rectangle.Y);
                rightBound = Math.Max(rightBound, keyValuePair.Value.rectangle.X + keyValuePair.Value.rectangle.Width);
                lowerBound = Math.Max(lowerBound, keyValuePair.Value.rectangle.Y + keyValuePair.Value.rectangle.Height);
            }
            return (rightBound - leftBound, lowerBound - upperBound);
        }
    }
}