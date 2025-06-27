using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ustagram.Domain.Model
{
    public class UserFollow
    {
        public Guid FollowerId { get; set; }
        public User Follower { get; set; }

        public Guid FollowingId { get; set; }
        public User Following { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
