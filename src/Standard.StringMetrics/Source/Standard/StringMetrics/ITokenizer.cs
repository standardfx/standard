using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Interface class for the tokenizer.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Tokenize a word into a collection of tokens.
        /// </summary>
        /// <param name="word">The string to tokenize.</param>
        /// <returns>A collection of tokens from <paramref name="word"/>.</returns>
        Collection<string> Tokenize(string word);

        /// <summary>
        /// Tokenize a word into a collection of token sets.
        /// </summary>
        /// <param name="word">The string to tokenize.</param>
        /// <returns>A collection of token sets from <paramref name="word"/>.</returns>
        Collection<string> TokenizeToSet(string word);

        /// <summary>
        /// The delimiter to use for converting a string into tokens.
        /// </summary>
        string Delimiters { get; }

        /// <summary>
        /// A handler for the stop word.
        /// </summary>
        ITermHandler StopWordHandler { get; set; }
    }
}
