using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MetReferenceDataService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ParsedDLApi",
                routeTemplate: "DriversLicense/",
                defaults: new
                {
                    action = "GetBylicenseText",
                    controller = "DLParser"
                }
                );

        }
    }
}
