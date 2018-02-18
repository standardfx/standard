using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Standard.Diagnostics.Core;

namespace Standard.Diagnostics
{
	// devnote: Replace all on sublime!!!
	// 
	//   (^(\s+)public static void)
	// $2\[DebuggerStepThrough\]$1    

	public static partial class Assert
	{
		// Empty and NotEmpty

		/// <summary>
		/// Verifies that a collection is empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is not empty.</exception>
		public static void Empty<T>(IEnumerable<T> collection)
			=> Empty(collection, null, null, null);

		/// <summary>
		/// Verifies that a collection is empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="paramName">The parameter name to be associated with the exception.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is not empty.</exception>
		public static void Empty<T>(IEnumerable<T> collection, string paramName)
			=> Empty(collection, paramName, null, null);

		/// <summary>
		/// Verifies that a collection is not empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is empty.</exception>
		public static void NotEmpty<T>(IEnumerable<T> collection)
			=> NotEmpty(collection, null, null, null);

		/// <summary>
		/// Verifies that a collection is not empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="paramName">The parameter name to be associated with the exception.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is empty.</exception>
		public static void NotEmpty<T>(IEnumerable<T> collection, string paramName)
			=> NotEmpty(collection, paramName, null, null);

		/// <summary>
		/// Verifies that a collection is empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="paramName">The parameter name to be associated with the exception.</param>
		/// <param name="message">Message to be shown if the collection is empty.</param>
		/// <param name="args">Additional format arguments to the <paramref name="message"/> parameter.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is not empty.</exception>
		public static void Empty<T>(IEnumerable<T> collection, string paramName, string message, params object[] args)
			=> EmptyInternal(collection, true, paramName, message, args);

		/// <summary>
		/// Verifies that a collection is not empty.
		/// </summary>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="paramName">The parameter name to be associated with the exception.</param>
		/// <param name="message">Message to be shown if the collection is empty.</param>
		/// <param name="args">Additional format arguments to the <paramref name="message"/> parameter.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection is empty.</exception>
		public static void NotEmpty<T>(IEnumerable<T> collection, string paramName, string message, params object[] args)
			=> EmptyInternal(collection, false, paramName, message, args);

		[DebuggerStepThrough]
		private static void EmptyInternal<T>(IEnumerable<T> collection, bool requireEmpty, string paramName, string message, params object[] args)
		{
			NotNull(collection, paramName, message, args);

			if (collection.Any() == requireEmpty)
			{
				if (message == null)
					message = RS.Err_CollectionIsEmpty;
				else if (args != null)
					message = string.Format(message, args);

				throw new ArgumentException(message, paramName);
			}
		}


		// All

		/// <summary>
		/// Verifies that all items in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains at least one item that does not satisfy the <paramref name="predicate"/> specified.</exception>
		public static void All<T>(IEnumerable<T> collection, Predicate<T> predicate)
			=> All(collection, predicate, null, null, null);

