using System;
namespace AuthWeb.Models
{
    public record struct CreateUserDto(
    int UserId,
    string Username,
    string Email,
    string Phone,
    string imagePath,
    string password
    );
}

