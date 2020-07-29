using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvaliacaoUbus.Metodos
{
    public static class Metodos
    {
        public static List<int> ExecGetAPI(string api, int tentativas, int timeOutEmSegundos = 100)
        {
            List<int> retorno = new List<int>();

            try
            {
                Stopwatch sw = new Stopwatch();
                Random gerador = new Random();

                for (int n = 1; n <= tentativas; n++)
                {
                    sw.Restart();
                    int handleTentativa = gerador.Next(1, 9999999);

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, api))
                        {
                            client.Timeout = new TimeSpan(0, 0, timeOutEmSegundos);

                            var response = client.SendAsync(request).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                retorno = JsonConvert.DeserializeObject<List<int>>(response.Content.ReadAsStringAsync().Result);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //se der erro nao faca nada
            }

            return retorno;
        }
    }
}
