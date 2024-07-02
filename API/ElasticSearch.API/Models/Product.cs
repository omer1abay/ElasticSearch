using ElasticSearch.API.DTOs;
using Nest;

namespace ElasticSearch.API.Models
{
    public class Product
    {


        //[PropertyName("_id")] //Elastic.Clients kütüphanesi ile artık belirtmemize gerek yoktur. //ES'de id değeri farklı tutulduğu için bu şekilde belirtmemiz gerekiyor id değeri metadata'da tutulur bunu belirtmezsek düz bir property'i olarak algılanır. _id ES'deki isimlendirmedir kafamızdan belirlemedik.
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public ProductFeature? Feature { get; set; } //Nosql db'lerde genellikle ilişkiler pek olmaz normalde tek bir index'te tutulur. NoSql db'lerde ilişkiler sanal ilişki olarak kullanılır yani her durum manuel yapılır elimizle
        //ÖRN : EfCore ilişkili tablolarda veri silme işlemlerinde tüm verileri tablolardan temizlerken nosql db'lerde öyle bir özellik yoktur.
    
        

        public ProductDto CreateDto()
        {
            if (Feature == null) //fast fail
                return new ProductDto(Id : Id, Name : Name, Price : Price, Stock : Stock, Feature : null);

            return new ProductDto(Id: Id, Name: Name, Price: Price, Stock: Stock, Feature: new ProductFeatureDto(Feature.Width, Feature.Height, Feature.Color.ToString()));

        }


    
    }
}
