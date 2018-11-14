namespace Standard.StringParsing
{
    /// <summary>
    /// Represents a type that has a source <see cref="Position"/>.
    /// </summary>
    /// <typeparam name="T">The type of the matched result.</typeparam>
    public interface IPositionAware<out T>
    {
        /// <summary>
        /// Set the start <see cref="Position"/> and the matched length.
        /// </summary>
        /// <param name="startPos">The start position</param>
        /// <param name="length">The matched length.</param>
        /// <returns>
        /// The matched result.
        /// </returns>
        T SetPosition(Position startPos, int length);
    }
}
