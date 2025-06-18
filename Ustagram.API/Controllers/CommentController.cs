using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;

namespace Ustagram.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateComment([FromForm] CommentDTO commentDTO)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var commentId = await _commentService.CreateComment(commentDTO, userId);
        return Ok(commentId);
    }

    [HttpPut("{commentId}")]
    public async Task<ActionResult<string>> UpdateComment(Guid commentId, [FromForm] CommentDTO commentDTO)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _commentService.UpdateComment(commentId, commentDTO, userId);

        if (result == "Unauthorized")
            return Unauthorized("You don't own this comment");

        return Ok(result);
    }

    [HttpDelete("{commentId}")]
    public async Task<ActionResult<string>> DeleteComment(Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _commentService.DeleteComment(commentId, userId);

        if (result == "Unauthorized")
            return Unauthorized("You don't own this comment");

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{commentId}")]
    public async Task<ActionResult<CommentResponseDTO>> GetCommentById(Guid commentId)
    {
        var comment = await _commentService.GetCommentById(commentId);
        if (comment == null)
            return NotFound("Comment not found");
        return Ok(comment);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<CommentResponseDTO>>> GetAllComments()
    {
        var comments = await _commentService.GetAllComments();
        return Ok(comments);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<CommentResponseDTO>>> GetCommentsByPost(Guid id)
    {
        var relatedComments = await _commentService.GetCommentsByPost(id);
        return Ok(relatedComments);
    }
}