using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using API.Main.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace API.Main
{
    public class ServiceClient
    {       
        #region métodos
        public static Object GetUserData(string usuarioNome)
        {           
            try
            {                                        
               return ConsultarUsuario(usuarioNome);                            
            }
            catch(Exception ex) {
                throw ex;
            }            
        }
        
        public static Cliente GetClient(int idCliente, Usuario usuario)
        {           
           try
            {                                        
               return ConsultarCliente(idCliente, usuario);                            
            }
            catch(Exception ex) {
                throw ex;
            }    
        }        
        #endregion
    }
}
