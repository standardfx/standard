using System;
using System.Linq;
using System.Collections.Generic;
using System.Security;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class SecureStringTest
    {
        [Fact]
        public void CanGetUnderlyingValue()
        {
            string mypassword = "mysecret";
            SecureString s1 = new SecureString();

            try
            {
                for (int i = 0; i < mypassword.Length; i++)
                {
                    s1.AppendChar(mypassword[i]);
                }

                Assert.Equal(mypassword, s1.GetValue());
            }
            finally
            {
                s1.Dispose();
            }
        }

        [Fact]
        public void CanCompareEqual() 
        {
            string mypassword = "mysecret";

            SecureString s1 = new SecureString();
            SecureString s2 = new SecureString();

            try
            {
                for (int i = 0; i < mypassword.Length; i++)
                {
                    s1.AppendChar(mypassword[i]);
                    s2.AppendChar(mypassword[i]);
                }

                // mysecret -vs- mysecret
                Assert.True(s1.ValueEquals(s2));

                // mysecret -vs- mysecretS
                s2.AppendChar('S');
                Assert.False(s1.ValueEquals(s2));

                // mysecretS -vs- mysecretS
                s1.AppendChar('S');
                Assert.True(s1.ValueEquals(s2));

                // mysecretS! -vs- mysecretS?
                s1.AppendChar('!');
                s1.AppendChar('?');
                Assert.False(s1.ValueEquals(s2));
            }
            finally
            {
                s1.Dispose();
                s2.Dispose();
            }
        }
    }
}
