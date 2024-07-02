using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using Microsoft.AspNetCore.Http.Metadata;
//using Nest;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repository
{
    public class ProductRepository
    {

        //private readonly ElasticClient _client; NEST kütüphanesi
        private readonly ElasticsearchClient _client;
        private const string productIndexName = "products11";


        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x=> x.Index(productIndexName).Id(Guid.NewGuid().ToString())); //id'yi kendimiz belirledik.

            if (!response.IsSuccess()) //IsValid Nest kütüphanesindeki karşılığıdır.
                 return null; //fast fail yöntemidir scope içine almıyoruz ve else kullanmıyoruz..

            newProduct.Id = response.Id;

            return newProduct;

        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => 
                                s.Index(productIndexName)
                                    .Query(q => 
                                        q.MatchAll()
                                     )
                                );

            //result tipi ISearchResponse
            //return result.Documents.ToImmutableList(); //documents içinde id'ler yok bu yüzden ilk olarak çektiğimizde null gelir id'ler. ID'ler Hits içindedir.

            //Hits içinden id'yi alıp data'ya basacağız.

            foreach ( var hit in result.Hits ) hit.Source.Id = hit.Id; //source'un id'sine veriyoruz çünkü documents source'tan besleniyor

            return result.Documents.ToImmutableList();


        }

        public async Task<Product?> GetById(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(productIndexName));

            if (!response.IsSuccess()) //IsValid NEST kütüphanesindeki karşılığıdır.
            {
                return null;
            }

            response.Source.Id = response.Id; //id mapping

            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto productUpdateDto)
        {
            //var response = await _client.UpdateAsync<Product, ProductUpdateDto>(productUpdateDto.Id,
            //                                x => x.Index(productIndexName).Doc(productUpdateDto)); NEST kütüphanesi içindir

            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(productIndexName,productUpdateDto.Id, x=> x.Doc(productUpdateDto));

            return response.IsSuccess();

        }

        /// <summary>
        /// Hata yönetimi için bu metot ele alınmıştır. repository'de hatalar ele alınmaz.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id,
                                            x => x.Index(productIndexName));

            return response;

        }

    }
}
