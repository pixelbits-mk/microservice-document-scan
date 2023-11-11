using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Interfaces
{
    public interface ISecretsProvider
    {
        T GetSecret<T>(string vaultName, string secretName);
    }
}
