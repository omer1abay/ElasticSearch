using ElasticSearch.API.Models;

namespace ElasticSearch.API.DTOs
{
    public record ProductUpdateDto(string Id, string Name, decimal Price, int Stock, ProductFeatureDto ProductFeature) //record'lar syntax tarafında bir yeniliktir intermediate language tarafına gelince class'lara çevrilirler.
    {
        
        
    }
}
