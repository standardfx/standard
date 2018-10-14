using System.Text;

namespace Standard.StringMetrics
{
    /// <summary>
    /// A dummy stop term handler that does not do anything.
    /// </summary>
    public sealed class DummyStopTermHandler : ITermHandler
    {
        /// <summary>
        /// Add a string to the list of terms.
        /// </summary>
        /// <param name="termToAdd">The string to add.</param>
        /// <remarks>
        /// This method does nothing.
        /// </remarks>
        public void AddWord(string termToAdd)
        {
        }

        /// <summary>
        /// Tests whether a string exists in the list of terms.
        /// </summary>
        /// <param name="termToTest">The string to test.</param>
        /// <returns>This method always returns `false`.</returns>
        public bool IsWord(string termToTest)
        {
            return false;
        }

        /// <summary>
        /// Removes an existing string from the list of terms.
        /// </summary>
        /// <param name="termToRemove">The string to remove.</param>
        /// <remarks>
        /// This method does nothing.
        /// </remarks>
        public void RemoveWord(string termToRemove)
        {
        }

        /// <summary>
        /// The number of terms in the current list.
        /// </summary>
        /// <remarks>
        /// This property always return zero.
        /// </remarks>
        public int NumberOfWords
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns the buffer.
        /// </summary>
        /// <remarks>
        /// This property returns an empty <see cref="StringBuilder"/>.
        /// </remarks>
        public StringBuilder WordsAsBuffer
        {
            get
            {
                return new StringBuilder();
            }
        }
    }
}
