using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class Visualisation
    {
        private readonly float maxFontSize;
        private readonly float minFontSize;
        private readonly bool debug;

        public Visualisation() : this(150.0f, 20.0f, true)
        {
        }
        
        public Visualisation(float maxFontSize, float minFontSize, bool debug)
        {
            if (minFontSize < 0 || maxFontSize < 0 || maxFontSize <= minFontSize)
                throw new ArgumentException();
            this.maxFontSize = maxFontSize;
            this.minFontSize = minFontSize;
            this.debug = debug;
        }

        public Bitmap MakeWordClod(Dictionary<string, int> stats)
        {
            if(stats is null)
                throw new ArgumentNullException();
            
            var center = new Point(0, 0);
            var layouter = new CircularCloudLayouter(center);
            
            var maxWeight = stats.Max(kvp => kvp.Value);
            var minWeight = stats.Min(kvp => kvp.Value);

            var wordsInCloud = stats.ToDictionary(kvp => kvp.Key, kvp =>
            {
                var fontSize = Math.Max(maxFontSize * (kvp.Value - minWeight) / (maxWeight - minWeight), minFontSize);
                var font = new Font(FontFamily.GenericMonospace, fontSize);
                var rectangle =
                    layouter.PutNextRectangle(new Size((int) Math.Round(fontSize) * kvp.Key.Length, font.Height));
                return (rectangle, font);
            });
            
            var image = new Bitmap(layouter.Width + 20, layouter.Height + 20);
            using (var graphics = Graphics.FromImage(image))
            {
                foreach (var kvp in wordsInCloud)
                {
                    graphics.DrawString(kvp.Key, kvp.Value.font, Brushes.Black,
                        new PointF(kvp.Value.rectangle.X + center.X - layouter.LeftBound + 10,
                        kvp.Value.rectangle.Y + center.Y - layouter.UpperBound + 10));
                    if (debug)
                    {
                        graphics.DrawRectangle(Pens.Blue, new Rectangle(
                            new Point(kvp.Value.rectangle.X + center.X - layouter.LeftBound + 10,
                                kvp.Value.rectangle.Y + center.Y - layouter.UpperBound + 10),
                            kvp.Value.rectangle.Size));
                    }
                }
            }
            return image;
        }
    }
}