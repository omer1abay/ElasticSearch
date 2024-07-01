using Elasticsearch.Net;
using Nest;
using ElasticSearch.API.Extensions;
using ElasticSearch.API.Services;
using ElasticSearch.API.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Elastic client injection'ý için oluþturduðumuz extension sýnýfýný ekliyoruz
builder.Services.AddElasticClient(builder.Configuration);

//repository ve service'ler mutlaka scope olarak eklenmeli
//DB'ye baðlanmayan Helper sýnýflarý singleton olabilir
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductRepository>();

//DI Container -> SOLID ve IoC prensiplerini uygulamamýzý saðlar
// ! ? iþaretleri sadece compiler içindir runtime'a bir etkileri yoktur. Nullable özelliði kapatmak nullable olabilecek yerleri tespit etmek için saðlýklý deðildir.
//EF Core'daki Context nesnesi singleton olursa transaction bozulur
//Transient -> her constructor çaðýrýmýnda bir tane üretir
//Scoped -> Her request ve response iþlemi tamamlanýnca memory'den atýlýr yenisi üretilir
//Singleton -> uygulama boyunca bir nesne örneði kullanýlýr.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
