using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.ActionResult;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        ICountingKsRepository _repo { get; set; }
        ModelFactory _modelFactory { get; set; }

        public BaseApiController(ICountingKsRepository repo)
        {
            _repo = repo;
        }

        //defer createion of ModelFactory until needed
        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request,TheRepository);
                }
                return _modelFactory;
            }
        }
        protected ICountingKsRepository TheRepository
        {
            get
            {
                return _repo;
            }
        }

        protected IHttpActionResult Versioned<T>(T body,string version="v1") where T :class
        {
            return new VersionedActionResult<T>(request, version, body);
        }
    }
}