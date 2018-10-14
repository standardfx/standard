using System;

namespace Standard.StringMetrics
{
    internal class TokenizerSGram2 : TokenizerQGram2
    {
        private const string TokenizerName = "TokenizerSGram2";
         
        public TokenizerSGram2()
        {
            base.CharacterCombinationIndex = 1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            if (base.CharacterCombinationIndex == 0)
                return string.Format(
                    RS.QgramTokenizerOnHold,
                    new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
                );

            return string.Format(
                RS.QgramTokenizerWithCharCombinatorOnHold,
                new object[] { TokenizerName, base.SuppliedWord,  
                    Convert.ToInt32(base.CharacterCombinationIndex), 
                    Convert.ToInt32(base.QGramLength) 
                }
            );
        }
    }

    internal class TokenizerSGram2Extended : TokenizerQGram2Extended
    {
        private const string TokenizerName = "TokenizerSGram2Extended";
         
        public TokenizerSGram2Extended()
        {
            base.CharacterCombinationIndex = 1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            if (base.CharacterCombinationIndex == 0)
                return string.Format(
                    RS.QgramTokenizerOnHold,
                    new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
                );

            return string.Format(
                RS.QgramTokenizerWithCharCombinatorOnHold,
                new object[] { TokenizerName, base.SuppliedWord,  
                    Convert.ToInt32(base.CharacterCombinationIndex), 
                    Convert.ToInt32(base.QGramLength) 
                }
            );
        }
    }

    internal class TokenizerSGram3 : TokenizerQGram3
    {
        private const string TokenizerName = "TokenizerSGram3";
         
        public TokenizerSGram3()
        {
            base.CharacterCombinationIndex = 1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            if (base.CharacterCombinationIndex == 0)
                return string.Format(
                    RS.QgramTokenizerOnHold,
                    new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
                );

            return string.Format(
                RS.QgramTokenizerWithCharCombinatorOnHold,
                new object[] { TokenizerName, base.SuppliedWord,  
                    Convert.ToInt32(base.CharacterCombinationIndex), 
                    Convert.ToInt32(base.QGramLength) 
                }
            );
        }
    }

    internal class TokenizerSGram3Extended : TokenizerQGram3Extended
    {
        private const string TokenizerName = "TokenizerSGram3Extended";

        public TokenizerSGram3Extended()
        {
            base.CharacterCombinationIndex = 1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(base.SuppliedWord))
                return string.Format(RS.TokenizerNotReady, TokenizerName);

            if (base.CharacterCombinationIndex == 0)
                return string.Format(
                    RS.QgramTokenizerOnHold,
                    new object[] { TokenizerName, base.SuppliedWord, Convert.ToInt32(base.QGramLength) }
                );

            return string.Format(
                RS.QgramTokenizerWithCharCombinatorOnHold,
                new object[] { TokenizerName, base.SuppliedWord,  
                    Convert.ToInt32(base.CharacterCombinationIndex), 
                    Convert.ToInt32(base.QGramLength) 
                }
            );
        }
    }
}
