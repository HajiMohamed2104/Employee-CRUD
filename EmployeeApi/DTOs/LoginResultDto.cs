namespace EmployeeApi.DTOs
{
    public class LoginResultDto
    {
        public required string Token { get; set; }
        public required string Role { get; set; }
    }
}