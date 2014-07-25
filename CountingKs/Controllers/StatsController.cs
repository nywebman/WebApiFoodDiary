using CountingKs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    //this is used as prefix for all below
    [RoutePrefix("api/stats")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo)
            : base(repo)
        {
            
        }
        [Route("")]
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };
            return Request.CreateResponse(results);
        }

        [Route("{id}")]
        public HttpResponseMessage Get(int id)
        {
            if (id == 1)
            {

            }
            if (id == 2)
            {
            }

            throw new NotImplementedException();
        }

        [Route("~/api/stat/{name:alpha}")]
        public HttpResponseMessage Get(string name)
        {
            throw new NotImplementedException();
        }
    }
}
