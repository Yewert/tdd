using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class WordCloud
    {
        private readonly IFontNormalizer normalizer;
        private readonly IWordCloudVisualisator visualisator;
        private readonly WordCloudElement[] wordCloud;

        public WordCloud(IEnumerable<string> sourceData,
            IWordFrequencyAnalyzer statsMaker,
            int amountOfWords,
            ICircularCloudLayouter layouter,
            IFontNormalizer normalizer,
            IImageBounder bounder,
            IWordCloudVisualisator visualisator)
        {
            this.normalizer = normalizer;
            this.visualisator = visualisator;
            var stats = MakeStats(sourceData, statsMaker, amountOfWords);
            wordCloud = MakeWordCloudFromStats(stats, layouter)
                .Select(element =>
                    new WordCloudElement(element.Name, 
                        new Rectangle(
                            bounder.TransformRelativeToAbsoluteBounded(element.Rectangle.Location,
                                layouter.Center,
                                layouter.LeftBound, layouter.UpperBound),
                            element.Rectangle.Size),
                        element.Font)).ToArray();
        }

        private IEnumerable<KeyValuePair<string, int>> MakeStats(IEnumerable<string> sourceData,
            IWordFrequencyAnalyzer statsMaker,
            int amountOfWords)
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
        
        
        
        public Bitmap GetImage => visualisator.DrawWorldCloud(wordCloud);
    }
}