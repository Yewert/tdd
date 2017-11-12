﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    public class WordFrequencyAnalyzer
    {
        private readonly bool lowerCase;
        private readonly Regex wordPattern;

        public WordFrequencyAnalyzer(int minimalWordLength, bool lowerCase = true)
        {
            if(minimalWordLength < 1)
                throw new ArgumentException();
            this.lowerCase = lowerCase;
            var pattern = $@"[a-zа-я][a-zа-я-]{{{minimalWordLength - 1},}}";
            wordPattern = new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public Dictionary<string, int> MakeStatisitcs(IEnumerable<string> lines)
        {
            if (lines is null)
                throw new ArgumentNullException();
            var stats = new Dictionary<string, int>();
            foreach (var line in lines)
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
            return stats;
        }
    }
}