using System.Text;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Interface class for handling terms in a candidate string.
    /// </summary>
    public interface ITermHandler
    {
        /// <summary>
        /// Add a string to the list of terms.
        /// </summary>
        /// <param name="termToAdd">The string to add.</param>
        void AddWord(string termToAdd);

        /// <summary>
        /// Tests whether a string exists in the list of terms.
        /// </summary>
        /// <param name="termToTest">The string to test.</param>
        /// <returns>`true` if <paramref name="termToTest"/> exists in the list of terms. Otherwise, `false`.</returns>
        bool IsWord(string termToTest);

        /// <summary>
        /// Removes an existing string from the list of terms.
        /// </summary>
        /// <param name="termToRemove">The string to remove.</param>
        void RemoveWord(string termToRemove);

        /// <summary>
        /// The number of terms in the current list.
        /// </summary>
        int NumberOfWords { get; }

        /// <summary>
        /// Returns the buffer.
        /// </summary>
        StringBuilder WordsAsBuffer { get; }
    }
}