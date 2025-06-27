using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;

namespace Ustagram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IUserFollowingController : ControllerBase
    {
        private readonly IUserFollowingService _userFollowingService;

        public IUserFollowingController(IUserFollowingService userFollowingService)
        {
            _userFollowingService = userFollowingService;
        }

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromQuery] FollowDto dto)
        {
            var result = await _userFollowingService.FollowAsync(dto.FollowerId, dto.FollowingId);
            return result ? Ok("Followed") : BadRequest("Already following or invalid");
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromQuery] FollowDto dto)
        {
            var result = await _userFollowingService.UnfollowAsync(dto.FollowerId, dto.FollowingId);
            return result ? Ok("Unfollowed") : NotFound("Not following");
        }   
    }
}
