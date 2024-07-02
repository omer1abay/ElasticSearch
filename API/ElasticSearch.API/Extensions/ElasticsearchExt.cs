using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Nest;

namespace ElasticSearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElasticClient(this IServiceCollection services, IConfiguration configuration)
        {

            #region Elastic.Clients.Elasticsearch Library
            var settingsES = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"]!));

            var userName = configuration.GetSection("Elastic")["Username"]!.ToString();
            var password = configuration.GetSection("Elastic")["Password"]!.ToString();

            settingsES.Authentication(new BasicAuthentication(userName,password));

            var clientES = new ElasticsearchClient(settingsES);

            #endregion


            #region NEST Library
            //Elastic Client sınıfının implementasyonu 
            //Elastic Client için Elastic firmasının önerisi Singleton olarak kullanılmasıdır.

            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("ELastic")["Url"]!));

            var settings = new ConnectionSettings(pool);

            //settings.BasicAuthentication //username password için kullanmazsak default username-password ile bağlanacak

            var client = new ElasticClient(settings); //Elastic Client thread safe'tir birden fazla thread'den bu nesneye erişebiliriz. Ama DbContext sınıfı thread safe değildir.
            #endregion

            services.AddSingleton(clientES);
        }
    }
}
