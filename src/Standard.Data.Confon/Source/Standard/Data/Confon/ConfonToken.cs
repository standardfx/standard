namespace Standard.Data.Confon
{
    /// <summary>
    /// This enumeration defines the different types of tokens found within a Confon string.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// This token type represents a comment.
        /// </summary>
        Comment,

        /// <summary>
        /// This token type represents the key portion of a key-value pair.
        /// </summary>
        Key,

        /// <summary>
        /// This token type represents the value portion of a key-value pair.
        /// </summary>
        LiteralValue,

        /// <summary>
        /// This token type represents the assignment operator, <c>=</c> or <c>:</c> .
        /// </summary>
        Assign,

        /// <summary>
        /// This token type represents the beginning of an object, <c>{</c> .
        /// </summary>
        ObjectStart,

        /// <summary>
        /// This token type represents the end of an object, <c>}</c> .
        /// </summary>
        ObjectEnd,

        /// <summary>
        /// This token type represents a namespace separator, <c>.</c> .
        /// </summary>
        Dot,

        /// <summary>
        /// This token type represents the end of the configuration string.
        /// </summary>
        EOF,

        /// <summary>
        /// This token type represents the beginning of an array, <c>[</c> .
        /// </summary>
        ArrayStart,

        /// <summary>
        /// This token type represents the end of an array, <c>]</c> .
        /// </summary>
        ArrayEnd,

        /// <summary>
        /// This token type represents the separator in an array, <c>,</c> .
        /// </summary>
        Comma,

        /// <summary>
        /// This token type represents a replacement variable, <c>$foo</c> .
        /// </summary>
        Substitute,

        /// <summary>
        /// A include key
        /// </summary>
        Include
    }

    /// <summary>
    /// This class represents a token within a Confon string.
    /// </summary>
    public class ConfonToken
    {
        // for serialization
        private ConfonToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonToken"/> class.
        /// </summary>
        /// <param name="type">The type of token to associate with.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        public ConfonToken(TokenType type, int sourceIndex, int sourceLength) 
            : this(null, type, sourceIndex, sourceLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonToken"/> class.
        /// </summary>
        /// <param name="value">The string literal value to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        public ConfonToken(string value, int sourceIndex, int sourceLength) 
            : this(value, TokenType.LiteralValue, sourceIndex, sourceLength)
        {
        }

        public ConfonToken(string value, TokenType type, int sourceIndex, int sourceLength)
        {
            Type = type;
            Value = value;
            SourceIndex = sourceIndex;
            Length = sourceLength;
        }

        public int SourceIndex 
        { 
            get; 
            private set; 
        }

        public int Length 
        { 
            get;
            private set; 
        }

        /// <summary>
        /// The value associated with this token. If this token is
        /// a <see cref="TokenType.LiteralValue"/>, then this property
        /// holds the string literal.
        /// </summary>
        public string Value 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// The type that represents this token.
        /// </summary>
        public TokenType Type 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Creates a key token with a given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A key token with the given key.</returns>
        public static ConfonToken Key(string key, int sourceIndex, int sourceLength)
        {
            return new ConfonToken(key, TokenType.Key, sourceIndex, sourceLength);
        }

        /// <summary>
        /// Creates a substitution token with a given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A substitution token with the given path.</returns>
        public static ConfonToken Substitution(string path, int sourceIndex, int sourceLength)
        {
            return new ConfonToken(path, TokenType.Substitute, sourceIndex, sourceLength);
        }

        /// <summary>
        /// Creates a string literal token with a given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A string literal token with the given value.</returns>
        public static ConfonToken LiteralValue(string value, int sourceIndex, int sourceLength)
        {
            return new ConfonToken(value, TokenType.LiteralValue, sourceIndex, sourceLength);
        }

        public static ConfonToken Include(string path, int sourceIndex, int sourceLength)
        {
            return new ConfonToken(path, TokenType.Include, sourceIndex, sourceLength);
        }
    }
}
