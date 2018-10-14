using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// A whitespace tokenizer.
    /// </summary>
    public sealed class TokenizerWhitespace : ITokenizer
    {
        private string delimiters = "\r\n\t \x00a0";
        private ITermHandler stopWordHandler = new DummyStopTermHandler();
        private TokenizerUtility<string> tokenUtility = new TokenizerUtility<string>();

        /// <summary>
        /// Tokenize a word into a collection of tokens, using whitespace as the delimiter.
        /// </summary>
        /// <param name="word">The string to tokenize.</param>
        /// <returns>A collection of tokens from <paramref name="word"/>.</returns>
        public Collection<string> Tokenize(string word)
        {
            Collection<string> collection = new Collection<string>();
            if (word != null)
            {
                int length;
                for (int i = 0; i < word.Length; i = length)
                {
                    char c = word[i];
                    if (char.IsWhiteSpace(c))
                        i++;

                    length = word.Length;
                    for (int j = 0; j < this.delimiters.Length; j++)
                    {
                        int index = word.IndexOf(this.delimiters[j], i);
                        if ((index < length) && (index != -1))
                            length = index;
                    }
                    string termToTest = word.Substring(i, length - i);
                    if (!this.stopWordHandler.IsWord(termToTest))
                        collection.Add(termToTest);
                }
            }
            return collection;
        }

        /// <summary>
        /// Tokenize a word into a collection of token sets, using whitespace as the delimiter.
        /// </summary>
        /// <param name="word">The string to tokenize.</param>
        /// <returns>A collection of token sets from <paramref name="word"/>.</returns>
        public Collection<string> TokenizeToSet(string word)
        {
            if (word != null)
                return this.tokenUtility.CreateSet(this.Tokenize(word));

            return null;
        }

        /// <summary>
        /// The delimiter to use for converting a string into tokens.
        /// </summary>
        public string Delimiters
        {
            get { return this.delimiters; }
        }

        /// <summary>
        /// Returns a dummp word handler.
        /// </summary>
        public ITermHandler StopWordHandler
        {
            get { return this.stopWordHandler; }
            set { this.stopWordHandler = value; }
        }
    }
}
