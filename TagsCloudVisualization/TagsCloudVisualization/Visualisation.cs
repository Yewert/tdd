using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    public static class Visualisation
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No agrs, no honey!");
                return 1;
            }
            if (args.Length != 2) return 0;
            var savePath = args[0];
            var statsSource = args[1];
            MakeWordClod(MakeStatisitcs(statsSource), savePath);
            return 0;
        }

        private const float MaxFontSize = 100.0f;
        private const float MinFontSize = 30.0f;

        private static void MakeWordClod(Dictionary<string, int> stats, string savePath)
        {
            
            var center = new Point(0, 0);
            var layouter = new CircularCloudLayouter(center);
            var wordsInCloud = new Dictionary<string, (Rectangle rectangle, Font font)>();


            var significantStats = stats.ToList();
            significantStats.Sort((p1, p2) => -1 * p1.Value.CompareTo(p2.Value));
            significantStats = significantStats.Take(100).ToList();
            var maxWeight = significantStats[0].Value;
            var minWeight = significantStats[significantStats.Count - 1].Value;
            
            foreach (var keyValuePair in significantStats)
            {
                var fontSize = Math.Max(MaxFontSize * (keyValuePair.Value - minWeight) / (maxWeight - minWeight), MinFontSize);
                var font = new Font(FontFamily.GenericMonospace, fontSize);
                var rectangle = layouter.PutNextRectangle(new Size((int)Math.Round(fontSize) * keyValuePair.Key.Length, font.Height));
                wordsInCloud[keyValuePair.Key] = (rectangle, font);
            }
            
            var image = new Bitmap(layouter.Width + 20, layouter.Height + 20);
            using (var graphics = Graphics.FromImage(image))
            {
                foreach (var kvp in wordsInCloud)
                {
                    graphics.DrawString(kvp.Key, kvp.Value.font, Brushes.Black,
                        new PointF(kvp.Value.rectangle.X + center.X - layouter.LeftBound + 10,
                        kvp.Value.rectangle.Y + center.Y - layouter.UpperBound + 10));
                    graphics.DrawRectangle(Pens.Blue, new Rectangle(
                        new Point(kvp.Value.rectangle.X + center.X - layouter.LeftBound + 10,
                            kvp.Value.rectangle.Y + center.Y - layouter.UpperBound + 10),
                        kvp.Value.rectangle.Size));
                }
            }
            image.Save(savePath);
        }

        private static readonly Regex WordPattern = new Regex(@"[a-zа-я][a-zа-я]{2,}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        
        private static Dictionary<string, int> MakeStatisitcs(string fileName)
        {
            var stats = new Dictionary<string, int>();
            using (var file = new StreamReader(fileName))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                    var words = WordPattern.Matches(line);
                    foreach (Match match in words)
                    {
                        var word = match.Value.ToUpperInvariant();
                        if (!stats.ContainsKey(word))
                            stats[word] = 0;
                        stats[word]++;
                    }
                }
            }
            return stats;
        }
    }
}