using ElasticSearch.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {

        private readonly ECommerceRepository _repository;

        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            var response = await _repository.TermQuery(customerFirstName);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNames)
        {
            var response = await _repository.TermsQuery(customerFirstNames);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> TermsQuery(string prefix)
        {
            var response = await _repository.PrefixQuery(prefix);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            var response = await _repository.RangeQuery(fromPrice,toPrice);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            

            var response = await _repository.MatchAllQuery();

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page, int pageSize)
        {
            var response = await _repository.PaginationQuery(page,pageSize);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            var response = await _repository.WildCardQuery(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName, int fuzziness)
        {
            var response = await _repository.FuzzyQuery(customerName,fuzziness);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> FullTextQuery(string content)
        {
            var response = await _repository.FullTextQuery(content);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchBooleanPrefixFullTextQuery(string content)
        {
            var response = await _repository.MatchBooleanPrefixFullTextQuery(content);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchPhraseFullTextQuery(string content)
        {
            var response = await _repository.MatchPhraseFullTextQuery(content);

            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> CompoundQuery(string content, double taxfulltotalprice, string categoryName, string manufacturer)
        {
            var response = await _repository.CompoundQuery(content,taxfulltotalprice,categoryName,manufacturer);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQuery2(string customerFullName)
        {
            var response = await _repository.CompoundQuery2(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MultiMatchQuery(string name)
        {
            var response = await _repository.MultiMatchQuery(name);

            return Ok(response);
        }

    }
}
