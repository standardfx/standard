using System;
using System.Collections.Generic;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents an input for parsing.
    /// </summary>
    public class Input : IInput
    {
        private readonly string _source;
        private readonly int _position;
        private readonly int _line;
        private readonly int _column;

        /// <summary>
        /// Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        /// <param name="source">The source string being parsed.</param>
        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            _source = source;
            _position = position;
            _line = line;
            _column = column;

            Memos = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the memos used by this <see cref="Input"/> instance.
        /// </summary>
        public IDictionary<object, object> Memos { get; private set; }

        /// <see cref="IInput.Advance()"/>
        public IInput Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException(RS.InputAlreadyAtEndOfSource);

            return new Input(
                _source, 
                _position + 1, 
                Current == '\n' ? _line + 1 : _line, 
                Current == '\n' ? 1 : _column + 1);
        }

        /// <see cref="IInput.Source"/>
        public string Source 
        { 
            get { return _source; } 
        }

        /// <see cref="IInput.Current"/>
        public char Current 
        { 
            get { return _source[_position]; } 
        }

        /// <see cref="IInput.AtEnd"/>
        public bool AtEnd 
        { 
            get { return _position == _source.Length; } 
        }

        /// <summary>
        /// Gets the current position within <see cref="Source"/>. This is the zero-based index position within the source string.
        /// </summary>
        public int Position
        { 
            get { return _position; } 
        }

        /// <see cref="IInput.Line"/>
        public int Line 
        { 
            get { return _line; } 
        }

        /// <see cref="IInput.Column"/>
        public int Column 
        { 
            get { return _column; } 
        }

        /// <see cref="Object.ToString()"/>
        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }

        /// <summary>
        /// Serves as a hash function for <see cref="Input"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Input" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ _position;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Input"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// `true` if the specified object is equal to the current <see cref="Input"/>; otherwise, `false`.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IInput);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Input" /> is equal to another <see cref="IInput"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// `true` if the specified object is equal to the current <see cref="Input"/>; otherwise, `false`.
        /// </returns>
        public bool Equals(IInput other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            
            return string.Equals(_source, other.Source) && _position == other.Position;
        }

        /// <summary>
        /// Determines whether two specified <see cref="Input"/> have the same value.
        /// </summary>
        /// <param name="left">The first <see cref="Input" /> to compare.</param>
        /// <param name="right">The second <see cref="Input" /> to compare.</param>
        /// <returns>
        /// `true` if both <see cref="Input"/> are equal; otherwise, `false`.
        /// </returns>
        public static bool operator ==(Input left, Input right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="Input"/> do not have the same value.
        /// </summary>
        /// <param name="left">The first <see cref="Input" /> to compare.</param>
        /// <param name="right">The second <see cref="Input" /> to compare.</param>
        /// <returns>
        /// `true` if both <see cref="Input"/> are not equal; otherwise, `false`.
        /// </returns>
        public static bool operator !=(Input left, Input right)
        {
            return !Equals(left, right);
        }
    }
}
