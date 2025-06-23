using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ustagram.Domain.Model;

namespace Ustagram.Application.Abstractions
{
    public interface IElasticSerachService
    {
        Task<bool> AddElasticUserAsync(User user);
        Task<List<ElasticUser>> GetUsersAsync(string query);
        Task DeleteElasticUserAsync(User user);
    }
}
