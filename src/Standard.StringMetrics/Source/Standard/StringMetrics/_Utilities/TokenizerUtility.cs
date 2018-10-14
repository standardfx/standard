using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Utility class for manipulating tokens.
    /// </summary>
    /// <typeparam name="T">The type of token objects.</typeparam>
    public class TokenizerUtility<T>
    {
        private Collection<T> allTokens;
        private int firstSetTokenCount;
        private int firstTokenCount;
        private int secondSetTokenCount;
        private int secondTokenCount;
        private Collection<T> tokenSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerUtility{T}"/> class.
        /// </summary>
        public TokenizerUtility()
        {
            this.allTokens = new Collection<T>();
            this.tokenSet = new Collection<T>();
        }

        private void AddTokens(Collection<T> tokenList)
        {
            foreach (T local in tokenList)
            {
                this.allTokens.Add(local);
            }
        }

        private void AddUniqueTokens(Collection<T> tokenList)
        {
            foreach (T local in tokenList)
            {
                if (!this.tokenSet.Contains(local))
                    this.tokenSet.Add(local);
            }
        }

        private int CalculateUniqueTokensCount(Collection<T> tokenList)
        {
            Collection<T> collection = new Collection<T>();
            foreach (T local in tokenList)
            {
                if (!collection.Contains(local))
                    collection.Add(local);
            }
            return collection.Count;
        }

        /// <summary>
        /// Returns the number of common token set terms.
        /// </summary>
        /// <returns>The number of common token set terms.</returns>
        public int CommonSetTerms()
        {
            return ((this.FirstSetTokenCount + this.SecondSetTokenCount) - this.tokenSet.Count);
        }

        /// <summary>
        /// Returns the number of common token terms.
        /// </summary>
        /// <returns>The number of common token terms.</returns>
        public int CommonTerms()
        {
            return ((this.FirstTokenCount + this.SecondTokenCount) - this.allTokens.Count);
        }

        /// <summary>
        /// Returns a token collection from the tokens specified.
        /// </summary>
        /// <param name="firstTokens">The first collection of tokens to merge.</param>
        /// <param name="secondTokens">The second collection of tokens to merge.</param>
        /// <returns>A token collection derived from <paramref name="firstTokens"/> and <paramref name="secondTokens"/>.</returns>
        public Collection<T> CreateMergedList(Collection<T> firstTokens, Collection<T> secondTokens)
        {
            this.allTokens.Clear();
            this.firstTokenCount = firstTokens.Count;
            this.secondTokenCount = secondTokens.Count;
            this.MergeLists(firstTokens);
            this.MergeLists(secondTokens);
            return this.allTokens;
        }

        /// <summary>
        /// Returns a token set from the tokens specified.
        /// </summary>
        /// <param name="firstTokens">The first collection of tokens to merge.</param>
        /// <param name="secondTokens">The second collection of tokens to merge.</param>
        /// <returns>A token set derived from <paramref name="firstTokens"/> and <paramref name="secondTokens"/>.</returns>
        public Collection<T> CreateMergedSet(Collection<T> firstTokens, Collection<T> secondTokens)
        {
            this.tokenSet.Clear();
            this.firstSetTokenCount = this.CalculateUniqueTokensCount(firstTokens);
            this.secondSetTokenCount = this.CalculateUniqueTokensCount(secondTokens);
            this.MergeIntoSet(firstTokens);
            this.MergeIntoSet(secondTokens);
            return this.tokenSet;
        }

        /// <summary>
        /// Returns a token set from a collection of tokens.
        /// </summary>
        /// <param name="tokenList">A collections of tokens.</param>
        /// <returns>A token set derived from <paramref name="tokenList"/>.</returns>
        public Collection<T> CreateSet(Collection<T> tokenList)
        {
            this.tokenSet.Clear();
            this.AddUniqueTokens(tokenList);
            this.firstTokenCount = this.tokenSet.Count;
            this.secondTokenCount = 0;
            return this.tokenSet;
        }

        /// <summary>
        /// Add unique tokens to the list.
        /// </summary>
        /// <param name="firstTokens">Tokens to add to the list. Only unique items will be added.</param>
        public void MergeIntoSet(Collection<T> firstTokens)
        {
            this.AddUniqueTokens(firstTokens);
        }

        /// <summary>
        /// Add tokens to the list.
        /// </summary>
        /// <param name="firstTokens">Tokens to add to the list.</param>
        public void MergeLists(Collection<T> firstTokens)
        {
            this.AddTokens(firstTokens);
        }

        /// <summary>
        /// Returns the number of items in the first token set.
        /// </summary>
        public int FirstSetTokenCount
        {
            get
            {
                return this.firstSetTokenCount;
            }
        }

        /// <summary>
        /// Returns the number of items in the first token.
        /// </summary>
        public int FirstTokenCount
        {
            get
            {
                return this.firstTokenCount;
            }
        }

        /// <summary>
        /// Returns all tokens.
        /// </summary>
        public Collection<T> MergedTokens
        {
            get
            {
                return this.allTokens;
            }
        }

        /// <summary>
        /// Returns the number of items in the second token set.
        /// </summary>
        public int SecondSetTokenCount
        {
            get
            {
                return this.secondSetTokenCount;
            }
        }

        /// <summary>
        /// Returns the number of items in the second token.
        /// </summary>
        public int SecondTokenCount
        {
            get
            {
                return this.secondTokenCount;
            }
        }

        /// <summary>
        /// Returns the token set.
        /// </summary>
        public Collection<T> TokenSet
        {
            get
            {
                return this.tokenSet;
            }
        }
    }
}
