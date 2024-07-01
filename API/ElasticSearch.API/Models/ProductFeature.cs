namespace ElasticSearch.API.Models
{
    public class ProductFeature
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public EColor Color { get; set; }
    }

    public enum EColor
    {
        Red = 1, 
        Blue = 2, 
        Green = 3
    }

}
