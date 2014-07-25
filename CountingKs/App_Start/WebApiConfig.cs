using CacheCow.Server;
using CacheCow.Server.EntityTagStore.SqlServer;
using CountingKs.Converters;
using CountingKs.Filters;
using CountingKs.Services;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
        config.MapHttpAttributeRoutes();
        /*
        config.Routes.MapHttpRoute(
            name: "Food",
            routeTemplate: "api/nutrition/{foodid}",
            defaults: new { Controller = "Foods", foodid = RouteParameter.Optional },
            constraints: new { id="/d+"} //regular expression for the parameter so its only an integer that accepted
        );
        */
        config.Routes.MapHttpRoute(
            name: "Measures",
            routeTemplate: "api/nutrition/{foodid}/measures/{id}",
            defaults: new { Controller = "measures", id = RouteParameter.Optional },
            constraints: new { id = "/d+" } //regular expression for the parameter so its only an integer that accepted
        );
        /*
        config.Routes.MapHttpRoute(
            name: "Measures2",
            routeTemplate: "api/nutrition/{foodid}/measures/{id}",
            defaults: new { Controller = "MeasuresV2", id = RouteParameter.Optional },
            constraints: new { id = "/d+" } //regular expression for the parameter so its only an integer that accepted
        );
        */
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
        jsonFormatter.SerializerSettings.Converters.Add(new LinkModelConverter()); //converters know how to convert into and out of json
        CreateMediaTypes(jsonFormatter);

        //Add support for jsonp
        //without accepts header will go to json, need accepts with javascript
        //also nees ?callback= in the url
        var formatter = new JsonpMediaTypeFormatter(jsonFormatter,"cb");
        config.Formatters.Insert(0, formatter);

        //Replace the controller configuration with our controller selector
        //config.Services.Replace(typeof(IHttpControllerSelector),new CountingKsControllerSelector(config));


        //Configure caching/etag support
        //this outputs etag after request in response header
        var connString=ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        var etagStore = new SqlServerEntityTagStore(connString);
        var cacheHandler = new CachingHandler(config);  //not sure if config is correct, example didnt pass any params
        cacheHandler.AddLastModifiedHeader = false;  // not needed, true by default
        config.MessageHandlers.Add(cacheHandler);



        #if !DEBUG
                //Force HTTPS on entire API
                config.Filters.Add(new RequireHttpsAttribute());
        #endif

    }

    private static void CreateMediaTypes(JsonMediaTypeFormatter jsonFormatter)
    {
        //if someone comes in with these types, assume we are talking about json formatter
        var mediaTypes = new string[]
        {
            "application/vnd.countingks.food.v1+json",
            "application/vnd.countingks.measure.v1+json",
            "application/vnd.countingks.measure.v2+json",
            "application/vnd.countingks.diary.v1+json",
            "application/vnd.countingks.diaryEntry.v1+json"
        };
        foreach (var mediaType in mediaTypes)
        {
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
        }
    }
  }
}