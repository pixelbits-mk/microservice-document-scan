using Common.Util.Exceptions;
using Common.Util.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Util.Utilities
{

    public class KeyVaultTokenParser
    {
        static readonly string REGEX_KEYVAULT_REFERENCES = @"@Microsoft\.KeyVault\([^)]+\)";
        static readonly string REGEX_SECRET_URI = @"@Microsoft\.KeyVault\(SecretUri=https:\/\/(?<keyVaultName>[^.]+)\.vault\.azure\.net\/secrets\/(?<secretName>[^\/]+)(\/(?<secretVersion>[^;]+))?\)";
        static readonly string REGEX_VAULTNAME = @"VaultName=(?<keyVaultName>[^;)]+)";
        static readonly string REGEX_SECRET_NAME = @"SecretName=(?<secretName>[^;)]+)";
        static readonly string REGEX_SECRET_VERSION = @"SecretVersion=(?<secretVersion>[^;)]+)";

        public static KeyValuePair<string, string>[] ParseUniqueKeyVaults(string contents)
        {
            var keyVaultReference = ParseKeyVaultReferences(contents);
            return keyVaultReference.Select(x => new KeyValuePair<string, string>(x.VaultName, x.GetKeyVaultUrl() ?? string.Empty)).Distinct().ToArray();
        }
        public static KeyVaultReference ParseKeyVaultReferenceVersion1(string secretUri)
        {
            Match secretUriMatch = Regex.Match(secretUri, REGEX_SECRET_URI);

            if (secretUriMatch.Success)
            {
                string keyVaultName = secretUriMatch.Groups["keyVaultName"].Value;
                string secretName = secretUriMatch.Groups["secretName"].Value;
                string secretVersion = secretUriMatch.Groups["secretVersion"].Value;

                return new KeyVaultReference
                {
                    VaultName = keyVaultName,
                    SecretName = secretName,
                    SecretVersion = secretVersion,
                    Token = secretUriMatch.Value
                };
            }
            else
            {
                throw new ArgumentException("Invalid SecretUri format");
            }

        }

        public static KeyVaultReference ParseKeyVaultReferenceVersion2(string value)
        {
            Match vaultNameMatch = Regex.Match(value, REGEX_VAULTNAME);
            Match secretNameMatch = Regex.Match(value, REGEX_SECRET_NAME);
            Match secretVersionMatch = Regex.Match(value, REGEX_SECRET_VERSION);

            if (vaultNameMatch.Success && secretNameMatch.Success)
            {
                string keyVaultName = vaultNameMatch.Groups["keyVaultName"].Value;
                string secretName = secretNameMatch.Groups["secretName"].Value;
                string? secretVersion = secretVersionMatch.Success ? secretVersionMatch.Groups["secretVersion"].Value : null;

                return new KeyVaultReference
                {
                    VaultName = keyVaultName,
                    SecretName = secretName,
                    SecretVersion = secretVersion,
                    Token = value
                };
            } else
            {
                throw new SecretsProviderException("Invalid (VaultName, SecretName, SecretVersion) format");
            }
        }

        public static KeyVaultReference[] ParseKeyVaultReferences(string input)
        {
            List<KeyVaultReference> references = new List<KeyVaultReference>();

            MatchCollection matches = Regex.Matches(input, REGEX_KEYVAULT_REFERENCES);

            foreach (Match match in matches)
            {
                string value = match.Value;

                // Check if it's a SecretUri
                Match secretUriMatch = Regex.Match(value, REGEX_SECRET_URI);
                if (secretUriMatch.Success)
                {
                    references.Add(ParseKeyVaultReferenceVersion1(value));
                }
                else
                {
                    references.Add(ParseKeyVaultReferenceVersion2(value));
                }
            }

            return references.ToArray();
        }
    }
}
