using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace API.Main.Entidades
{
    [Serializable]
    [DataContract]
    public class Sessao
    {
        [DataMember]
        public string Pagina { get; set; }        
    }
}
