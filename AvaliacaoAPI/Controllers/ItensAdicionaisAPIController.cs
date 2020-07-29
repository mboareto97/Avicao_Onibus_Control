using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvaliacaoAPI.Controllers
{
    [ApiController]
    [Route("v1/itensAdicionais")]
    public class ItensAdicionaisAPIController : Controller
    {
        [HttpGet]
        [Route("{origemId:int}/{destinoId:int}")]
        public ActionResult<List<int>> Get([FromServices] BaseContext context, int origemId = 0, int destinoId = 0)
        {
            var itens = new List<int>();
            var veiculos = new List<Veiculo>();
            var listaFuncionarios = context.Viagem.ToList();

            if (origemId > 0 && destinoId > 0)
            {
                var queryViagem = (from viagem in context.Viagem
                                   where viagem.OrigemId == origemId &&
                                         viagem.DestinoId == destinoId
                                   select viagem).ToList();

                if (queryViagem.Count > 0)
                {
                    foreach (var viagem in queryViagem)
                    {
                        var veiculo = context.Veiculo.AsNoTracking().FirstOrDefault(f => f.Id == viagem.VeiculoId);

                        if (veiculo != null)
                        {
                            if (!veiculos.Contains(veiculo))
                                veiculos.Add(veiculo);
                        }
                    }

                    foreach (var vei in veiculos)
                    {
                        var queryItens = context.ItemAdicionalVeiculo.AsNoTracking().Where(a => a.VeiculoId == vei.Id).ToList();

                        if (queryItens.Count > 0)
                        {
                            foreach (var item in queryItens)
                            {
                                if (!itens.Contains(item.ItemAdicionalId))
                                    itens.Add(item.ItemAdicionalId);
                            }
                        }
                    }
                }
            }

            return itens;
        }
    }
}
