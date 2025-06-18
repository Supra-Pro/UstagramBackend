using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;

namespace Ustagram.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class FavouritesController : ControllerBase
{
    private readonly IFavouritesService _service;

    public FavouritesController(IFavouritesService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateFavourite([FromForm] FavouriteDTO favouriteDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _service.CreateFavourite(favouriteDto, userId);
        return Ok(result);
    }

    [HttpPut("{favouriteId}")]
    public async Task<ActionResult<string>> UpdateFavourite(Guid favouriteId, [FromForm] FavouriteDTO favouriteDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _service.UpdateFavourite(favouriteId, favouriteDto, userId);

        if (result == "Unauthorized")
            return Unauthorized("You don't own this favourite");

        return Ok(result);
    }

    [HttpDelete("{favouriteId}")]
    public async Task<ActionResult<string>> DeleteFavourite(Guid favouriteId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _service.DeleteFavourite(favouriteId, userId);

        if (result == "Unauthorized")
            return Unauthorized("You don't own this favourite");

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{favouriteId}")]
    public async Task<ActionResult<FavouriteResponseDTO>> GetFavouriteById(Guid favouriteId)
    {
        var favourite = await _service.GetFavouriteById(favouriteId);
        if (favourite == null)
            return NotFound("Favourite not found");
        return Ok(favourite);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<FavouriteResponseDTO>>> GetFavourites()
    {
        var favourites = await _service.GetAllFavourites();
        return Ok(favourites);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<FavouriteResponseDTO>>> GetMyFavourites()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Invalid or missing user ID");

        var result = await _service.GetFavouritesByUser(userId);
        return Ok(result);
    }
}