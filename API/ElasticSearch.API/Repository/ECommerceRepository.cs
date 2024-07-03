using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModels;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repository
{
    public class ECommerceRepository
    {
        //C# yeni gelen özellikler ile functional programlama için immutable nesneleri daha fazla destekliyor.
        //public int MyProperty { get; init; } //ÖRN : buradaki init keyword'ü değişkenin yalnızca nesne örneği oluşturulduğunda değer verilebilmesini sonradan değiştirilememesini sağlar

        //private readonly ElasticClient _client; NEST kütüphanesi
        private readonly ElasticsearchClient _client;

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        private const string indexName = "kibana_sample_data_ecommerce";



        public async Task<ImmutableList<ECommerce>> TermQuery(string customer_first_name)
        {
            #region Tip Güvensiz
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //                            s.Index(indexName)
            //                             .Query(q =>
            //                                q.Term(t =>
            //                                    t.Field("customer_first_name.keyword")
            //                                     .Value(customer_first_name)
            //                                )
            //                            )
            //                       );
            #endregion

            #region Tip Güvenli
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //                            s.Index(indexName)
            //                             .Query(q =>
            //                                q.Term(t =>
            //                                    t.CustomerFirstName.Suffix("keyword"), customer_first_name //Suffix ile son ek ekledik ve tip güvenli hale getirdik
            //                                )
            //                            )
            //                       );
            #endregion

            #region 3.yol

            var termQuery = new TermQuery("customer_first_name.keyword")
            {
                Value = customer_first_name,
                CaseInsensitive = true
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            #endregion


            foreach (var term in result.Hits) term.Source.Id = term.Id;

            return result.Documents.ToImmutableList(); //Immutable dönmemizin sebebi data üzerinde bir değişiklik yapılamamasıdır daha güvenlidir fonksiyonel programlamaya daha uygundur.

        }


        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNames)
        {
            List<FieldValue> terms = new List<FieldValue>();

            customerFirstNames.ForEach(x => terms.Add(x));

            #region 1.yol
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));
            #endregion

            #region 2.yol

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(100)
                                                                    .Query(q =>
                                                                    q.Terms(t =>
                                                                    t.Field(f =>
                                                                    f.CustomerFirstName.Suffix("keyword"))
                                                                     .Terms(new TermsQueryField(terms.AsReadOnly())))));

            #endregion

            foreach (var res in result.Hits) res.Source.Id = res.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQuery(string prefix)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Query(q =>
                                            q.Prefix(p=> 
                                                p.Field(f => f.CustomerFullName.Suffix("keyword"))
                                                 .Value(prefix)
                                            )
                                        )
                                   );
            
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        
        }

        public async Task<ImmutableList<ECommerce>> RangeQuery(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Query(q =>
                                            q.Range(r => 
                                                r.NumberRange(nr => 
                                                    nr.Field(f => f.TaxfulTotalPrice)
                                                      .Gte(fromPrice)
                                                      .Lte(toPrice)
                                                )
                                            )
                                        )
                                   );
            

            return result.Documents.ToImmutableList();

        }


        public async Task<ImmutableList<ECommerce>> MatchAllQuery()
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Size(100)
                                         .Query(q =>
                                            q.MatchAll()
                                          )
                                   );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


        public async Task<ImmutableList<ECommerce>> PaginationQuery(int page, int pageSize)
        {
            //pagination istersek her query'e uygulayabiliriz

            // page 1 ve pageSize 10 ise 1-10 arasındaki kayıtları getirir
            // page 1 ve pageSize 10 ise 11-20 arasındaki kayıtları getirir
            // page 3 ve pageSize 10 ise 21-30 arasındaki kayıtları getirir

            var pageFrom = (page - 1) * pageSize; // bu hesap ile sayfalamayı kullanabiliriz.

            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Size(pageSize)
                                         .From(pageFrom)
                                         .Query(q =>
                                            q.MatchAll()
                                          )
                                   );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCardQuery(string customerFullName)
        {

            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Query(q =>
                                            q.Wildcard(w => 
                                                w.Field(f => 
                                                    f.CustomerFirstName.Suffix("keyword")
                                                ).Wildcard(customerFullName)
                                            )
                                          )
                                   );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


        public async Task<ImmutableList<ECommerce>> FuzzyQuery(string customerName, int fuzziness)
        {

            var result = await _client.SearchAsync<ECommerce>(s =>
                                        s.Index(indexName)
                                         .Query(q =>
                                            q.Fuzzy(fu => 
                                                fu.Field(f=> f.CustomerFirstName.Suffix("keyword"))
                                                 .Value(customerName)
                                                 .Fuzziness(new Fuzziness(fuzziness))
                                            )
                                          ).Sort(sort => sort.Field(fi => fi.TaxfulTotalPrice, new FieldSort() { Order = SortOrder.Desc })) //Order, sıralama işlemi
                                   );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FullTextQuery(string content)
        {
            //ES default olarak boşluk karakterleri ile parçalar.
            //Ömer Abay ile yapılan bir aramada Ömer (or) Abay gibi bir arama yapar ES
            //Term level query yaparsak burada direk bir arama yapar score dönmez. direkt şartı sağlama ister.

            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.Match(m =>
                                                    m.Field(f => f.Category)
                                                        .Query(content)
                                                        .Operator(Operator.And)))); //operator belirtirsek bu sefer kelimeleri parçalayıp aralarına and ifadesini koyar default or'dur.

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBooleanPrefixFullTextQuery(string content)
        {
            //sondaki kelimeyi bir prefix olarak algılar
            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.MatchBoolPrefix(m =>
                                                    m.Field(f => f.CustomerFullName)
                                                        .Query(content)))); 

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseFullTextQuery(string content)
        {
            //sıralaması ile birlikte tüm kelimeyi prefix olarak algılar yani öbek olarak
            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.MatchPhrase(m =>
                                                    m.Field(f => f.CustomerFullName)
                                                        .Query(content))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


        public async Task<ImmutableList<ECommerce>> CompoundQuery(string content, double taxfulltotalprice,string categoryName, string manufacturer)
        {
            //must, must not, filter, should keyword'leri

            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.Bool(b => 
                                                    b.Must(m => 
                                                        m.Term(t => 
                                                            t.Field("geoip.city_name")
                                                             .Value(content)
                                                        )
                                                    )
                                                    .MustNot(mn => 
                                                        mn.Range(r => 
                                                            r.NumberRange(nr => 
                                                                nr.Field(f => f.TaxfulTotalPrice)
                                                                  .Lte(taxfulltotalprice)
                                                            )
                                                        )
                                                    )
                                                    .Should(s => 
                                                        s.Term(t => 
                                                            t.Field(f => f.Category.Suffix("keyword"))
                                                             .Value(categoryName)
                                                        )
                                                    )
                                                    .Filter(f => 
                                                        f.Term(t => 
                                                            t.Field("manufacturer.keyword")
                                                             .Value(manufacturer)
                                                        )
                                                    )
                                                )
                                              )
                                          );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


        public async Task<ImmutableList<ECommerce>> CompoundQuery2(string customerFullName)
        {
            //aşağıdaki işlemin daha basit hali MatchPhrasePrefix Query'dir.

            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.Bool(b => 
                                                    b.Should(m => 
                                                        m.Match(m => 
                                                            m.Field(f => f.CustomerFullName)
                                                             .Query(customerFullName)
                                                        )
                                                        .Prefix(p => 
                                                            p.Field(f => f.CustomerFullName.Suffix("keyword"))
                                                             .Value(customerFullName)
                                                        )
                                                    )
                                                )
                                              )
                                          );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


        public async Task<ImmutableList<ECommerce>> MultiMatchQuery(string name)
        {
            //aşağıdaki işlemin daha basit hali MatchPhrasePrefix Query'dir.

            var result = await _client.SearchAsync<ECommerce>(s =>
                                            s.Index(indexName)
                                             .Size(1000)
                                             .Query(q =>
                                                q.MultiMatch(mm => 
                                                    mm.Fields(
                                                        new Field("customer_first_name")
                                                        .And(new Field("customer_last_name"))
                                                        .And(new Field("customer_full_name")))
                                                        .Query(name)
                                                )
                                              )
                                          );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

    }
}
