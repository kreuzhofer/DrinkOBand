namespace DrinkOBand.Core.Infrastructure
{
    public interface IPasswordVault
    {
        PasswordVaultCredential GetCredentialByResource(string resource);
        void Add(string resource, string userName, string password);
        void RemoveResource(string resource);
    }
}