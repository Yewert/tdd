using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Fclp;

namespace TagsCloudVisualization
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string savePath = null;
            string statsSource = null;
            var amountOfWords = 250;
            var minWordLength = 3;
            var minFontSize = 10.0f;
            var maxFontSize = 150.0f;
            var debug = false;
            var lowerCase = false;
            var parser = new FluentCommandLineParser();
            parser.Setup<string>('S', "source").Callback(source => statsSource = source).Required();
            parser.Setup<string>('s', "save").Callback(save => savePath = save).Required();
            parser.Setup<int>('a', "amount").Callback(amount => amountOfWords = amount);
            parser.Setup<int>('L', "Length").Callback(length => minWordLength = length);
            parser.Setup<int>('m', "minfont").Callback(min => minFontSize = min);
            parser.Setup<int>('M', "maxfont").Callback(max => maxFontSize = max);
            parser.Setup<bool>('d', "debug").Callback(d => debug = d);
            parser.Setup<bool>('l', "lower").Callback(l => lowerCase = l);
            parser.SetupHelp("h", "help", "?").Callback(x => Console.WriteLine(x)).UseForEmptyArgs();
            parser.Parse(args);
            if (savePath is null || statsSource is null)
            {
                Console.WriteLine("save and source are required");
                return;
            }

            var lines = File.ReadLines(statsSource);
            var statsMaker = new WordFrequencyAnalyzer(minWordLength, lowerCase);
            var layouter = new CircularCloudLayouter(new Point(0, 0));
            var cloudMaker = new CloudMaker(minFontSize, maxFontSize);
            var visualisator = new WordCloudVisualisator(debug);
            
            
            var stats = statsMaker.MakeStatisitcs(lines);
            var significantStats = stats
                .OrderByDescending(kvp => kvp.Value)
                .Take(amountOfWords)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            var wordCloud =
                cloudMaker.MakeWordCloudFromStats(significantStats, layouter)
                    .AsParallel()
                    .Select(kvp =>
                        new KeyValuePair<string, (Rectangle rectangle, Font font)>(kvp.Key, 
                            (new Rectangle(
                                CoordinatesTransformer.TransformRelativeToAbsoluteBounded(
                                    kvp.Value.rectangle.Location, layouter.Center, layouter.LeftBound, layouter.UpperBound),
                                kvp.Value.rectangle.Size),
                            kvp.Value.font)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            var image = visualisator.DrawWorldCloud(wordCloud);
            
            image.Save(savePath);
        }
    }
}