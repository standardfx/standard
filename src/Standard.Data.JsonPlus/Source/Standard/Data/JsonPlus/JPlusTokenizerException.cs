using System;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// Represents an error when parsing Json+ source text into tokens.
    /// </summary>
    public class JPlusTokenizerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusParserException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public JPlusTokenizerException(string message) 
            : base(message)
        {
        }
    }
}
