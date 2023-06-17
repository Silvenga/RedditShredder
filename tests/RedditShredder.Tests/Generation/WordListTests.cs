using FluentAssertions;
using RedditShredder.Generation;
using Xunit;

namespace RedditShredder.Tests.Generation
{
    public class WordListTests
    {
        [Fact]
        public void WordList_should_contain_word_list()
        {
            var wordList = new WordList();

            // Act
            var result = wordList.Words;

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Word_list_should_be_stable()
        {
            var wordList = new WordList();

            // Act
            var result = wordList.Words;

            // Assert
            // ReSharper disable once StringLiteralTypo
            result[4242].Should().Be("adherant");
        }
    }
}
