using ElasticSearch.API.Models;

namespace ElasticSearch.API.DTOs
{
    //record c# 9.0 ile geldi immutable'dır yani üretildikten sonra değiştirilemez functional programming için önemlidir
    public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto ProductFeature) //record'lar syntax tarafında bir yeniliktir intermediate language tarafına gelince class'lara çevrilirler.
    {
        //yukarıdaki tanımlama record'a özel bir tanımlama yöntemidir.

        //dönüştürme işlemini burada yapacağız yani mapping işlemi
        //mapping işlemi, ilgili kodu ilgili sınıfa yaklaştırdık  yani bağımlılığı arttırdık.
        public Product CreateProduct()
        {
            return new Product
            {
                Name = Name,
                Price = Price,
                Stock = Stock,
                Feature = new ProductFeature() { Width = ProductFeature.Width, Height = ProductFeature.Height ,Color = (EColor) int.Parse (ProductFeature.Color) },
            };
        }
    }
}
