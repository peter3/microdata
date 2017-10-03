using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

/// <summary>
/// Enable CORS for specified origins
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
                AllowMultiple = true, Inherited = true)]
public sealed class EnableCorsAttribute : System.Web.Mvc.FilterAttribute, System.Web.Mvc.IActionFilter
{
    private const string AllowAllOrigins = "*";
    private const string IncomingOriginHeader = "Origin";
    private const string OutgoingOriginHeader = "Access-Control-Allow-Origin";
    private const string OutgoingMethodsHeader = "Access-Control-Allow-Methods";
    private const string OutgoingAgeHeader = "Access-Control-Max-Age";
    public EnableCorsAttribute()
    {
        AllowedOrigin = AllowAllOrigins;
        AllowedMethods = "GET,PUT,POST,DELETE,OPTIONS";
        MaxAge = 3600;
    }
    /// <summary>
    /// Maximum time in seconds the headers will be cached, default is 3600
    /// </summary>
    public int MaxAge { get; set; }
    /// <summary>
    /// A regular expression that describes one or more domain patterns that are allowed, default is "*" (all)
    /// </summary>
    public string AllowedOrigin { get; set; }
    /// <summary>
    /// Comma separated list of allowed methods, default is 'GET,PUT,POST,DELETE,OPTIONS'
    /// </summary>
    public string AllowedMethods { get; set; }
    /// <summary>
    /// Set to true to not allow local access, false by default
    /// </summary>
    public bool DisableLocal { get; set; }
    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
        // Do nothing
    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var isLocal = filterContext.HttpContext.Request.IsLocal && !DisableLocal;
        var originHeader =
             filterContext.HttpContext.Request.Headers.Get(IncomingOriginHeader);
        var response = filterContext.HttpContext.Response;

        if (!String.IsNullOrWhiteSpace(originHeader) &&
            (isLocal || IsAllowedOrigin(originHeader)))
        {
            response.AddHeader(OutgoingOriginHeader, originHeader);
            response.AddHeader(OutgoingMethodsHeader, AllowedMethods);
            response.AddHeader(OutgoingAgeHeader, MaxAge.ToString());
        }
    }

    private bool IsAllowedOrigin(string origin)
    {
        return AllowedOrigin == AllowAllOrigins || new Regex(AllowedOrigin).IsMatch(origin);
    }
}