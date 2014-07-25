using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Filters;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize(false)]
    [RoutePrefix("api/nutrition/foods")]
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repo) : base(repo)
        {

        }

        const int PAGE_SIZE = 50;

        [Route("",Name="Food")]
        public object Get(bool includeMeasures=true,int page= 0)
        {
            var repo = new CountingKsRepository(new CountingKsContext());

            IQueryable<Food> query;
            if(includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();

            }

            var baseQuery = query.OrderBy(f => f.Description);

            var totalCount = baseQuery.Count();
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE); //Math.Ceiling to get 1 more page than needed, so last page is partial page
            var helper = new UrlHelper(Request);

            var links = new List<LinkModel>();

            if(page>0)
            {
                links.Add(TheModelFactory.CreateLink(helper.Link("Food", new { page = page - 1 }),"prevPage"));
            }
            if(page<totalPages -1)
            {
                links.Add(TheModelFactory.CreateLink(helper.Link("Food", new { page = page + 1 }), "nextPage"));
            }

            var results = baseQuery.Skip(PAGE_SIZE * page)
                .Take(25)
                .ToList()
                .Select(f => TheModelFactory.Create(f));

            return new
            {
                TotalCount = totalCount,
                TotalPage = totalPages,
                Links=links,
                Results = results
            };
        }
        [Route("{foodid}", Name = "Food")] //naming the route will allow things like the helper.Link(~~~) work even in the model factory create() method
        public IHttpActionResult Get(int foodid)
        {
           // return Versioned(TheModelFactory.Create(TheRepository.GetFood(foodid)));
            return Versioned(TheModelFactory.Create(TheRepository.GetFood(foodid)),"v2");
        }
    }
}
