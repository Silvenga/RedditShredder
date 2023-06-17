using System.Text;

namespace RedditShredder.Generation
{
    public class ContentGenerator
    {
        private readonly ParagraphGenerator _generator;

        public ContentGenerator(ParagraphGenerator generator)
        {
            _generator = generator;
        }

        public string GenerateCommentForId(string commentId)
        {
            var builder = new StringBuilder();

            builder.Append(_generator.GetParagraph(commentId));
            builder.Append("\r\n");
            builder.Append("\r\n");
            builder.Append("___");
            builder.Append("\r\n");
            builder.Append("\r\n");
            builder.Append("This comment was deleted in response to the choices by Reddit leadership (see https://redd.it/1476fkn). "
                           + "The code that made this automated modification can be found at https://github.com/Silvenga/RedditShredder. "
                           + "You may contact the commenter for the original contents.");

            return builder.ToString();
        }

        public string GeneratePostForId(string commentId)
        {
            var builder = new StringBuilder();

            builder.Append(_generator.GetParagraph(commentId));
            builder.Append("\r\n");
            builder.Append("\r\n");
            builder.Append("___");
            builder.Append("\r\n");
            builder.Append("\r\n");
            builder.Append("This post was deleted in response to the choices by Reddit leadership (see https://redd.it/1476fkn). "
                           + "The code that made this automated modification can be found at https://github.com/Silvenga/RedditShredder. "
                           + "You may contact the poster for the original contents.");

            return builder.ToString();
        }
    }
}
