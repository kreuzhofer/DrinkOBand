using System.Threading.Tasks;

namespace DrinkOBand.Core.Infrastructure
{
    public interface IMobileServicesAuthentication
    {
        Task<bool> AuthenticateAsync();
        void RemoveCredentials();
    }
}