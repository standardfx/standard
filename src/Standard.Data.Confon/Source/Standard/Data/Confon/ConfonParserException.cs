using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standard.Data.Confon
{
    public class ConfonParserException : Exception
    {
        public ConfonParserException(string message) 
            : base(message)
        {
        }

        public ConfonParserException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
