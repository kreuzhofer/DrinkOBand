namespace DrinkOBand.Core.Infrastructure
{
    public class PasswordVaultCredential
    {
        public PasswordVaultCredential(string resource, string userName, string password)
        {
            this.Resource = resource;
            this.UserName = userName;
            this.Password = password;
        }

        public string Resource { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}