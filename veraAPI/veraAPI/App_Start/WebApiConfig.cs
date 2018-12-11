﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace VeraAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var corsPolicy = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "GET, POST, OPTIONS, PUT");
            config.EnableCors(corsPolicy);
        }
    }
}
