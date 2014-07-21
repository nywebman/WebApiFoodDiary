using CountingKs.Filters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            name: "Food",
            routeTemplate: "api/v1/nutrition/{foodid}",
            defaults: new { Controller = "Foods", foodid = RouteParameter.Optional },
            constraints: new { id="/d+"} //regular expression for the parameter so its only an integer that accepted
        );

        config.Routes.MapHttpRoute(
            name: "Measures",
            routeTemplate: "api/v1/nutrition/{foodid}/measures/{id}",
            defaults: new { Controller = "measures", id = RouteParameter.Optional },
            constraints: new { id = "/d+" } //regular expression for the parameter so its only an integer that accepted
        );

        config.Routes.MapHttpRoute(
            name: "Measures2",
            routeTemplate: "api/v2/nutrition/{foodid}/measures/{id}",
            defaults: new { Controller = "MeasuresV2", id = RouteParameter.Optional },
            constraints: new { id = "/d+" } //regular expression for the parameter so its only an integer that accepted
        );

        config.Routes.MapHttpRoute(
            name: "Diaries",
            routeTemplate: "api/user/diaries/{diaryid}",
            defaults: new { Controller = "diaries", diaryid = RouteParameter.Optional }
        );

        config.Routes.MapHttpRoute(
            name: "DiarySummary",
            routeTemplate: "api/user/diaries/{diaryid}/summary",
            defaults: new { Controller = "DiarySummary" }
        );

        config.Routes.MapHttpRoute(
            name: "Token",
            routeTemplate: "api/token",
            defaults: new { Controller = "token" }
        );

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
        );

      // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
      // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
      // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
      //config.EnableQuerySupport();
      
        var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
        jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        //Add support for jsonp
        //without accepts header will go to json, need accepts with javascript
        //also nees ?callback= in the url
        var formatter = new JsonpMediaTypeFormatter(jsonFormatter,"cb");
        config.Formatters.Insert(0, formatter);


        #if !DEBUG
                //Force HTTPS on entire API
                config.Filters.Add(new RequireHttpsAttribute());
        #endif

    }
  }
}