using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ustagram.Domain.DTOs
{
    public class FollowDto
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}
