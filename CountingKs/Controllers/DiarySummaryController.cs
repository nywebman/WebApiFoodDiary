using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiarySummaryController : BaseApiController
    {
        private CountingKsIdentityService _identityService;    
        
        public DiarySummaryController(ICountingKsRepository repo,
            CountingKsIdentityService identityService)
            : base(repo)
        {
            _identityService = identityService;
        }

        public HttpResponseMessage Get(DateTime diaryId)
        {
            //DiarySummaryModel
            try
            {
                var diary = TheRepository.GetDiary(_identityService.CurrentUser,diaryId);
                if (diary == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return TheModelFactory.CreateSummary(diary);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}