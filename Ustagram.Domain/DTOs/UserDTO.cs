using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ustagram.Domain.DTOs;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UserDTO
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [StringLength(100)]
    public string Location { get; set; }

    public IFormFile? Photo { get; set; }

    [StringLength(10)]
    public string Dob { get; set; }

    [StringLength(50)]
    public string Status { get; set; }

    [StringLength(50)]
    public string MasterType { get; set; }

    [StringLength(500)]
    public string Bio { get; set; }

    [Range(0, 100)]
    public int Experience { get; set; }

    [Url]
    public string TelegramUrl { get; set; }

    [Url]
    public string InstagramUrl { get; set; }
}



public class UserSummaryDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PhotoPath { get; set; }
}



public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Phone { get; set; }
    public string Location { get; set; }
    public string PhotoPath { get; set; }
    public string Dob { get; set; }
    public string Status { get; set; }
    public string MasterType { get; set; }
    public string Bio { get; set; }
    public int Experience { get; set; }
    public string TelegramUrl { get; set; }
    public string InstagramUrl { get; set; }
}


public class PublicUserDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PhotoPath { get; set; }
    public string Bio { get; set; }
}