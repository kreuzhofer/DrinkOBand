using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Shared.Infrastructure;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace DrinkOBand.Universal.Infrastructure
{
    public class MobileServicesAuthentication : IMobileServicesAuthentication
    {
        // Define a member variable for storing the signed-in user. 
        private MobileServiceClientManager _clientManager;
        private IPasswordVault _passwordVault;

        public MobileServicesAuthentication(MobileServiceClientManager clientManager, IPasswordVault passwordVault)
        {
            _clientManager = clientManager;
            _passwordVault = passwordVault;
        }

        public async Task<bool> AuthenticateAsync()
        {
            string message;
            MobileServiceUser user;

            PasswordVaultCredential credential = null;

            while (credential == null)
            {
                credential = _passwordVault.GetCredentialByResource(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString());

                if (credential != null)
                {
                    // Create a user from the stored credentials.
                    user = new MobileServiceUser(credential.UserName);
                    user.MobileServiceAuthenticationToken = credential.Password;

                    // Set the user from the stored credentials.
                    _clientManager.MobileService.CurrentUser = user;

                    try
                    {
                        // Calling /.auth/refresh will update the tokens in the token store
                        // and will also return a new mobile authentication token.
                        JObject refreshJson = (JObject)await _clientManager.MobileService.InvokeApiAsync(
                            "/.auth/refresh",
                            HttpMethod.Get,
                            null);

                        string newToken = refreshJson["authenticationToken"].Value<string>();
                        _clientManager.MobileService.CurrentUser.MobileServiceAuthenticationToken
                            = newToken;

                        // get information about me
                        var meJson = await _clientManager.MobileService.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);
                        var entry = meJson.ToObject<TokenEntry[]>();
                    }
                    catch(Exception ex)
	                {
                        Debug.WriteLine(ex.Message);
                        // failed means we have to login again
                    }

                    _passwordVault.Add(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString(),
                                user.UserId, user.MobileServiceAuthenticationToken);

                    try
                    {
                        // Try to return an item now to determine if the cached credential has expired.
                        await _clientManager.MobileService.GetTable<DrinkLogItem>().Take(1).ToListAsync();
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Remove the credential with the expired token.
                            _passwordVault.RemoveResource(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString());
                            credential = null;
                            continue;
                        }
                    }
                }
                else
                {
                    try
                    {
                        // Login with the identity provider.
                        user = await _clientManager.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);

                        // get information about me
                        var meJson = await _clientManager.MobileService.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);

                        // Create and store the user credentials.
                        _passwordVault.Add(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString(),
                                user.UserId, user.MobileServiceAuthenticationToken);
                    }
                    catch (MobileServiceInvalidOperationException)
                    {
                        message = "You must log in. Login Required";
                        return false;
                    }
                }
            }
            return true;
        }

        public void RemoveCredentials()
        {
            _passwordVault.RemoveResource(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString());
        }
    }
}