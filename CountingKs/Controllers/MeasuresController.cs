using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class MeasuresController : ApiController
    {
        public ModelFactory _modeFactory { get; set; }
        public ICountingKsRepository _repo { get; set; }

        public MeasuresController(ICountingKsRepository repo)
        {
            _repo = repo;
            _modeFactory = new ModelFactory();
        }

        public IEnumerable<MeasureModel> Get(int foodid)
        {
            var results = _repo.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m => _modeFactory.Create(m));

            return results;
        }

        public MeasureModel Get(int foodid, int id)
        {
            var results = _repo.GetMeasure(id);
            if (results.Food.Id == foodid)
            {
                return _modeFactory.Create(results);
            }
            else
            {
                return null;
            }
        }
    }
}