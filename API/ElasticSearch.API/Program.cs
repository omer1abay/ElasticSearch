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

//Elastic client injection'� i�in olu�turdu�umuz extension s�n�f�n� ekliyoruz
builder.Services.AddElasticClient(builder.Configuration);

//repository ve service'ler mutlaka scope olarak eklenmeli
//DB'ye ba�lanmayan Helper s�n�flar� singleton olabilir
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductRepository>();

//DI Container -> SOLID ve IoC prensiplerini uygulamam�z� sa�lar
// ! ? i�aretleri sadece compiler i�indir runtime'a bir etkileri yoktur. Nullable �zelli�i kapatmak nullable olabilecek yerleri tespit etmek i�in sa�l�kl� de�ildir.
//EF Core'daki Context nesnesi singleton olursa transaction bozulur
//Transient -> her constructor �a��r�m�nda bir tane �retir
//Scoped -> Her request ve response i�lemi tamamlan�nca memory'den at�l�r yenisi �retilir
//Singleton -> uygulama boyunca bir nesne �rne�i kullan�l�r.

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
