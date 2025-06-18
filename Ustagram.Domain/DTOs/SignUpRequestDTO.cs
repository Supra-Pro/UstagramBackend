using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ustagram.Domain.DTOs;


public class SignUpRequestDTO
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

    [StringLength(255)]
    public string PhotoPath { get; set; }

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

    public IFormFile? Photo { get; set; }
}