		/// <summary>
		/// Verifies that all items in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains at least one item that does not satisfy the <paramref name="predicate"/> specified.</exception>
		public static void All<T>(IEnumerable<T> collection, Predicate<T> predicate, string paramName)
			=> All(collection, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that all items in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains at least one item that does not satisfy the <paramref name="predicate"/> specified.</exception>
		[DebuggerStepThrough]
		public static void All<T>(IEnumerable<T> collection, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			NotNull(collection, paramName, message, args);
			NotNull(predicate, paramName, message, args);

			if (!collection.All(i => predicate(i)))
			{
				if (message == null)
					message = RS.Err_CollectionPredicateFailure;
				else if (args != null)
					message = string.Format(message, args);

				if (string.IsNullOrEmpty(paramName))
					throw new ArgumentException(message);
				else
					throw new ArgumentException(message, paramName);
			}
		}


		// Any

		/// <summary>
		/// Verifies that at least one item in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain any item that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void Any<T>(IEnumerable<T> collection, Predicate<T> predicate)
			=> Any(collection, predicate, null, null, null);

		/// <summary>
		/// Verifies that at least one item in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain any item that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void Any<T>(IEnumerable<T> collection, Predicate<T> predicate, string paramName)
			=> Any(collection, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that at least one item in the collection satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain any item that satisfies the <paramref name="predicate"/> specified.</exception>
		[DebuggerStepThrough]
		public static void Any<T>(IEnumerable<T> collection, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			NotNull(collection, paramName, message, args);
			NotNull(predicate, paramName, message, args);

			if (!collection.Any(i => predicate(i)))
			{
				if (message == null)
					message = RS.Err_CollectionPredicateFailureAny;
				else if (args != null)
					message = string.Format(message, args);

				if (string.IsNullOrEmpty(paramName))
					throw new ArgumentException(message);
				else
					throw new ArgumentException(message, paramName);
			}
		}


		// Count (exact)

		/// <summary>
		/// Verifies that a collection contains exactly the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="length">The number of items that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain the number of items specified by <paramref name="length"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void Count<T>(IEnumerable<T> collection, int length, Predicate<T> predicate)
			=> Count(collection, length, predicate, null, null, null);

		/// <summary>
		/// Verifies that a collection contains exactly the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="length">The number of items that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain the number of items specified by <paramref name="length"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void Count<T>(IEnumerable<T> collection, int length, Predicate<T> predicate, string paramName)
			=> Count(collection, length, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that a collection contains exactly the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="length">The number of items that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection does not contain the number of items specified by <paramref name="length"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void Count<T>(IEnumerable<T> collection, int length, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length), length, RS.Err_ExpectPositiveInteger);

			if (message == null)
				message = string.Format(RS.Err_CollectionLengthMismatch, length, "{#count}");
			else if (args != null)
				message = string.Format(message, args);

			CountInternal(collection, predicate, length, -1, 0, paramName, message);
		}

		// Count (range)

		/// <summary>
		/// Verifies that a collection contains the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The number of items that satisfies the <paramref name="predicate"/> specified is outside the given range.</exception>
		public static void Count<T>(IEnumerable<T> collection, int min, int max, Predicate<T> predicate)
			=> Count(collection, min, max, predicate, null, null, null);

		/// <summary>
		/// Verifies that a collection contains exactly the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The number of items that satisfies the <paramref name="predicate"/> specified is outside the given range.</exception>
		public static void Count<T>(IEnumerable<T> collection, int min, int max, Predicate<T> predicate, string paramName)
			=> Count(collection, min, max, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that a collection contains exactly the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The number of items that satisfies the <paramref name="predicate"/> specified is outside the given range.</exception>
		public static void Count<T>(IEnumerable<T> collection, int min, int max, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException(nameof(max), max, string.Format(RS.Err_MinGtMax, min, max));

			if (min < 0)
				throw new ArgumentOutOfRangeException(nameof(min), min, RS.Err_ExpectPositiveInteger);

			if (max < 0)
				throw new ArgumentOutOfRangeException(nameof(max), max, RS.Err_ExpectPositiveInteger);

			if (message == null)
				message = string.Format(RS.Err_CollectionLengthRangeMismatch, min, max, "{#count}");
			else if (args != null)
				message = string.Format(message, args);

			CountInternal(collection, predicate, min, max, 3, paramName, message);
		}

		// Count (min)

		/// <summary>
		/// Verifies that a collection contains at least the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains less than the number of items specified by <paramref name="min"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MinCount<T>(IEnumerable<T> collection, int min, Predicate<T> predicate)
			=> MinCount(collection, min, predicate, null, null, null);

		/// <summary>
		/// Verifies that a collection contains at least the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains less than the number of items specified by <paramref name="min"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MinCount<T>(IEnumerable<T> collection, int min, Predicate<T> predicate, string paramName)
			=> MinCount(collection, min, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that a collection contains at least the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="min">The minimum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains less than the number of items specified by <paramref name="min"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MinCount<T>(IEnumerable<T> collection, int min, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			if (min < 0)
				throw new ArgumentOutOfRangeException(nameof(min), min, RS.Err_ExpectPositiveInteger);

			if (message == null)
				message = string.Format(RS.Err_CollectionRequireLength, min, "{#count}");
			else if (args != null)
				message = string.Format(message, args);

			CountInternal(collection, predicate, min, -1, 1, paramName, message);
		}

		// Count (max)

		/// <summary>
		/// Verifies that a collection contains at most the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains more than the number of items specified by <paramref name="max"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MaxCount<T>(IEnumerable<T> collection, int max, Predicate<T> predicate)
			=> MaxCount(collection, max, predicate, null, null, null);

		/// <summary>
		/// Verifies that a collection contains at most the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains more than the number of items specified by <paramref name="max"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MaxCount<T>(IEnumerable<T> collection, int max, Predicate<T> predicate, string paramName)
			=> MaxCount(collection, max, predicate, paramName, null, null);

		/// <summary>
		/// Verifies that a collection contains at most the number of items specified that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of object to be verified.</typeparam>
		/// <param name="collection">The collection being evaluated.</param>
		/// <param name="predicate">The predicate to test each item against.</param>
		/// <param name="max">The maximum number of items (inclusive) that must satisfy the <paramref name="predicate"/> specified.</param>
		/// <param name="paramName">The parameter name to be associate with any exception that is raised.</param>
		/// <param name="message">Message to show if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentNullException">A null collection is supplied.</exception>
		/// <exception cref="ArgumentException">The collection contains more than the number of items specified by <paramref name="max"/> that satisfies the <paramref name="predicate"/> specified.</exception>
		public static void MaxCount<T>(IEnumerable<T> collection, int max, Predicate<T> predicate, string paramName, string message, params object[] args)
		{
			if (max < 0)
				throw new ArgumentOutOfRangeException(nameof(max), max, RS.Err_ExpectPositiveInteger);

			if (message == null)
				message = string.Format(RS.Err_CollectionExceedLength, max, "{#count}");
			else if (args != null)
				message = string.Format(message, args);

			CountInternal(collection, predicate, max, -1, 2, paramName, message);
		}

		// mode: 0 exact, 1 min, 2 max, 3 range
		[DebuggerStepThrough]
		private static void CountInternal<T>(IEnumerable<T> collection, Predicate<T> predicate, int length1, int length2, int mode, string paramName, string message)
		{
			NotNull(collection, paramName, message, null);
			NotNull(predicate, paramName, message, null);

			int count = collection.Count((i) => predicate(i));

			bool result = false;
			if (mode == 0)
				result = count == length1;
			else if (mode == 1)
				result = count >= length1;
			else if (mode == 2)
				result = count <= length1;
			else
				result = (count <= length2) && (count >= length1);

			if (!result)
				throw new ArgumentException(message.Replace("{#count}", count.ToString()), paramName);
		}


		// ContainsKey

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey expectedKey)
			=> ContainsKey(value, expectedKey, null, null, null);

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey[] expectedKey)
			=> ContainsKey(value, expectedKey, null, null, null);

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey expectedKey, string paramName)
			=> ContainsKey(value, expectedKey, paramName, null, null);

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey[] expectedKey, string paramName)
			=> ContainsKey(value, expectedKey, paramName, null, null);

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey expectedKey, string paramName, string message, params object[] args)
			=> ContainsKey(value, new TKey[] { expectedKey }, paramName, message, args);

		/// <summary>
		/// Verifies that the dictionary object contains the key specified.
		/// </summary>
		[DebuggerStepThrough]
		public static void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> value, TKey[] expectedKey, string paramName, string message, params object[] args)
		{
			NotNull(value, paramName, message, args);

			if (expectedKey == null)
				return;

			if (message != null && args != null)
				message = string.Format(message, args);

			foreach (TKey key in expectedKey)
			{
				if (!value.ContainsKey(key))
				{
					// paramName can be null
					if (message != null)
						throw new ArgumentException(message, paramName);
					else
						throw new ArgumentException(string.Format(RS.Err_CollectionsNotContainsKey, key), paramName);
				}
			}
		}
	}
}
