using System;
using System.Diagnostics;
using System.Linq;
using Windows.Security.Credentials;
using DrinkOBand.Core.Infrastructure;

namespace DrinkOBand.Common.Infrastructure
{
    public class UwpPasswordVault : IPasswordVault
    {
        public PasswordVaultCredential GetCredentialByResource(string resource)
        {
            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential =
                    vault.FindAllByResource(resource)
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                // When there is no matching resource an error occurs, which we ignore.
                Debug.WriteLine(ex.ToString());
            }

            if (credential != null)
            {
                credential.RetrievePassword();
                return new PasswordVaultCredential(resource, credential.UserName, credential.Password);
            }
            return null;

        }

        public void Add(string resource, string userName, string password)
        {
            PasswordVault vault = new PasswordVault();
            vault.Add(new PasswordCredential(resource, userName, password));
        }

        public void RemoveResource(string resource)
        {
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential =
                    vault.FindAllByResource(resource)
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                // When there is no matching resource an error occurs, which we ignore.
                Debug.WriteLine(ex.ToString());
            }

            if (credential != null)
            {
                vault.Remove(credential);
            }
        }
    }


}