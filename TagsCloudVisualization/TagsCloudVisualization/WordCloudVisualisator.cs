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
        public Bitmap DrawWorldCloud(Dictionary<string, (Rectangle rectangle, Font font)> wordCloud,
            Point center, int width, int height, int leftBound, int upperBound)
        {
            if(wordCloud is null)
                throw new ArgumentNullException();
            
            
            
            var image = new Bitmap(width + 2 * margin, height + 2 * margin);
            using (var graphics = Graphics.FromImage(image))
            {
                foreach (var kvp in wordCloud)
                {
                    graphics.DrawString(kvp.Key, kvp.Value.font, Brushes.Black,
                        new PointF(kvp.Value.rectangle.X + center.X - leftBound + margin,
                        kvp.Value.rectangle.Y + center.Y - upperBound + margin));
                    if (debug)
                    {
                        graphics.DrawRectangle(Pens.Blue, new Rectangle(
                            new Point(kvp.Value.rectangle.X + center.X - leftBound + margin,
                                kvp.Value.rectangle.Y + center.Y - upperBound + margin),
                            kvp.Value.rectangle.Size));
                    }
                }
            }
            return image;
        }
    }
}