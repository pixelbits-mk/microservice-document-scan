using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Models
{
    public class KeyVaultReference
    {
        public string VaultName { get; set; } = string.Empty;
        public string SecretName { get; set; } = string.Empty;
        public string? SecretVersion { get; set; }

        public string Token { get; set; } = string.Empty;
        public string GetKeyVaultUrl()
        {
            return $"https://{VaultName}.vault.azure.net";
        }

    }
}
