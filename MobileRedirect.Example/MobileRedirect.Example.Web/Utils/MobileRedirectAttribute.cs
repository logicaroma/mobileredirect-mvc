using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace Mobile.Redirect.Example.Web.Utils
{
    /// <summary>
    /// Redirects to the mobile view if on a supported device
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MobileRedirectAttribute : AuthorizeAttribute
    {
        private string _clientFragment;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MobileRedirectAttribute()
        {
            _clientFragment = string.Empty;
        }

        /// <summary>
        /// Constructor that takes an argument
        /// </summary>
        /// <param name="clientUrl">The url fragment we should append to the url</param>
        public MobileRedirectAttribute(string clientFragment)
        {
            _clientFragment = clientFragment;
        }

        /// <summary>
        /// Tests if this request originates from a supported mobile device 
        /// and redirects as appropriate
        /// </summary>
        /// <param name="ctx">The action execution context</param>
        public override void OnAuthorization(AuthorizationContext ctx)
        {
            //------------------------------------------------------------------------
            // THIS TEST IS A QUICK KLUDGE IMPLEMENT YOUR OWN MOBILE DEVICE TEST HERE
            if (ctx.HttpContext.Request.UserAgent.Contains("iPad"))
            //------------------------------------------------------------------------
            {
                // parse the fragment with request parameters
                string fragment = ParseClientFragment(ctx);

                // construct the redirect url
                UrlHelper urlHelper = new UrlHelper(ctx.RequestContext);
                string url = string.Format("{0}#{1}", urlHelper.Action("Index", "Mobile"), fragment);

                // return redirect result to prevent action execution
                ctx.Result = new RedirectResult(url);
            }
        }

        /// <summary>
        /// Parses the client fragment and replaces :[token] with the request parameter
        /// </summary>
        /// <param name="ctx">The controller context</param>
        /// <returns>The parsed fragment</returns>
        private string ParseClientFragment(ControllerContext ctx)
        {
            string parsedFragment = _clientFragment ?? string.Empty;

            if (!string.IsNullOrEmpty(parsedFragment))
            {
                NameValueCollection @params = ctx.HttpContext.Request.Params;
                MatchCollection matches = Regex.Matches(_clientFragment, ":[a-zA-Z]+");
                RouteData routeData = RouteTable.Routes.GetRouteData(ctx.HttpContext);

                // check each token and replace with param or route values
                foreach (Match match in matches)
                {
                    string token = match.Value.TrimStart(':');
                    string value = @params[token];

                    // if we haven;t got a parameter here we must check the route values
                    if (string.IsNullOrEmpty(value) && routeData.Values.ContainsKey(token))
                    {
                        value = routeData.Values[token] as string;
                    }

                    // perform the replace
                    parsedFragment = parsedFragment.Replace(match.Value, value);
                }
            }

            return parsedFragment;
        }
    }
}