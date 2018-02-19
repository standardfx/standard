using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standard.Data.Confon
{
    public class ConfonTokenizerException : Exception
    {
        public ConfonTokenizerException(string message) 
            : base(message)
        {
        }
    }
}
