using System;
using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    internal class TokenizerQGram2 : AbstractTokenizerQGramN
    {
        private const string TokenizerName = "TokenizerQGram2";

        public TokenizerQGram2()
        {
            base.StopWordHandler = new DummyStopTermHandler();
            base.TokenUtility = new TokenizerUtility<string>();
            base.CharacterCombinationIndex = 0;
            base.QGramLength = 2;
        }

        public override Collection<string> Tokenize(string word)
        {
            return base.Tokenize(word, false, base.QGramLength, base.CharacterCombinationIndex);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            return string.Format(
                RS.QgramTokenizerOnHold,
                new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
            );
        }
    }

    internal class TokenizerQGram2Extended : TokenizerQGram2
    {
        private const string TokenizerName = "TokenizerQGram2Extended";

        public override Collection<string> Tokenize(string word)
        {
            return base.Tokenize(word, true, base.QGramLength, base.CharacterCombinationIndex);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            return string.Format(
                RS.QgramTokenizerOnHold,
                new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
            );
        }
    }

    internal class TokenizerQGram3 : AbstractTokenizerQGramN
    {
        private const string TokenizerName = "TokenizerQGram3";

        public TokenizerQGram3()
        {
            base.StopWordHandler = new DummyStopTermHandler();
            base.TokenUtility = new TokenizerUtility<string>();
            base.CharacterCombinationIndex = 0;
            base.QGramLength = 3;
        }

        public override Collection<string> Tokenize(string word)
        {
            return base.Tokenize(word, false, base.QGramLength, base.CharacterCombinationIndex);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            return string.Format(
                RS.QgramTokenizerOnHold,
                new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
            );
        }
    }

    internal class TokenizerQGram3Extended : TokenizerQGram3
    {
        private const string TokenizerName = "TokenizerQGram3Extended";

        public override Collection<string> Tokenize(string word)
        {
            return base.Tokenize(word, true, base.QGramLength, base.CharacterCombinationIndex);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            return string.Format(
                RS.QgramTokenizerOnHold,
                new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
            );
        }
    }
}
