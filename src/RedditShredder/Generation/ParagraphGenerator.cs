using System;
using System.Text;

namespace RedditShredder.Generation
{
    public class ParagraphGenerator
    {
        private readonly WordList _wordList;

        public ParagraphGenerator(WordList wordList)
        {
            _wordList = wordList;
        }

        public string GetParagraph(string seed)
        {
            var random = GetRandom(seed);
            var builder = new StringBuilder();

            AppendParagraphs(random, builder, 1);

            return builder.ToString();
        }

        public string GetParagraphs(string seed)
        {
            var random = GetRandom(seed);
            var builder = new StringBuilder();

            var paragraphsCount = random.Next(1, 12);
            AppendParagraphs(random, builder, paragraphsCount);

            return builder.ToString();
        }

        private void AppendParagraphs(Random random, StringBuilder builder, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    builder.Append("\r\n")
                           .Append("\r\n");
                }

                AppendParagraph(random, builder);
            }
        }

        private void AppendParagraph(Random random, StringBuilder builder)
        {
            var sentencesCount = random.Next(1, 6);
            for (var i = 0; i < sentencesCount; i++)
            {
                if (i != 0)
                {
                    builder.Append(' ');
                }

                AppendSentence(random, builder);
            }
        }

        private void AppendSentence(Random random, StringBuilder builder)
        {
            var sentenceLength = random.Next(1, 8);

            for (var i = 0; i < sentenceLength; i++)
            {
                var word = GetWord(random);

                if (i == 0)
                {
                    var firstChar = word[0];
                    builder.Append(char.ToUpperInvariant(firstChar))
                           .Append(word, 1, word.Length - 2);
                }
                else
                {
                    builder.Append(' ')
                           .Append(word);
                }
            }

            builder.Append(GetPunctuation(random));
        }

        private string GetWord(Random random)
        {
            var wordIndex = random.Next(0, _wordList.Words.Count - 1);
            return _wordList.Words[wordIndex];
        }

        private char GetPunctuation(Random random)
        {
            var next = random.Next(0, 100);
            return next switch
            {
                < 50 => '.',
                < 80 => '?',
                _ => '!'
            };
        }

        private Random GetRandom(string seed)
        {
            var hashCode = GetStableHashCode(seed);
            return new Random(hashCode);
        }

        private static int GetStableHashCode(string str)
        {
            // https://stackoverflow.com/a/36845864/2001966

            unchecked
            {
                var hash1 = 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
