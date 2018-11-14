using System;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents an optional result of a parsing operation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface IOption<out T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is defined.
        /// </summary>
        bool IsDefined { get; }

        /// <summary>
        /// Gets the matched result or a default value.
        /// </summary>
        /// <returns>
        /// The matched result, or the default value of the type if no matches were found.
        /// </returns>
        T GetOrDefault();

        /// <summary>
        /// Gets the matched result.
        /// </summary>
        /// <returns>
        /// The matched result.
        /// </returns>
        T Get();
    }

    /// <summary>
    /// This class contains extension methods for <see cref="IOption{T}"/>.
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        /// Gets the value, or returns a default value if the option instance is empty.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="option">An instance of <see cref="IOption{T}"/>.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="option"/> is empty.</param>
        /// <returns>
        /// The matched result if <paramref name="option"/> is not empty; otherwise, <paramref name="defaultValue"/>.
        /// </returns>
        public static T GetOrElse<T>(this IOption<T> option, T defaultValue)
        {
            if (option == null) 
                throw new ArgumentNullException(nameof(option));
            
            return option.IsEmpty 
                ? defaultValue 
                : option.Get();
        }
    }

    internal abstract class AbstractOption<T> : IOption<T>
    {
        public abstract bool IsEmpty { get; }

        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        public T GetOrDefault()
        {
            return IsEmpty 
                ? default(T) 
                : Get();
        }

        public abstract T Get();
    }

    internal sealed class Some<T> : AbstractOption<T>
    {
        private readonly T _value;

        public Some(T value)
        {
            _value = value;
        }

        public override bool IsEmpty
        {
            get { return false; }
        }

        public override T Get()
        {
            return _value;
        }
    }

    internal sealed class None<T> : AbstractOption<T>
    {
        public override bool IsEmpty
        {
            get { return true; }
        }

        public override T Get()
        {
            throw new InvalidOperationException(RS.CannotGetValueFromNone);
        }
    }
}
