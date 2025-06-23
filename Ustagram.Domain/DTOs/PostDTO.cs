using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ustagram.Domain.DTOs;

public class PostDTO
{
    [Required]
    public string PostType { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int Price { get; set; }


    public IFormFile Photo { get; set; }
}