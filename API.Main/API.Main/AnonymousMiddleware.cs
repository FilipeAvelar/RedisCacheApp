using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Main
{
    public class AnonymousMiddleware : ControllerBase
    {
        #region Variables
        private readonly RequestDelegate _next;

        public static string nomeUsuario = string.Empty;
        //TODO: maybe this can be improved to get rid of these magic strings
        private List<string> AllowedControllers = new List<string>
        {
        "/Anonymous",
        "/api/authorization",
        "/api/cliente",
        "/api/cache",       
        "/AuthorizationController",
        "/ClienteController",
        "/CacheController",
        "/EntityDBController"
        };
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public AnonymousMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.Name != null)
                nomeUsuario = context.User.Identity.Name;

            // if requests target anonymous controller or there is a CORS related OPTIONS request
            // => let it be and challenge only for other request methods (GET, POST etc.)
            if (context.User.Identity.IsAuthenticated ||
                context.Request.Method == "OPTIONS" ||
                AllowedControllers.Any(c =>
                {                   
                    string path = context.Request.Path.ToString();                    
                    return path.StartsWith(c, StringComparison.InvariantCulture);
                }))
            {                
                await _next(context);
                return;
            }

            await context.ChallengeAsync("Windows");
        }

    }
}
