using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repository;
using System.Collections.Immutable;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {

            var response = await _repository.SaveAsync(request.CreateProduct());

            if (response == null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string>() { "kayıt esnasında bir hata meydana geldi"}, System.Net.HttpStatusCode.InternalServerError);
            }


            return ResponseDto<ProductDto>.Success(response.CreateDto(), System.Net.HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();

            var listProduct = products.Select(x => new ProductDto(x.Id, x.Name, x.Price, x.Stock, new ProductFeatureDto(x.Feature?.Width, x.Feature?.Height, x.Feature?.Color.ToString()))).ToList();


            return ResponseDto<List<ProductDto>>.Success(listProduct,System.Net.HttpStatusCode.OK);
        }


        public async Task<ResponseDto<ProductDto>> GetById(string id)
        {
            var product = await _repository.GetById(id);

            if (product == null)
            {
                return ResponseDto<ProductDto>.Fail("ilgili ürün bulunamadı", System.Net.HttpStatusCode.NotFound);
            }

            var resultProduct = product.CreateDto();

            return ResponseDto<ProductDto>.Success(resultProduct, System.Net.HttpStatusCode.OK);

        }


        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto productUpdateDto)
        {
            var responseProduct = await _repository.UpdateAsync(productUpdateDto);

            if (!responseProduct)
            {
                return ResponseDto<bool>.Fail("güncelleme işlemi başarısız",System.Net.HttpStatusCode.BadRequest);
            }


            return ResponseDto<bool>.Success(true,System.Net.HttpStatusCode.NoContent);
        }


        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var response = await _repository.DeleteAsync(id);

            //response.Result bize bazı bilgiler verir data ile ilgili, bu bilgi her durumda dolu olur.
            if (!response.IsValidResponse && response.Result == Result.NotFound)
            {
                return ResponseDto<bool>.Fail("silinecek ürün bulunamadı", System.Net.HttpStatusCode.BadRequest);
            }

            
            //loglama işlemi
            if (!response.IsValidResponse) //response.IsValid NEST kütüphanesinden gelir
            {
                //_logger.LogError(response.OriginalException, response.ServerError.Error.ToString()); NEST kütüphanesi

                //Elastic.Clients.Elasticsearch
                response.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, response.ElasticsearchServerError.Error.ToString());
                
                return ResponseDto<bool>.Fail("silme işlemi başarısız", System.Net.HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, System.Net.HttpStatusCode.NoContent);

        }


    }
}
