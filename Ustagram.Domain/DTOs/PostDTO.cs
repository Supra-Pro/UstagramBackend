using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ustagram.Domain.DTOs;

public class PostDTO
{
    [Required(ErrorMessage = "PostType is required")]
    [StringLength(50, ErrorMessage = "PostType cannot exceed 50 characters")]
    public string PostType { get; set; }

    [Required(ErrorMessage = "Text is required")]
    [StringLength(100, ErrorMessage = "Text cannot exceed 100 characters")]
    public string Text { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Price cannot be negative")]
    public int Price { get; set; }

    // public IFormFile? Attachment { get; set; }

    [StringLength(255, ErrorMessage = "PhotoPath cannot exceed 255 characters")]
    public string PhotoPath { get; set; } = string.Empty; 
}