using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ustagram.Application.Abstractions
{
    public interface IUserFollowingService
    {
        Task<bool> FollowAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowAsync(Guid followerId, Guid followingId);
    }
}
