using Azure.Core;
using Azure.Identity;
using Common.Util.Exceptions;
using Common.Util.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common.Util.Services
{
    public class SecretsProvider : ISecretsProvider
    {

        private readonly ILogger<SecretsProvider> _logger;
        private readonly Dictionary<string, IConfiguration> _keyVaults = new Dictionary<string, IConfiguration>();


        public SecretsProvider(ILogger<SecretsProvider> logger)
        {
            _logger = logger;
        }

        public void AddKeyVault(string keyVaultName, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(keyVaultName))
            {
                throw new SecretsProviderException("KeyVault name is required.");
            }
            if (configuration == null)
            {
                throw new SecretsProviderException("KeyVault configuration is required.");
            }
            // store in dictionary
            _keyVaults.Add(keyVaultName.ToLower(), configuration);
        }

        public T GetSecret<T>(string keyVaultName, string secretName)
        {
            try
            {
                if (string.IsNullOrEmpty(keyVaultName))
                {
                    _logger.LogError("KeyVault name is required.");
                    throw new Exception("KeyVault name is required.");
                }
                if (string.IsNullOrEmpty(secretName))
                {
                    _logger.LogError("Secret name is required.");
                    throw new Exception("Secret name is required.");
                }
                var keyVaultNameLower = keyVaultName.ToLower();
                // check if keyvault exists
                if (!_keyVaults.ContainsKey(keyVaultNameLower))
                {
                    LoadKeyVault(this, keyVaultNameLower);
                }
                var keyVault = _keyVaults[keyVaultNameLower];

                // check if secret exists
                if (!keyVault.GetChildren().Any(x => x.Key == secretName))
                {
                    _logger.LogError($"Secret {secretName} not found in KeyVault {keyVaultName}.");
                    throw new SecretsProviderException($"Secret {secretName} not found in KeyVault {keyVaultName}.");
                }

                // get the secret
                var secret = keyVault[secretName] ?? string.Empty;

                _logger.LogInformation($"Retrieved secret '{secretName}' from KeyVault '{keyVaultName}'.");

                // return the secret
                return (T)Convert.ChangeType(secret, typeof(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving secret from KeyVault.");
                throw;
            }

        }

        public static void RegisterKeyVault(SecretsProvider secretsProvider, string keyVaultName, string keyVaultUri, TokenCredential credential)
        {

            if (string.IsNullOrEmpty(keyVaultName))
            {
                throw new SecretsProviderException("KeyVault name is required.");
            }
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new SecretsProviderException("KeyVault URI is required.");
            }

            try
            {

                IConfiguration secretConfiguration = new ConfigurationBuilder()
                   .AddAzureKeyVault(new Uri(keyVaultUri), credential)
                   .Build();

                secretsProvider.AddKeyVault(keyVaultName, secretConfiguration);
            }
            catch (Exception ex)
            {
                throw new SecretsProviderException($"Error registering KeyVault {keyVaultName} with URI {keyVaultUri}.", ex);
            }

        }

        public void LoadKeyVault(SecretsProvider secretsProvider, string keyVaultName)
        {
            try
            {
                var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net";
                RegisterKeyVault(secretsProvider, keyVaultName, keyVaultUrl, GetCredential());
            }
            catch (Exception ex)
            {
                throw new SecretsProviderException($"Error loading KeyVault {keyVaultName}.", ex);
            }
        }

        private TokenCredential GetCredential()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            _logger.LogInformation($"Getting credential for environment: {environment}");

            if (string.IsNullOrEmpty(environment))
            {
                _logger.LogError("ASPNETCORE_ENVIRONMENT environment variable not set.");
                throw new Exception("ASPNETCORE_ENVIRONMENT environment variable not set.");
            }
            TokenCredential credential = environment == "Development" ? new AzureCliCredential() :
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeEnvironmentCredential = true,
                    ExcludeInteractiveBrowserCredential = true,
                    ExcludeAzurePowerShellCredential = true,
                    ExcludeSharedTokenCacheCredential = true,
                    ExcludeVisualStudioCodeCredential = true,
                    ExcludeVisualStudioCredential = true,
                    ExcludeAzureCliCredential = true,
                    ExcludeManagedIdentityCredential = false
                });

            return credential;
        }
    }
}
