using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace ElasticSearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElasticClient(this IServiceCollection services, IConfiguration configuration)
        {
            //Elastic Client sınıfının implementasyonu 
            //Elastic Client için Elastic firmasının önerisi Singleton olarak kullanılmasıdır.

            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("ELastic")["Url"]!));

            var settings = new ConnectionSettings(pool);

            //settings.BasicAuthentication //username password için kullanmazsak default username-password ile bağlanacak

            var client = new ElasticClient(settings); //Elastic Client thread safe'tir birden fazla thread'den bu nesneye erişebiliriz. Ama DbContext sınıfı thread safe değildir.

            services.AddSingleton(client);
        }
    }
}
