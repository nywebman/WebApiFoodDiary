﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CountingKs.Services
{
    public class CountingKsControllerSelector: DefaultHttpControllerSelector
    {
        private HttpConfiguration _config;
        public CountingKsControllerSelector(HttpConfiguration config):base(config)
        {
            _config = config;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {

            var controllers = GetControllerMapping(); //list of controllers
            var routeData = request.GetRouteData();

            var controllerName=(string) routeData.Values["controller"];

            HttpControllerDescriptor descriptor;
            if(controllers.TryGetValue(controllerName,out descriptor)) //need this way because can throw error if not found in collection
            {
                var version = GetVersionFromQueryString(request);
                var newName = string.Concat(controllerName, "V", version);
                HttpControllerDescriptor versionedDescriptor;
                if (controllers.TryGetValue(controllerName, out versionedDescriptor)) //need this way because can throw error if not found in collection
                {
                    return versionedDescriptor;
                }
                return descriptor;
            }
            return null; //so system will hanlde the way it would and return 404, or whatever
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var version = query["v"];
            if(version!=null)
            {
                return version;
            }
            return "1";
        }
    }
}