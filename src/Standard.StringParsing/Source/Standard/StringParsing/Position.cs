using System;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents a position in the input source.
    /// </summary>
    public class Position : IEquatable<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Position" /> class.
        /// </summary>
        /// <param name="pos">The zero-based index position within the input source string being parsed.</param>
        /// <param name="line">The line number.</param>
        /// <param name="column">The column number.</param>
        public Position(int pos, int line, int column)
        {
            Value = pos;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Creates an new <see cref="Position"/> instance from a specified <see cref="IInput"/> object.
        /// </summary>
        /// <param name="input">The current input.</param>
        /// <returns>
        /// A new <see cref="Position"/> instance.
        /// </returns>
        public static Position FromInput(IInput input)
        {
            return new Position(input.Position, input.Line, input.Column);
        }

        /// <summary>
        /// Gets the zero-based index position within the input source string being parsed.
        /// </summary>
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current column number.
        /// </summary>
        public int Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Position" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// `true` if the specified object is equal to the current <see cref="Position"/>; otherwise, `false`.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Position" /> is equal to another <see cref="Position"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// `true` if the specified object is equal to the current <see cref="Position"/>; otherwise, `false`.
        /// </returns>
        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;

            return Value == other.Value &&
                Line == other.Line &&
                Column == other.Column;
        }

        /// <summary>
        /// Determines whether two specified <see cref="Position"/> have the same value.
        /// </summary>
        /// <param name="left">The first <see cref="Position" /> to compare.</param>
        /// <param name="right">The second <see cref="Position" /> to compare.</param>
        /// <returns>
        /// `true` if both <see cref="Position"/> are equal; otherwise, `false`.
        /// </returns>
        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="Position"/> do not have the same value.
        /// </summary>
        /// <param name="left">The first <see cref="Position" /> to compare.</param>
        /// <param name="right">The second <see cref="Position" /> to compare.</param>
        /// <returns>
        /// `true` if both <see cref="Position"/> are not equal; otherwise, `false`.
        /// </returns>
        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Serves as a hash function for <see cref="Position"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Position" />.
        /// </returns>
        public override int GetHashCode()
        {
            int h = 31;
            h = h * 13 + this.Value;
            h = h * 13 + this.Line;
            h = h * 13 + this.Column;
            return h;
        }

        /// <see cref="Object.ToString()"/>
        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", Line, Column);
        }
    }
}
