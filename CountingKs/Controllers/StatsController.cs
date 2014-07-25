using CountingKs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo)
            : base(repo)
        {
            
        }

        //using atributes to define routs instead of through config
        [Route("api/stats")]
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };
            return Request.CreateResponse(results);
        }

        [Route("api/stats/{id}")]
        public HttpResponseMessage Get(int id)
        {
            //the param {id} in the route attrib is the same as the int id in the method call 

            if (id == 1)
            {

            }
            if (id == 2)
            {
            }

            throw new NotImplementedException();
        }
    }
}
