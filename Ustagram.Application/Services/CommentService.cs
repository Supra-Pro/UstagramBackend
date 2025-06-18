using Microsoft.EntityFrameworkCore;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;
using Ustagram.Domain.Model;
using Ustagram.Infrastructure.Persistance;

namespace Ustagram.Application.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;

    public CommentService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateComment(CommentDTO commentDto, Guid userId)
    {
        var comment = new Comment
        {
            Content = commentDto.Content,
            Date = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm"),
            UserId = userId,
            PostId = commentDto.PostId
        };

        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
        return comment.Id;
    }

    public async Task<string> UpdateComment(Guid commentId, CommentDTO commentDto, Guid userId)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null) return "Comment not found";

        if (comment.UserId != userId) return "Unauthorized";

        comment.Content = commentDto.Content;
        comment.Date = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm");
        await _db.SaveChangesAsync();
        return "Comment updated";
    }

    public async Task<string> DeleteComment(Guid commentId, Guid userId)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null) return "Comment not found";

        if (comment.UserId != userId) return "Unauthorized";

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return "Comment deleted";
    }

    public async Task<CommentResponseDTO> GetCommentById(Guid commentId)
    {
        var comment = await _db.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null) return null;

        return new CommentResponseDTO
        {
            Id = comment.Id,
            Content = comment.Content,
            Date = comment.Date,
            UserId = comment.UserId,
            User = new UserSummaryDTO
            {
                Id = comment.User.Id,
                FullName = comment.User.FullName,
                Username = comment.User.Username,
                PhotoPath = comment.User.PhotoPath
            },
            PostId = comment.PostId
        };
    }

    public async Task<List<CommentResponseDTO>> GetAllComments()
    {
        var comments = await _db.Comments
            .Include(c => c.User)
            .ToListAsync();

        return comments.Select(c => new CommentResponseDTO
        {
            Id = c.Id,
            Content = c.Content,
            Date = c.Date,
            UserId = c.UserId,
            User = new UserSummaryDTO
            {
                Id = c.User.Id,
                FullName = c.User.FullName,
                Username = c.User.Username,
                PhotoPath = c.User.PhotoPath
            },
            PostId = c.PostId
        }).ToList();
    }

    public async Task<List<CommentResponseDTO>> GetCommentsByPost(Guid postId)
    {
        var comments = await _db.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .ToListAsync();

        return comments.Select(c => new CommentResponseDTO
        {
            Id = c.Id,
            Content = c.Content,
            Date = c.Date,
            UserId = c.UserId,
            User = new UserSummaryDTO
            {
                Id = c.User.Id,
                FullName = c.User.FullName,
                Username = c.User.Username,
                PhotoPath = c.User.PhotoPath
            },
            PostId = c.PostId
        }).ToList();
    }
}