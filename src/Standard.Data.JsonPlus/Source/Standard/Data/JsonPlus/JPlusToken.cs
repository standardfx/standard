namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This enumeration defines the different types of tokens that the <see cref="ConfonJPlusParser"/> can support.
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
    /// A token within a Confon string.
    /// </summary>
    public class JPlusToken
    {
        // for serialization
        private JPlusToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusToken"/> class.
        /// </summary>
        /// <param name="type">The type of token to associate with.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        public JPlusToken(TokenType type, int sourceIndex, int sourceLength) 
            : this(null, type, sourceIndex, sourceLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusToken"/> class.
        /// </summary>
        /// <param name="value">The string literal value to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        public JPlusToken(string value, int sourceIndex, int sourceLength) 
            : this(value, TokenType.LiteralValue, sourceIndex, sourceLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusToken"/> class.
        /// </summary>
        /// <param name="value">The string literal value to associate with this token.</param>
        /// <param name="type">The type of token to associate with.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        public JPlusToken(string value, TokenType type, int sourceIndex, int sourceLength)
        {
            Type = type;
            Value = value;
            SourceIndex = sourceIndex;
            Length = sourceLength;
        }

        /// <summary>
        /// Gets the index position of the token within the raw text source.
        /// </summary>
        public int SourceIndex 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the length of the token.
        /// </summary>
        public int Length 
        { 
            get;
            private set; 
        }

        /// <summary>
        /// The value associated with this token. If this token is a <see cref="TokenType.LiteralValue"/>, this property
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
        /// Creates a key token with a given name.
        /// </summary>
        /// <param name="key">The key to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A key token with the given key.</returns>
        public static JPlusToken Key(string key, int sourceIndex, int sourceLength)
        {
            return new JPlusToken(key, TokenType.Key, sourceIndex, sourceLength);
        }

        /// <summary>
        /// Creates a substitution token with a given path.
        /// </summary>
        /// <param name="path">The path to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A substitution token with the given path.</returns>
        public static JPlusToken Substitution(string path, int sourceIndex, int sourceLength)
        {
            return new JPlusToken(path, TokenType.Substitute, sourceIndex, sourceLength);
        }

        /// <summary>
        /// Creates a string literal token with a given value.
        /// </summary>
        /// <param name="value">The value to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>A string literal token with the given value.</returns>
        public static JPlusToken LiteralValue(string value, int sourceIndex, int sourceLength)
        {
            return new JPlusToken(value, TokenType.LiteralValue, sourceIndex, sourceLength);
        }

        /// <summary>
        /// Creates an <see cref="TokenType.Include"/> token with a given path.
        /// </summary>
        /// <param name="path">The path to associate with this token.</param>
        /// <param name="sourceIndex">Index position of raw text source</param>
        /// <param name="sourceLength">Length of raw text source</param>
        /// <returns>An <see cref="TokenType.Include"/> token with the specified path.</returns>
        public static JPlusToken Include(string path, int sourceIndex, int sourceLength)
        {
            return new JPlusToken(path, TokenType.Include, sourceIndex, sourceLength);
        }
    }
}
