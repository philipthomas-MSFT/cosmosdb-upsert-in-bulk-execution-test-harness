
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

public class HarnessUtility
{
    public static async Task<string> GetCosmosDBConnectionStringAsync(string connectionName) {
        var tokenProvider = new AzureServiceTokenProvider();
        var client = new KeyVaultClient(
            authenticationCallback: new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback)
        );
        var secret = await client.GetSecretAsync(
            vaultBaseUrl: System.Environment.GetEnvironmentVariable("HARNESS_KEY_VAULT_URL"),
            secretName: connectionName).ConfigureAwait(continueOnCapturedContext: false);

        return secret.Value;
    }
}