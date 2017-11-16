using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.WebApi.RouteConstraints
{
    public class HttpMonitorIdRouteConstraint : IRouteConstraint
    {
        public const string RouteLabel = nameof(HttpMonitorId);

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var value = values[routeKey]?.ToString();

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new HttpMonitorId(Guid.Parse(value));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
