using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CloudMaker
    {
        private readonly IWordFrequencyAnalyzer statsMaker;
        private readonly int amountOfWords;
        private readonly ICircularCloudLayouter layouter;
        private readonly IFontNormalizer normalizer;
        private readonly IImageBounder bounder;
        private readonly IWordCloudVisualisator visualisator;

        public CloudMaker(
            IWordFrequencyAnalyzer statsMaker,
            int amountOfWords,
            ICircularCloudLayouter layouter,
            IFontNormalizer normalizer,
            IImageBounder bounder,
            IWordCloudVisualisator visualisator)
        {
            this.statsMaker = statsMaker;
            this.amountOfWords = amountOfWords;
            this.layouter = layouter;
            this.normalizer = normalizer;
            this.bounder = bounder;
            this.visualisator = visualisator;
            
        }

        private IEnumerable<KeyValuePair<string, int>> MakeStats(IEnumerable<string> sourceData)
        {
            var stats = statsMaker.MakeStatisitcs(sourceData);
            return stats
                .OrderByDescending(kvp => kvp.Value)
                .Take(amountOfWords);
        }

        private IEnumerable<WordCloudElement> MakeWordCloudFromStats(
                IEnumerable<KeyValuePair<string, int>> stats, ICircularCloudLayouter layouter)
        {
            
            // ReSharper disable once PossibleMultipleEnumeration
            var maxWeight = stats.Max(kvp => kvp.Value);
            // ReSharper disable once PossibleMultipleEnumeration
            var minWeight = stats.Min(kvp => kvp.Value);

            WordCloudElement GetFontAndPutRectangle(KeyValuePair<string, int> kvp)
            {
                var fontSize = normalizer.GetFontHeghit(kvp.Value, minWeight, maxWeight);
                var font = new Font(FontFamily.GenericMonospace, fontSize);
                var rectangle = layouter.PutNextRectangle(new Size((int) Math.Round(fontSize) * kvp.Key.Length,
                    font.Height));
                return new WordCloudElement(kvp.Key, rectangle, font);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return stats.Select(GetFontAndPutRectangle).ToArray();
        }



        public Bitmap GetImage(IEnumerable<string> source)
        {
            var stats = MakeStats(source);
            var wordCloud = MakeWordCloudFromStats(stats, layouter)
                .Select(element =>
                    new WordCloudElement(element.Name, 
                        new Rectangle(
                            bounder.TransformRelativeToAbsoluteBounded(element.Rectangle.Location,
                                layouter.Center,
                                layouter.LeftBound, layouter.UpperBound),
                            element.Rectangle.Size),
                        element.Font));
            return visualisator.DrawWorldCloud(wordCloud);
        }
    }
}