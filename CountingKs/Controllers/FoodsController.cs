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
    public class FoodsController : ApiController
    {
        public ICountingKsRepository _repo { get; set; }
        public ModelFactory _modelFactory { get; set; }

        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;
            _modelFactory = new ModelFactory();
        }

        public IEnumerable<FoodModel> Get(bool includeMeasures=true)
        {
            var repo = new CountingKsRepository(new CountingKsContext());

            /* public IEnumerable<Food> Get()
             * 
             * iteration 1
            var results = repo.GetAllFoods()
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList();
            */

            /*
             * * iteration 2
            var results = repo.GetAllFoodsWithMeasures()
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                //lamda in select brings food into linq below
                //creating anonymous type below to send back with select
                //the 2 anonymous types below are so that no issue with circular references
                .Select(f => new
                {
                    Description = f.Description,
                    Measures = f.Measures.Select(m => new 
                    {
                        Description = m.Description,
                        Calories=m.Calories
                    }
                    )
                });
            */

            /*
            //not anonymous types in iteration 3
            var results = repo.GetAllFoodsWithMeasures()
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                .Select(f => new FoodModel()
                {
                    Description = f.Description,
                    Measures = f.Measures.Select(m => new MeasureModel()
                    {
                        Description = m.Description,
                        Calories = m.Calories
                    }
                    )
                });
            */


            IQueryable<Food> query;
            if(includeMeasures)
            {
                query = _repo.GetAllFoodsWithMeasures();
            }
            else
            {
                query = _repo.GetAllFoods();

            }


            var results = query.OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                .Select(f => _modelFactory.Create(f));
            
            return results;
        }

        public FoodModel Get(int foodid)
        {
            //wrap call with factory instead of calling just _repo, will also include things done in factory like rounding formatting
            return _modelFactory.Create(_repo.GetFood(foodid));
        }
    }
}
