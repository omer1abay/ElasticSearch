using ElasticSearch.API.Models;

namespace ElasticSearch.API.DTOs
{
    public record ProductFeatureDto(int Width, int Height, EColor Color)
    {
    }
}
