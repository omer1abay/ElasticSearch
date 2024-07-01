using ElasticSearch.API.DTOs;
using ElasticSearch.API.Repository;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
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


    }
}
