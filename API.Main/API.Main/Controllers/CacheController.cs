using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using CH.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Main.Entidades;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Main.Controllers
{
    [EnableCors()]
    [ApiController]
    
    public class CacheController : ControllerBase
    {
        private IDistributedCache _memoryCache;
        public CacheController(IDistributedCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        [Route("/cache/usuario")]
        [HttpGet]
        public object GetUsuario()
        {
            Usuario usuario = null;          
            string userName = Util.ExtractUserName(HttpContext.User.Identity.Name != null ? HttpContext.User.Identity.Name : AnonymousMiddleware.nomeUsuario);
            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();             
            string key = "key";

            var existingCache = _memoryCache.GetString(key);
            if (!string.IsNullOrEmpty(existingCache))
            {
                try
                {                   
                    usuario = JsonConvert.DeserializeObject<Usuario>(existingCache);
                }
                catch 
                {
                    NotFound();
                }
            }
            return usuario;
        }

        [Route("/cache/cliente")]
        [HttpGet()]
        public object GetCliente()
        {
            Cache cache = new Cache();
            Sessao sessao = null;
            string userName = Util.ExtractUserName(HttpContext.User.Identity.Name != null ? HttpContext.User.Identity.Name : AnonymousMiddleware.nomeUsuario);
            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();          
            string key = "key";           

            var cacheCliente = _memoryCache.GetString(key);
            if (!string.IsNullOrEmpty(cacheCliente))
            {
                try
                {                    
                    sessao = JsonConvert.DeserializeObject<Sessao>(cacheCliente);
                }
                catch
                {
                    NotFound();
                }
            }
            return sessao;
        }
    }
}
