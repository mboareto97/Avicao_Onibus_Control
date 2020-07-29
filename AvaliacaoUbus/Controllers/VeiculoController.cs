using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AvaliacaoUbus.Enums;
using AvaliacaoUbus.Models;
using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AvaliacaoUbus.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly ILogger<VeiculoController> _logger;
        private readonly IRepository _repository;
        private readonly BaseContext _context;

        public VeiculoController(ILogger<VeiculoController> logger, IRepository repository, BaseContext context)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
        }

        public IActionResult Index()
        {
            var veiculos = _repository.ListarVeiculo();

            return View(veiculos);
        }

        public IActionResult Create()
        {
            PreparaDadosView();
            return View("Form", new Veiculo());
        }

        public IActionResult Edit(Veiculo veiculoViewModel, int id, string operacao)
        {
            bool inclusao = operacao == "inclusao";

            ViewBag.Operacao = operacao;

            if (!inclusao)
            {
                if (id != veiculoViewModel.Id)
                {
                    return Error();
                }
            }

            if (string.IsNullOrEmpty(veiculoViewModel.Marca))
            {
                ModelState.AddModelError("Marca", "Campo Obrigatorio!");
            }
            if (string.IsNullOrEmpty(veiculoViewModel.Modelo))
            {
                ModelState.AddModelError("Modelo", "Campo Obrigatorio!");
            }

            if (ModelState.ErrorCount != 0)
            {
                PreparaDadosView();
                return View("Form", veiculoViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (inclusao)
                        _context.Add(veiculoViewModel);
                    else
                        _context.Update(veiculoViewModel);

                    _context.SaveChanges();

                    return RedirectToAction("Index", "Veiculo");
                }
                catch (Exception erro)
                {
                    _logger.LogError(erro.ToString());
                }
            }
            else
            {
                PreparaDadosView();
                return View(veiculoViewModel);
            }

            return View("Form", veiculoViewModel);
        }

        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return Error();
            }

            var veiculoViewModel = _context.Veiculo.AsNoTracking().FirstOrDefault(m => m.Id == id);
            if (veiculoViewModel == null)
            {
                return Error();
            }

            ViewBag.Operacao = "alteracao";
            veiculoViewModel.ItensVeiculo = ListaItensVeiculo(veiculoViewModel.Id);

            PreparaDadosView();

            return View("Form", veiculoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var veiculoViewModel = _context.Veiculo.AsNoTracking().FirstOrDefault(m => m.Id == id);
                if (veiculoViewModel != null)
                {
                    _context.Remove(veiculoViewModel);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index", "Veiculo");
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                return Error();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void SalvaItemAdicional(string itens, int veiculoId)
        {
            var ItensAdicionais = new List<ItemAdicionalVeiculo>();

            try
            {
                var itenSalvos = _context.ItemAdicionalVeiculo.AsNoTracking().Where(i => i.VeiculoId == veiculoId).ToList();

                if (itenSalvos != null)
                {
                    _context.RemoveRange(itenSalvos);
                    _context.SaveChanges();
                }

                var splitItens = itens.Split(",");

                foreach (var item in splitItens)
                {
                    var itemAdicional = new ItemAdicionalVeiculo { ItemAdicionalId = Convert.ToInt32(item), VeiculoId = veiculoId };
                    ItensAdicionais.Add(itemAdicional);
                }

                _repository.AdicionarListaItemAdicionalVeiculo(ItensAdicionais);
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
            }
        }

        private void PreparaDadosView()
        {
            ListaItensAdicionais();
        }

        private void ListaItensAdicionais()
        {
            var itens = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(typeof(EnumItensAdicionais)))
            {
                var descricao = GetDescription((EnumItensAdicionais)item);
                itens.Add(new SelectListItem { Text = descricao, Value = (Convert.ToInt32(item)).ToString() });
            }

            var listaItensFinal = itens.OrderBy(e => e.Text).ToList();

            ViewBag.ItensAdicionais = listaItensFinal;
        }

        private string ListaItensVeiculo(int veiculoId)
        {
            string itensVeiculo = string.Empty;
            var queryItens = _context.ItemAdicionalVeiculo.AsNoTracking().Where(a => a.VeiculoId == veiculoId).ToList();

            if (queryItens != null && queryItens.Count > 0)
            {
                foreach (var item in queryItens)
                {
                    itensVeiculo += item.ItemAdicionalId.ToString() + ",";
                }

                var virgula = itensVeiculo.LastIndexOf(',');
                itensVeiculo = itensVeiculo.Substring(0, virgula);
            }

            return itensVeiculo;
        }

        private string GetDescription(Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
    }
}
