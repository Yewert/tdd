using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    public class FrequencyAnalyzer
    {
        private readonly int maxAmountOfWords;
        private readonly bool lowerCase;
        private readonly Regex wordPattern;

        public FrequencyAnalyzer(int maxAmountOfWords, int minimalWordLength, bool lowerCase = true)
        {
            if(maxAmountOfWords < 1 || minimalWordLength < 1)
                throw new ArgumentException();
            this.maxAmountOfWords = maxAmountOfWords;
            this.lowerCase = lowerCase;
            var pattern = $@"[a-zа-я][a-zа-я-]{{{minimalWordLength - 1},}}";
            wordPattern = new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public Dictionary<string, int> MakeStatisitcs(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException();
            var stats = new Dictionary<string, int>();
            using (var file = new StreamReader(fileName))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var words = wordPattern.Matches(line);
                    foreach (Match match in words)
                    {
                        var word = lowerCase
                            ? match.Value.ToLowerInvariant()
                            : match.Value.ToUpperInvariant();
                        if (!stats.ContainsKey(word))
                            stats[word] = 0;
                        stats[word]++;
                    }
                }
            }
            return stats
                .OrderByDescending(kvp => kvp.Value)
                .Take(maxAmountOfWords)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}