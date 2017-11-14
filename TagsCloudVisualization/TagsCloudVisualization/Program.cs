using System;
using System.Drawing;
using System.IO;
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
            
            var cloudMaker = new CloudMaker(minFontSize, maxFontSize);
            var image = cloudMaker.MakeCloud(
                File.ReadLines(statsSource),
                new WordFrequencyAnalyzer(minWordLength, lowerCase),
                amountOfWords,
                new CircularCloudLayouter(new Point(0, 0), new BasisChanger()),
                new ImageBounder(),
                new WordCloudVisualisator(debug));
            
            image.Save(savePath);
        }
    }
}