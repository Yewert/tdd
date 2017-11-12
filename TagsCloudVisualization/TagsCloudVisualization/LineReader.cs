using System.Collections.Generic;
using System.IO;

namespace TagsCloudVisualization
{
    public static class LineReader
    {
        public static IEnumerable<string> ReadLinesFromFile(string source)
        {
            using (var file = new StreamReader(source))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}