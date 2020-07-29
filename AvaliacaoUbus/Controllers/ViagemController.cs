using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AvaliacaoUbus.Enums;
using AvaliacaoUbus.Models;
using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace AvaliacaoUbus.Controllers
{
    public class ViagemController : Controller
    {
        private readonly ILogger<ViagemController> _logger;
        private readonly IRepository _repository;
        private readonly BaseContext _context;

        public ViagemController(ILogger<ViagemController> logger, IRepository repository, BaseContext context)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
        }

        public IActionResult Index(DateTime? dataPesquisa = null, int? statusPesquisa = null)
        {
            //var viagens = _repository.ListarViagem();

            var viagens = new List<Viagem>();

            if (dataPesquisa != null)
            {
                if (statusPesquisa > 0)
                    viagens = _context.Viagem.AsNoTracking().Where(v => v.DataViagem == dataPesquisa && v.Status == statusPesquisa).ToList();
                else
                    viagens = _context.Viagem.AsNoTracking().Where(v => v.DataViagem == dataPesquisa).ToList();
            }
            else
            {
                if (statusPesquisa > 0)
                    viagens = _context.Viagem.AsNoTracking().Where(v => v.Status == statusPesquisa).ToList();
                else
                {
                    var listaViagens = _repository.ListarViagem();
                    viagens = (List<Viagem>)listaViagens;
                }
            }

            ListaStatus();

            foreach (var viagem in viagens)
            {
                viagem.OrigemDescricao = GetDescription((EnumMunicipios)viagem.OrigemId);
                viagem.DestinoDescricao = GetDescription((EnumMunicipios)viagem.DestinoId);
                if (viagem.FuncionarioId != 0)
                    viagem.NomeFuncionario = _context.Funcionario.AsNoTracking().FirstOrDefault(a => a.Id == viagem.FuncionarioId).Nome;
                else
                    viagem.NomeFuncionario = "Nenhum";
            }

            return View(viagens);
        }

        public IActionResult Create()
        {
            PreparaDadosView();
            return View("Form", new Viagem());
        }

        public IActionResult Edit(List<Viagem> viagens, Viagem viagemViewModel, int id, string operacao)
        {
            bool inclusao = operacao == "inclusao";

            ViewBag.Operacao = operacao;

            if (!inclusao)
            {
                if (id != viagemViewModel.Id)
                {
                    return Error();
                }
            }

            if (viagemViewModel.HoraPartida >= viagemViewModel.HoraChegada)
            {
                ModelState.AddModelError("HoraPartida", "Horário de partida maior que o de chegada!");
            }
            if (viagemViewModel.HoraChegada <= viagemViewModel.HoraPartida)
            {
                ModelState.AddModelError("HoraChegada", "Horário de chegada menor que o de partida!");
            }
            if (viagemViewModel.DataViagem <= DateTime.MinValue)
            {
                ModelState.AddModelError("DataViagem", "Data Invalida!");
            }
            if (viagemViewModel.DataViagem > DateTime.Now && (EnumStatusViagem)viagemViewModel.Status == EnumStatusViagem.Realizada)
            {
                ModelState.AddModelError("DataViagem", "Data Inválida para viagem realizada!");
            }
            if (viagemViewModel.OrigemId <= 0)
            {
                ModelState.AddModelError("OrigemId", "Campo Obrigatório!");
            }
            if (viagemViewModel.DestinoId <= 0)
            {
                ModelState.AddModelError("DestinoId", "Campo Obrigatório!");
            }
            if (viagemViewModel.VeiculoId <= 0)
            {
                ModelState.AddModelError("VeiculoId", "Campo Obrigatório!");
            }
            if (viagemViewModel.Status <= 0)
            {
                ModelState.AddModelError("Status", "Campo Obrigatório!");
            }
            if (viagemViewModel.Valor != null)
            {
                if (viagemViewModel.Valor <= 0)
                {
                    ModelState.AddModelError("Valor", "Valor deve ser maior que 0!");
                }
            }
            else
            {
                ModelState.ClearValidationState("Valor");
                ModelState.AddModelError("Valor", "Campo Obrigatório!");
            }

            if (ModelState.ErrorCount != 0)
            {
                PreparaDadosView();
                return View("Form", viagemViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (inclusao)
                        _context.Add(viagemViewModel);
                    else
                        _context.Update(viagemViewModel);

                    _context.SaveChanges();

                    return RedirectToAction("Index", "Viagem");
                }
                catch (Exception erro)
                {
                    _logger.LogError(erro.ToString());
                }
            }
            else
            {
                PreparaDadosView();
                return View(viagemViewModel);
            }

            return View("Form", viagemViewModel);
        }

        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return Error();
            }

            var viagemViewModel = _context.Viagem.AsNoTracking().FirstOrDefault(m => m.Id == id);
            if (viagemViewModel == null)
            {
                return Error();
            }

            ViewBag.Operacao = "alteracao";
            viagemViewModel.ItensVeiculo = ListaItensVeiculo(viagemViewModel.VeiculoId);

            PreparaDadosView();

            return View("Form", viagemViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var viagemViewModel = _context.Viagem.AsNoTracking().FirstOrDefault(m => m.Id == id);
                if (viagemViewModel != null)
                {
                    _context.Remove(viagemViewModel);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index", "Viagem");
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                return Error();
            }
        }

        public IActionResult AtualizaViagemRealizada()
        {
            var listaViagens = _context.Viagem.AsNoTracking().Where(v => v.Status == (int)EnumStatusViagem.Pendente).ToList();

            if (listaViagens != null)
            {
                foreach (var viagem in listaViagens)
                {
                    if(viagem.DataViagem.Date <= DateTime.Now.Date)
                    {
                        if (viagem.DataViagem.Date == DateTime.Now.Date)
                        {
                            if (viagem.HoraChegada < DateTime.Now.TimeOfDay)
                            {
                                viagem.Status = (int)EnumStatusViagem.Realizada;
                                _context.Update(viagem);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            viagem.Status = (int)EnumStatusViagem.Realizada;
                            _context.Update(viagem);
                            _context.SaveChanges();
                        }
                    }
                }
            }

            var listaViagensAtualizada = (List<Viagem>)_repository.ListarViagem();

            return RedirectToAction("Index", listaViagensAtualizada);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void SalvaItemAdicional(string itens, int veiculoId)
        {
            var ItensAdicionais = new List<ItemAdicionalVeiculo>();
            var splitItens = itens.Split(",");

            foreach (var item in splitItens)
            {
                var itemAdicional = new ItemAdicionalVeiculo { ItemAdicionalId = Convert.ToInt32(item), VeiculoId = veiculoId };
                ItensAdicionais.Add(itemAdicional);
            }

            _repository.AdicionarListaItemAdicionalVeiculo(ItensAdicionais);
        }

        private void PreparaDadosView()
        {
            ListaVeiculos();
            ListaFuncionarios();
            ListaMunicipios();
            ListaStatus();
            ListaItensAdicionais();
        }

        private void ListaVeiculos()
        {
            var veiculos = new List<SelectListItem>();

            foreach (var item in _repository.ListarVeiculo())
            {
                veiculos.Add(new SelectListItem { Text = item.Marca + "-" + item.Modelo, Value = item.Id.ToString() });
            }

            var listaVeiculosFinal = veiculos.OrderBy(e => e.Text).ToList();

            listaVeiculosFinal.Insert(0, new SelectListItem { Text = "Selecione", Value = "0" });

            ViewBag.Veiculos = listaVeiculosFinal;
        }

        private void ListaFuncionarios()
        {
            var funcionarios = new List<SelectListItem>();

            foreach (var item in _repository.ListarFuncionario())
            {
                funcionarios.Add(new SelectListItem { Text = item.Nome, Value = item.Id.ToString() });
            }

            var listaFuncFinal = funcionarios.OrderBy(e => e.Text).ToList();

            listaFuncFinal.Insert(0, new SelectListItem { Text = "Selecione", Value = "0" });

            ViewBag.Funcionarios = listaFuncFinal;
        }

        private void ListaMunicipios()
        {
            var municipios = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(typeof(EnumMunicipios)))
            {
                var descricao = GetDescription((EnumMunicipios)item);
                municipios.Add(new SelectListItem { Text = descricao, Value = (Convert.ToInt32(item)).ToString() });
            }

            var listaMunicipiosFinal = municipios.OrderBy(e => e.Text).ToList();

            listaMunicipiosFinal.Insert(0, new SelectListItem { Text = "Selecione", Value = "0" });

            ViewBag.Municipios = listaMunicipiosFinal;
        }

        private void ListaStatus()
        {
            var status = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(typeof(EnumStatusViagem)))
            {
                status.Add(new SelectListItem { Text = item.ToString(), Value = (Convert.ToInt32(item)).ToString() });
            }

            var listaStatusFinal = status.OrderBy(e => e.Text).ToList();

            listaStatusFinal.Insert(0, new SelectListItem { Text = "Selecione", Value = "0" });

            ViewBag.Status = listaStatusFinal;
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

            if(queryItens != null)
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
