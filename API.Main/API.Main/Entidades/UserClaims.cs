using API.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace API.Main.Entidades
{   
    [Serializable]
    [DataContract]
    public class Menus
    {
        [DataMember]
        public List<Menu> ListaMenus { get; set; }
      
    }
}
