﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CloudMaker
    {
        private readonly float minFontSize;
        private readonly float maxFontSize;

        public CloudMaker(float minFontSize, float maxFontSize)
        {
            this.minFontSize = minFontSize;
            this.maxFontSize = maxFontSize;
        }

        public (Dictionary<string, (Rectangle rectangle, Font font)> wordCloud,
            Point center, int width, int height, int leftBound, int upperBound) MakeWordCloudFromStats(
                Dictionary<string, int> stats)
        {
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
            return (wordsInCloud, center, layouter.Width, layouter.Height, layouter.LeftBound, layouter.UpperBound);
        }
    }
}