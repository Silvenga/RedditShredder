using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace RedditShredder.Generation
{
    public class WordList
    {
        private readonly Lazy<IReadOnlyList<string>> _lazy = new(GetWordList, LazyThreadSafetyMode.ExecutionAndPublication);

        public IReadOnlyList<string> Words => _lazy.Value;

        private static IReadOnlyList<string> GetWordList()
        {
            var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var wordStream = typeof(WordList).Assembly.GetManifestResourceStream("RedditShredder.Assets.words.txt")
                                   ?? throw new ArgumentException("Word list is required.");
            using var reader = new StreamReader(wordStream);
            while (reader.ReadLine() is { } word)
            {
                if (!word.Contains('.', StringComparison.OrdinalIgnoreCase)
                    && !word.Contains('\'', StringComparison.OrdinalIgnoreCase)
                    && !word.Contains('&', StringComparison.OrdinalIgnoreCase)
                    && !word.Contains('-', StringComparison.OrdinalIgnoreCase)
                    && !word.Contains(',', StringComparison.OrdinalIgnoreCase)
                    && !word.Contains(' ', StringComparison.OrdinalIgnoreCase))
                {
                    words.Add(word.ToLower(CultureInfo.InvariantCulture));
                }
            }

            var wordsArray = words.ToArray();
            Array.Sort(wordsArray, StringComparer.Ordinal);

            return wordsArray;
        }
    }
}
