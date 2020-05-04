using API.Main.Models;
using API.Main.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Main
{
    public class Util : ControllerBase
    {
        public static string ExtractUserName(string identity)
        {
            string result = null;
            try
            {
                result = identity.Split('\\').Last();
            }
            catch {
                throw new Exception();
            }
            return result;
        }
        
        public static Sessao CarregarDadosSessao(Client cliente, string pagina)
        {
            Sessao dadosSessaoCliente = new Sessao();                       
            dadosSessaoCliente.DataDeNascimento = item.DataNascimento;             
            dadosSessaoCliente.NomeCliente = item.NomePessoa;

            return dadosSessaoCliente;
        }

        public static string RetornoIP()
        {
            string ip = string.Empty;
            try
            {
                IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
                ip = heserver.AddressList[1].ToString();
            }
            catch
            {
                throw new Exception();
            }
            return ip;
        }
       
    }
}
