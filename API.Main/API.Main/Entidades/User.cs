using API.Main.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace API.Main.Entidades
{
    [Serializable]
    [DataContract]
    public class Usuario
    {
        [DataMember]
        public List<Access> Acessos { get; set; }

        [DataMember]
        public User DadosUsuario { get; set; }

        [DataMember]
        public int IndicadorErro { get; set; }

        [DataMember]
        public string CodigoErro { get; set; }

        [DataMember]
        public string DescricaoErro { get; set; }
    }    
}
