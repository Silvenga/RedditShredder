using FluentAssertions;
using RedditShredder.Generation;
using Xunit;

namespace RedditShredder.Tests.Generation
{
    public class ParagraphGeneratorTests
    {
        private static readonly WordList WordList = new();

        [Fact]
        public void When_generating_content_then_GetParagraph_should_return_stable_results()
        {
            const string seed = "hello";
            var generator = new ParagraphGenerator(WordList);

            // Act
            var comment = generator.GetParagraph(seed);

            // Assert
            comment.Should().Be("Chimonanthu.");
        }

        [Fact]
        public void When_generating_content_then_GetParagraphs_should_return_stable_results()
        {
            const string seed = "hello";
            var generator = new ParagraphGenerator(WordList);

            // Act
            var comment = generator.GetParagraphs(seed);

            // Assert
            comment.Should().Be("Jethr squinted!");
        }
    }
}
