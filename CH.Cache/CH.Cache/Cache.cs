using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Runtime.Caching;
using System.Text;

namespace CH.Cache
{    
    public class Cache
    {

        private string TIPO_GRAVACAO_CACHE = string.Empty;
        private string ALIAS_SERVIDOR_CACHE = "servidor";
        private string CHAVE_COMPLETA = string.Empty;
        
        public Cache()
        {
            List<string> validCacheTypes = new List<string>() { "REDIS", "DOTNET" };
            if (validCacheTypes.Contains(ConfigurationManager.AppSettings["TIPO_GRAVACAO_CACHE"]))
                TIPO_GRAVACAO_CACHE = ConfigurationManager.AppSettings["TIPO_GRAVACAO_CACHE"].ToString();
            else
                TIPO_GRAVACAO_CACHE = "REDIS";
        }
       
        public void Adicionar(string chave, object valor, double tempoArmazenamentoEmMinutos, string siglaSistema)
        {

            CHAVE_COMPLETA = siglaSistema.ToUpper() + "-" + chave.ToUpper();
            string dadosSerializados = string.Empty;
            try
            {
                if (TIPO_GRAVACAO_CACHE.ToUpper() == "REDIS")
                {                    
                    using (var redis = new RedisClient(ALIAS_SERVIDOR_CACHE))
                    {
                        if (redis.Exists(CHAVE_COMPLETA) > 0)
                        {
                            redis.Del(CHAVE_COMPLETA);
                            dadosSerializados = JsonConvert.SerializeObject(valor, Formatting.Indented);
                            redis.SetEx(CHAVE_COMPLETA, (int)(tempoArmazenamentoEmMinutos * 60), Compactar(dadosSerializados));
                        }
                        else
                        {
                            dadosSerializados = JsonConvert.SerializeObject(valor, Formatting.Indented);
                            redis.SetEx(CHAVE_COMPLETA, (int)(tempoArmazenamentoEmMinutos * 60), Compactar(dadosSerializados));
                        }
                    }
                }
                else if (TIPO_GRAVACAO_CACHE.ToUpper() == "DOTNET")
                {                   
                    ObjectCache cache = MemoryCache.Default;
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(tempoArmazenamentoEmMinutos);
                    cache.Add(CHAVE_COMPLETA, valor, cacheItemPolicy);                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
        public void Remover(string chave, string siglaSistema)
        {

            CHAVE_COMPLETA = siglaSistema.ToUpper() + "-" + chave.ToUpper();

            try
            {
                if (TIPO_GRAVACAO_CACHE.ToUpper() == "REDIS")
                {                  
                    using (var redis = new RedisClient(ALIAS_SERVIDOR_CACHE))
                    {
                        if (redis.Exists(CHAVE_COMPLETA) > 0)
                        {
                            redis.Del(CHAVE_COMPLETA);
                        }
                    }
                }
                else if (TIPO_GRAVACAO_CACHE.ToUpper() == "DOTNET")
                {                    
                    ObjectCache cache = MemoryCache.Default;
                    cache.Remove(CHAVE_COMPLETA);                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
        public T Obter<T>(string chave, string siglaSistema)
        {
            CHAVE_COMPLETA = siglaSistema.ToUpper() + "-" + chave.ToUpper();
            object retorno = null;
            object retornoFinal = null;
            try
            {
                if (TIPO_GRAVACAO_CACHE.ToUpper() == "REDIS")
                {                   
                    using (var redis = new RedisClient(ALIAS_SERVIDOR_CACHE))
                    {
                        bool temCache = redis.Exists(CHAVE_COMPLETA) > 0;

                        if (temCache)
                        {
                            retorno = JsonConvert.DeserializeObject<T>(Descompactar(redis.Get(CHAVE_COMPLETA)));
                        }
                    }
                    retornoFinal = (T)Convert.ChangeType(retorno, typeof(T)); ;
                }
                else if (TIPO_GRAVACAO_CACHE.ToUpper() == "DOTNET")
                {                    
                    ObjectCache cache = MemoryCache.Default;
                    object objRetornoOriginal = cache.Get(CHAVE_COMPLETA);
                    retornoFinal = (T)Convert.ChangeType(objRetornoOriginal, typeof(T));                   
                }
                return (T)Convert.ChangeType(retornoFinal, typeof(T)); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public bool Contem(string chave, string siglaSistema)
        {
            CHAVE_COMPLETA = siglaSistema.ToUpper() + "-" + chave.ToUpper();
            bool retorno = false;
            try
            {
                if (TIPO_GRAVACAO_CACHE.ToUpper() == "REDIS")
                {                    
                    using (var redis = new RedisClient(ALIAS_SERVIDOR_CACHE))
                    {                        
                        retorno = redis.Exists(CHAVE_COMPLETA) > 0;                        
                    }
                }
                else if (TIPO_GRAVACAO_CACHE.ToUpper() == "DOTNET")
                {                    
                    ObjectCache cache = MemoryCache.Default;
                    retorno = cache.Contains(CHAVE_COMPLETA);
                    
                }
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static bool TryParse<T>(string s, out T value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                value = (T)converter.ConvertFromString(s);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        private byte[] Compactar(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

            return gzBuffer;
        }

        private string Descompactar(byte[] gzBuffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
                byte[] buffer = new byte[msgLength];
                ms.Position = 0;

                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}