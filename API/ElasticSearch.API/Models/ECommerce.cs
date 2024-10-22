﻿using System.Text.Json.Serialization;

namespace ElasticSearch.API.Models.ECommerceModels
{
    public class ECommerce
    {

        //NEST kütüphanesi kullansaydık JsonPropertyName kullanmayacaktık ona ait bir attribute var

        [JsonPropertyName("_id")]
        public string Id  { get; set; } = null!;

        [JsonPropertyName("customer_first_name")]
        public string CustomerFirstName { get; set; } = null!;

        [JsonPropertyName("customer_last_name")]
        public string CustomerLastName  { get; set; } = null!;

        [JsonPropertyName("customer_full_name")]
        public string CustomerFullName  { get; set; } = null!;

        [JsonPropertyName("category")]
        public string[] Category { get; set; } = null!;

        [JsonPropertyName("taxful_total_price")]
        public double TaxfulTotalPrice{ get; set; }

        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }

        [JsonPropertyName("order_date")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("products")]
        public Product[] Products{ get; set; }
    }

    public class Product
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
    }

}
