﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    public class WordFrequencyAnalyzer : IWordFrequencyAnalyzer
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

            return lines.SelectMany(line => wordPattern.Matches(line).Cast<Match>()).Select(m => lowerCase
                        ? m.Value.ToLowerInvariant()
                        : m.Value.ToUpperInvariant())
                    .GroupBy(w => w)
                    .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}