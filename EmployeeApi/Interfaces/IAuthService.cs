using EmployeeApi.DTOs;
using EmployeeApi.Models;

namespace EmployeeApi.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(User user, string password);
        Task<LoginResultDto?> LoginAsync(string username, string password);
    }
}