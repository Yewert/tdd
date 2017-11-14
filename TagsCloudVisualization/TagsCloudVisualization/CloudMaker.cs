using System;
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

        public Bitmap MakeCloud(IEnumerable<string> sourceData,
            IWordFrequencyAnalyzer statsMaker,
            int amountOfWords,
            ICircularCloudLayouter layouter,
            IImageBounder bounder,
            IWordCloudVisualisator visualisator)
        {
            var stats = statsMaker.MakeStatisitcs(sourceData);
            var significantStats = stats    
                .OrderByDescending(kvp => kvp.Value)
                .Take(amountOfWords)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            var wordCloud = MakeWordCloudFromStats(significantStats, layouter)
                .Select(kvp =>
                    new KeyValuePair<string, (Rectangle rectangle, Font font)>(kvp.Key, 
                        (new Rectangle(
                            bounder.TransformRelativeToAbsoluteBounded(kvp.Value.rectangle.Location,
                                layouter.Center,
                                layouter.LeftBound, layouter.UpperBound),
                            kvp.Value.rectangle.Size),
                        kvp.Value.font)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return visualisator.DrawWorldCloud(wordCloud);
        }

        private Dictionary<string, (Rectangle rectangle, Font font)> MakeWordCloudFromStats(
                Dictionary<string, int> stats, ICircularCloudLayouter layouter)
        {
            
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
            return wordsInCloud;
        }
    }
}