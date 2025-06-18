namespace Ustagram.Domain.DTOs;

public class CommentDTO
{
    public string Content { get; set; }
    public Guid PostId { get; set; }
}

public class CommentResponseDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public Guid UserId { get; set; }
    public UserSummaryDTO User { get; set; }
    public Guid PostId { get; set; }
}