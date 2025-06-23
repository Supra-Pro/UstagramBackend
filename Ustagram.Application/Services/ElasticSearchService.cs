using Microsoft.AspNetCore.Http;
using Nest;
using Ustagram.Application.Abstractions;
using Ustagram.Infrastructure.Persistance;
using Ustagram.Domain.Model;
namespace Ustagram.Application.Services
{
    public class ElasticSearchService : IElasticSerachService
    {
        private readonly ApplicationDbContext _context;
        private readonly IElasticClient _elasticClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ElasticSearchService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IElasticClient elasticClient)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _elasticClient = CreateElasticClient();
        }


        private ElasticClient CreateElasticClient()
        {
            var uri = new Uri("http://localhost:9200");       // Elasticsearch manzili
            var defaultIndex = "users";                       // Index nomi

            var settings = new ConnectionSettings(uri)
                .DefaultIndex(defaultIndex)
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromSeconds(10));     // optional: timeout

            return new ElasticClient(settings);
        }


        private async Task<bool> IndexDocumentAsync(ElasticUser elasticProduct)
        {
            if (_elasticClient == null)
            {
                Console.WriteLine("ElasticClient is null");
                return false;
            }

            var indexResponse = await _elasticClient.IndexDocumentAsync(elasticProduct);

            if (!indexResponse.IsValid)
            {
                Console.WriteLine($"Product with id {elasticProduct.Id} indexed in ElasticSearch");
            }

            return indexResponse.IsValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> AddElasticUserAsync(User user)
        {
            if (_elasticClient == null)
            {
                Console.WriteLine("ElasticClient is null");
                return false;
            }

            var elasticProduct = new ElasticUser(user);

            return await IndexDocumentAsync(elasticProduct);
        }


        public async Task DeleteElasticUserAsync(User user)
        {
            if (_elasticClient == null)
            {
                Console.WriteLine("ElasticClient is null");
                return;
            }

            var elasticProduct = new ElasticUser(user);

            await DeleteElasticProduct(elasticProduct);
        }


        private async Task DeleteElasticProduct(ElasticUser elasticProduct)
        {
            var deleteResponse = await _elasticClient.DeleteAsync<ElasticUser>(elasticProduct.Id);
            if (!deleteResponse.IsValid)
            {
                Console.WriteLine($"Product with id {elasticProduct.Id} deleted from ElasticSearch");
            }
        }




        public async Task<List<ElasticUser>> GetUsersAsync(string query)
        {
            if (_elasticClient == null)
            {
                Console.WriteLine("ElasticClient is null");
                return new List<ElasticUser>();
            }

            var searchResponse = await _elasticClient.SearchAsync<ElasticUser>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                    .Fields(f => f
                            .Field(p => p.Username)
                        )
                        .Query(query)
                        .Fuzziness(Fuzziness.Auto)
                    ))
                .Size(1000)
            );

            if (searchResponse.IsValid)
                return searchResponse.Documents.ToList();

            return new List<ElasticUser>();
        }
    }
}
