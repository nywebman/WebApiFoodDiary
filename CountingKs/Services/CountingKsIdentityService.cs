using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CountingKs.Services
{
    public class CountingKsIdentityService : CountingKs.Services.ICountingKsIdentityService
    {
        public string CurrentUser 
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name; //Getting username and sending it along with diaries, since in db diff
                                                                //users will have diff diaries
            }
        }
    }
}