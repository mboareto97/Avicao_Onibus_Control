using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoUbus.Metodos
{
    public class APICall
    {
        struct Constantes
        {
            public const string ItensAdicionais = "v1/itensAdicionais";
            public const string Link = "https://localhost:5002/";
            public const int Tentativas = 3;
        }

        private static string MontaLink(string origemId, string destinoId)
        {
            return Constantes.Link + Constantes.ItensAdicionais + $"/{origemId}/{destinoId}";
        }

        public static List<int> ExecutaApiItensAdicionais(int origemId = 0, int destinoId = 0)
        {
            List<int> retornoApi;

            retornoApi = Metodos.ExecGetAPI(MontaLink(origemId.ToString(), destinoId.ToString()), Constantes.Tentativas);

            return retornoApi;
        }
    }
}
