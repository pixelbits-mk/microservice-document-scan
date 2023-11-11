using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Exceptions
{
    public class SecretsProviderException : ApplicationException
    {
        public SecretsProviderException(string message, Exception? inner = null) : base(message, inner)
        {

        }
    }
}
