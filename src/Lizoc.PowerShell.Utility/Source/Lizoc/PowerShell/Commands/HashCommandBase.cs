using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text;
using System.Security.Cryptography;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    public class HashCommandBase : PSCmdlet
    {
        private string _algorithm = HashAlgorithmNames.SHA256;

        /// <summary>
        /// Algorithm parameter
        /// The hash algorithm name: 
        /// - "SHA1", "SHA256", "SHA384", "SHA512", "MD5"
        /// - "MACTripleDES", "RIPEMD160" (not available on NETSTANDARD)
        /// </summary>
        [Parameter(Position = 1)]
        [ValidateSet(
            HashAlgorithmNames.SHA1,
            HashAlgorithmNames.SHA256,
            HashAlgorithmNames.SHA384,
            HashAlgorithmNames.SHA512,
#if NETSTANDARD || NETSTANDARD2
            HashAlgorithmNames.MD5
#else
            HashAlgorithmNames.MD5,
            HashAlgorithmNames.MACTripleDES,
            HashAlgorithmNames.RIPEMD160
#endif
        )]
        public string Algorithm
        {
            get
            {
                return HashAlgorithmNames.GetName(_algorithm);
            }
            set
            {
                // A hash algorithm name is case sensitive
                // and always must be in upper case
                _algorithm = value.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Hash algorithm is used
        /// </summary>
        protected HashAlgorithm Hasher;

		/// <summary>
		/// Hash algorithm names
		/// </summary>
		internal static class HashAlgorithmNames
		{
			public const string MD5 = "MD5";
			public const string SHA1 = "SHA1";
			public const string SHA256 = "SHA256";
			public const string SHA384 = "SHA384";
			public const string SHA512 = "SHA512";
#if !NETSTANDARD && !NETSTANDARD2
			public const string MACTripleDES = "MACTripleDES";
			public const string RIPEMD160 = "RIPEMD160";
#endif

			public static string GetName(string name)
			{
				if (name.ToUpperInvariant() == HashAlgorithmNames.MD5.ToUpperInvariant())
					return HashAlgorithmNames.MD5;
				else if (name.ToUpperInvariant() == HashAlgorithmNames.SHA1.ToUpperInvariant())
					return HashAlgorithmNames.SHA1;
				else if (name.ToUpperInvariant() == HashAlgorithmNames.SHA256.ToUpperInvariant())
					return HashAlgorithmNames.SHA256;
				else if (name.ToUpperInvariant() == HashAlgorithmNames.SHA384.ToUpperInvariant())
					return HashAlgorithmNames.SHA384;
				else if (name.ToUpperInvariant() == HashAlgorithmNames.SHA512.ToUpperInvariant())
					return HashAlgorithmNames.SHA512;
#if !NETSTANDARD && !NETSTANDARD2
				else if (name.ToUpperInvariant() == HashAlgorithmNames.MACTripleDES.ToUpperInvariant())
					return HashAlgorithmNames.MACTripleDES;
				else if (name.ToUpperInvariant() == HashAlgorithmNames.RIPEMD160.ToUpperInvariant())
					return HashAlgorithmNames.RIPEMD160;
#endif
				else
					throw new NotSupportedException();
			}
		}

        /// <summary>
        /// Init a hash algorithm
        /// </summary>
        protected void InitHasher(string algo)
        {
            try
            {
                switch (algo)
                {
                    case HashAlgorithmNames.SHA1:
                        Hasher = SHA1.Create();
                        break;

                    case HashAlgorithmNames.SHA256:
                        Hasher = SHA256.Create();
                        break;

                    case HashAlgorithmNames.SHA384:
                        Hasher = SHA384.Create();
                        break;

                    case HashAlgorithmNames.SHA512:
                        Hasher = SHA512.Create();
                        break;

                    case HashAlgorithmNames.MD5:
                        Hasher = MD5.Create();
                        break;

#if !NETSTANDARD && !NETSTANDARD2
                    case HashAlgorithmNames.MACTripleDES:
                        Hasher = new MACTripleDES();
                        break;

                    case HashAlgorithmNames.RIPEMD160:
                        Hasher = RIPEMD160.Create();
                        break;
#endif
                }
            }
            catch
            {
                // This shouldn't happen
                throw new NotSupportedException();
            }
        }
    }
}
