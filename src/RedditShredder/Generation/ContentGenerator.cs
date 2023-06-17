using System.Text;

namespace RedditShredder.Generation
{
    public class ContentGenerator
    {
        private readonly ParagraphGenerator _generator;
        private readonly Options _options;

        public ContentGenerator(ParagraphGenerator generator, Options options)
        {
            _generator = generator;
            _options = options;
        }

        public string GenerateCommentForId(string commentId)
        {
            var builder = new StringBuilder();

            builder.Append(_generator.GetParagraph(commentId));
            AppendFooter(builder);

            return builder.ToString();
        }

        public string GeneratePostForId(string commentId)
        {
            var builder = new StringBuilder();

            builder.Append(_generator.GetParagraph(commentId));
            AppendFooter(builder);

            return builder.ToString();
        }

        private void AppendFooter(StringBuilder builder)
        {
            if (_options.Footer != null)
            {
                builder.Append("\r\n");
                builder.Append("\r\n");
                builder.Append("___");
                builder.Append("\r\n");
                builder.Append("\r\n");
                builder.Append(_options.Footer);
            }
        }
    }
}
