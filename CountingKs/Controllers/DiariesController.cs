using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using CountingKs.Filters;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize]
    public class DiariesController : BaseApiController
    {
        public CountingKsIdentityService _identityService { get; set; }

        public DiariesController(ICountingKsRepository repo,
            CountingKsIdentityService identityService)
            : base(repo)
        {
            _identityService = identityService;
        }
        
        public IEnumerable<DiaryModel> Get(int id)
        {
            var username = _identityService.CurrentUser;
            var results = TheRepository.GetDiaries(username)
                .OrderByDescending(d=>d.CurrentDate)
                .Take(10)
                .ToList()
                .Select(d=>TheModelFactory.Create(d));

            return results;
        }

        public HttpResponseMessage Get(DateTime diaryid)
        {
             var username = _identityService.CurrentUser;
             var result = TheRepository.GetDiary(username, diaryid);

             if (result == null)
             {
                 return Request.CreateResponse(HttpStatusCode.NotFound);
             }

             return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }
    }
}