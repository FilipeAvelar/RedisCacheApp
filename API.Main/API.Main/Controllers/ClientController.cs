using CH.Cache;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Main.Controllers
{
    [EnableCors()]
    [ApiController]
    [Route("/client/{idClient}")]
    public class ClientController : ControllerBase
    {
        private IDistributedCache _memoryCache;
        public ClienteController(IDistributedCache memoryCache)
        {            
            _memoryCache = memoryCache;
        }

        #region métodos
        [HttpGet]
        public Sessao Get(int idCliente)
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            Client cliente = Client.GetClient(idCliente, usuario);         
            
            var clienteSessao = Util.CarregarDadosSessao(cliente.Cliente, Request.Headers["Referer"].ToString());
            key += "|Cliente:";                    
            _memoryCache.SetString(key, JsonConvert.SerializeObject(clienteSessao), new DistributedCacheEntryOptions()
					   .SetAbsoluteExpiration(TimeSpan.FromSeconds(100)));           
                        
            var cacheCliente = _memoryCache.GetString();
            if (!string.IsNullOrEmpty(cacheCliente))
            {
                result = JsonConvert.Deserialize<Client>(cacheCliente);
            }

            return result;
        }                  
        #endregion        
    }
}
