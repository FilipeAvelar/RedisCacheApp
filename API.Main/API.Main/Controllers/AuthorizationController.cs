using API.Main.Entidades;
using API.Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Main.Controllers
{
    [EnableCors()]
    [ApiController]
    [Route("/auth")]
    public class AuthorizationController : ControllerBase
    {       
        private readonly ILogger<AuthorizationController> _logger;
        private IDistributedCache _memoryCache;
        public AuthorizationController(ILogger<AuthorizationController> logger, IDistributedCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }       

        [HttpGet]        
        public Usuario Get()
        {
            Usuario result = null;
            string userName = Util.ExtractUserName(HttpContext.User.Identity.Name != null ? HttpContext.User.Identity.Name : AnonymousMiddleware.nomeUsuario);
            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();            
            string key = "key";
            try            
            {
                var existingCache = _memoryCache.GetString(key);
                if (!string.IsNullOrEmpty(existingCache))
                {                   
                    result = JsonConvert.DeserializeObject<Usuario>(existingCache);                                       
                }
                if (result == null)
                {
                    throw (new Exception("cache not found"));
                }
            }
            catch
            {
                Usuario user = (Usuario)Client.GetUserData(userName);

                if (user)
                {
                    result = new Usuario()
                    {
                        CodigoUsuario = userName,
                        NomeUsuario = user.Usuario.NomeUsuario,
                        Liberado = HttpContext.User.Identity.Name.ToLower()                       
                    };
                    try
                    {                       
                        _memoryCache.SetString(key, JsonConvert.SerializeObject(result), new DistributedCacheEntryOptions()
                                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(100)));                        
                    }
                    catch {
                        NotFound();
                    }
                }
            }
            finally
            {
                if (result == null)
                {
                    result = new Usuario();
                }
            }                      
            return result;
        }          
    }
}
