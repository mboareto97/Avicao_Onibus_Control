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
    public class FuncionarioController : Controller
    {
        private readonly ILogger<FuncionarioController> _logger;
        private readonly IRepository _repository;
        private readonly BaseContext _context;

        public FuncionarioController(ILogger<FuncionarioController> logger, IRepository repository, BaseContext context)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
        }

        public IActionResult Index(int? origemId = null, int? destinoId = null)
        {
            var funcionarios = new List<Funcionario>();
            var listaFuncionarios = _repository.ListarFuncionario();

            if (origemId > 0 && destinoId > 0)
            {
                foreach (var func in listaFuncionarios)
                {
                    var queryFunc = (from viagem in _context.Viagem
                                where viagem.OrigemId == origemId &&
                                      viagem.DestinoId == destinoId &&
                                      viagem.FuncionarioId == func.Id
                                select viagem.FuncionarioId).FirstOrDefault();

                    if (queryFunc > 0)
                    {
                        var funcionario = _context.Funcionario.AsNoTracking().FirstOrDefault(f => f.Id == queryFunc);

                        funcionarios.Add(funcionario);
                    }
                }                 
            }
            else
                funcionarios = (List<Funcionario>)listaFuncionarios;

            ListaMunicipios();

            return View(funcionarios);
        }

        public IActionResult Create()
        {
            return View("Form", new Funcionario());
        }

        public IActionResult Edit(Funcionario funcionarioViewModel, int id, string operacao)
        {
            bool inclusao = operacao == "inclusao";

            ViewBag.Operacao = operacao;

            if (!inclusao)
            {
                if (id != funcionarioViewModel.Id)
                {
                    return Error();
                }
            }

            if (string.IsNullOrEmpty(funcionarioViewModel.Nome))
            {
                ModelState.AddModelError("Nome", "Campo Obrigatorio!");
            }

            if (ModelState.ErrorCount != 0)
            {
                return View("Form", funcionarioViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (inclusao)
                        _context.Add(funcionarioViewModel);
                    else
                        _context.Update(funcionarioViewModel);

                    _context.SaveChanges();

                    return RedirectToAction("Index", "Funcionario");
                }
                catch (Exception erro)
                {
                    _logger.LogError(erro.ToString());
                }
            }
            else
            {
                return View(funcionarioViewModel);
            }

            return View("Form", funcionarioViewModel);
        }

        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return Error();
            }

            var funcionarioViewModel = _context.Funcionario.AsNoTracking().FirstOrDefault(m => m.Id == id);
            if (funcionarioViewModel == null)
            {
                return Error();
            }

            ViewBag.Operacao = "alteracao";

            return View("Form", funcionarioViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var funcionarioViewModel = _context.Funcionario.AsNoTracking().FirstOrDefault(m => m.Id == id);
                if (funcionarioViewModel != null)
                {
                    _context.Remove(funcionarioViewModel);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index", "Funcionario");
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
