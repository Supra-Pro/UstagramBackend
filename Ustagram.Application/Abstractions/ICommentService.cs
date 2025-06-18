using Ustagram.Domain.DTOs;
using Ustagram.Domain.Model;

namespace Ustagram.Application.Abstractions;

public interface ICommentService
{
    Task<Guid> CreateComment(CommentDTO commentDto, Guid userId);
    Task<string> UpdateComment(Guid commentId, CommentDTO commentDto, Guid userId);
    Task<string> DeleteComment(Guid commentId, Guid userId);
    Task<CommentResponseDTO> GetCommentById(Guid commentId);
    Task<List<CommentResponseDTO>> GetAllComments();
    Task<List<CommentResponseDTO>> GetCommentsByPost(Guid postId);
}