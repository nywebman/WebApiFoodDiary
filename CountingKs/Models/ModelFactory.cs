using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;
using CountingKs.Data;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private ICountingKsRepository _repo;
        public UrlHelper _urlHelper { get; set; }
        
        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repo)
        {
            _urlHelper = new UrlHelper();
            _repo = repo;
        }


        public FoodModel Create(Food food)
        {
            return new FoodModel()
            {
                Url = _urlHelper.Link("Food", new { foodid=food.Id}),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel()
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id=measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories)
            };
        }



        public DiaryModel Create(Diary d)
        {
            return new DiaryModel()
            {
                Links=new List<LinkModel>()
                {
                    CreateLink(_urlHelper.Link("Diaries", new { diaryid= d.CurrentDate.ToString("yyyy-MM-dd") }),
                        "Self"),
                    CreateLink(_urlHelper.Link("DiaryEntries", new { diaryid= d.CurrentDate.ToString("yyyy-MM-dd") }),
                        "newDiaryEntry","POST")
                },
                CurrentDate=d.CurrentDate
            };
        }

        public LinkModel CreateLink(string href, string rel,string method="GET",bool isTemplated=false)
        {
            return new LinkModel()
            {
                Href = href,
                Rel = rel,
                Method=method,
                IsTemplated=isTemplated
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (model.Quantity != default(double))
                {
                    entry.Quantity = model.Quantity;
                }
                var uri = new Uri(model.MeasureUrl);
                var measureId = int.Parse(uri.Segments.Last());
                var measure = _repo.GetMeasure(measureId);
                entry.Measure = measure;
                entry.FoodItem = measure.Food;

                return entry;
            }
            catch
            {
                return null;
            }
        }



        public DiaryEntryModel Create(DiaryEntry entity)
        {
            throw new NotImplementedException();
        }

        public DiarySummaryModel CreateSummary(Diary diary)
        {
            return new DiarySummaryModel()
            {
                DiaryDate = diary.CurrentDate,
                TotalCalories = Math.Round(diary.Entries.Sum(e => e.Measure.Calories * e.Quantity))
            };
        }

        public AuthTokenModel Create(AuthToken authToken)
        {
            return new AuthTokenModel()
            {
                Token = authToken.Token,
                Expiration = authToken.Expiration
            };
        }

        public MeasureV2Model Create2(Measure measure)
        {
            return new MeasureV2Model()
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories),
                Carbohydrates=measure.Carbohydrates,
                Cholestrol=measure.Cholestrol,
                Fiber = measure.Fiber
                //... and so on for the rest in the model
            };
        }


        public Diary Parse(DiaryModel model)
        {
            try
            {
                var entity = new Diary();

                var selfLink = model.Links.Where(l => l.Rel == "self").FirstOrDefault();
                if (selfLink != null && !string.IsNullOrWhiteSpace(selfLink.Href))
                {
                    var uri = new Uri(selfLink.Href);
                    entity.Id = int.Parse(uri.Segments.Last());
                }

                entity.CurrentDate = model.CurrentDate;

                if (model.Entries != null)
                {
                    foreach (var entry in model.Entries) entity.Entries.Add(Parse(entry));
                }

                return entity;
            }
            catch
            {
                return null;
            }
        }
    }
}