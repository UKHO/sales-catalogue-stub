using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace UKHO.SalesCatalogueStub.Api.IntegrationTests.Helpers
{
    public static class AzureKeyVaultConfigConfigurationRoot
    {
        private static IConfigurationRoot _instance;

        public static IConfigurationRoot Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var azureAppConfConnectionString = Environment.GetEnvironmentVariable("SCS_AZURE_APP_CONFIGURATION_CONNECTION_STRING");
                if (string.IsNullOrEmpty(azureAppConfConnectionString)) throw new ApplicationException("Missing environment variable: SCS_AZURE_APP_CONFIGURATION_CONNECTION_STRING");

                var keyVaultAddress = Environment.GetEnvironmentVariable("SCS_KEY_VAULT_ADDRESS");
                if (string.IsNullOrEmpty(keyVaultAddress)) throw new ApplicationException("Missing environment variable: SCS_KEY_VAULT_ADDRESS");

                var tokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient =
                    new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

                _instance = new ConfigurationBuilder().AddAzureAppConfiguration(azureAppConfConnectionString)
                                                      .AddAzureKeyVault(keyVaultAddress, keyVaultClient, new DefaultKeyVaultSecretManager())
                                                      .Build();

                return _instance;
            }
        }
    }
}