using Common.Util.Interfaces;
using Common.Util.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace Common.Util.Services
{

    public class ConfigurationService : IConfigurationService
    {

        private readonly ILogger<ConfigurationService> _logger;
        private readonly ISecretsProvider _secretsProvider;
        private IConfiguration _configuration;

        public ConfigurationService(ILogger<ConfigurationService> logger, IConfiguration configuration, ISecretsProvider secretsProvider)
        {
            _secretsProvider = secretsProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public T GetSetting<T>(string settingName)
        {
            var setting = _configuration[settingName];
            if (setting == null)
            {
                _logger.LogWarning($"Setting {settingName} not found.");
                throw new Exception($"Setting {settingName} not found.");
            }
            setting = ReplaceSecrets(setting);

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), setting);
            }
            else
            {
                return (T)Convert.ChangeType(setting, typeof(T));
            }
        }

        public IConfigurationService WithConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }

        private string ReplaceSecrets(string setting)
        {
            var keyVaultReferences = KeyVaultTokenParser.ParseKeyVaultReferences(setting);
            foreach (var keyVaultReference in keyVaultReferences)
            {
                var secret = _secretsProvider.GetSecret<string>(keyVaultReference.VaultName, keyVaultReference.SecretName);
                setting = setting.Replace(keyVaultReference.Token, secret);
            }


            //Match match = Regex.Match(setting, regex);
            //if (match.Success)
            //{
            //    string keyVaultName = match.Groups["keyVaultName"].Value;
            //    string secretName = match.Groups["secretName"].Value;

            //    _logger.LogInformation($"Found secret reference for {secretName} in KeyVault {keyVaultName}.");

            //    var secret = _secretsProvider.GetSecret<string>(keyVaultName, secretName);
            //    // replace match with secret
            //    setting = setting.Replace(match.Value, secret);

            //    _logger.LogInformation($"Replaced secret reference with actual secret value.");
            //}
            return setting;
        }

    }
}
