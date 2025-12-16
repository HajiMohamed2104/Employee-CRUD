using EmployeeApi.Models;
namespace EmployeeApi.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(User user, string password);
        Task<string?> LoginAsync(string username, string password);
    }
}
