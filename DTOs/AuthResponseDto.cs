// DTOs/AuthResponseDto.cs
namespace EventTicketingApp.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public AuthResponseDto(string token, string name, string email)
        {
            Token = token;
            Name = name;
            Email = email;
        }
    }
}