using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.Model;
using Ustagram.Infrastructure.Persistance;

namespace Ustagram.Application.Services
{
    public class UserFollowingService : IUserFollowingService
    {
        private readonly ApplicationDbContext _context;

        public UserFollowingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FollowAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId) return false;

            var alreadyFollowing = await _context.UsersFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (alreadyFollowing) return false;

            var follow = new UserFollow
            {
                FollowerId = followerId,
                FollowingId = followingId
            };

            _context.UsersFollows.Add(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowAsync(Guid followerId, Guid followingId)
        {
            var follow = await _context.UsersFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null) return false;

            _context.UsersFollows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
