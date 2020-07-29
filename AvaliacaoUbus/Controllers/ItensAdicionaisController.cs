using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AvaliacaoUbus.Enums;
using AvaliacaoUbus.Metodos;
using Data.Context;
using Data.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace AvaliacaoUbus.Controllers
{
    public class ItensAdicionaisController : Controller
    {
        private readonly ILogger<ItensAdicionaisController> _logger;
        private readonly IRepository _repository;
        private readonly BaseContext _context;

        public ItensAdicionaisController(ILogger<ItensAdicionaisController> logger, IRepository repository, BaseContext context)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
        }

        public IActionResult Index(int origemId = 0, int destinoId = 0)
        {
            var itens = new List<string>();

            var retornoAPi = APICall.ExecutaApiItensAdicionais(origemId, destinoId);

            itens = ListaItens(retornoAPi);

            ListaMunicipios();

            return View(itens);
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

        private List<string> ListaItens(List<int> itens)
        {
            var itensAdicioinais = new List<string>();

            if (itens.Count > 0)
            {
                var listItens = itens;

                foreach (var item in listItens)
                {
                    var descricao = GetDescription((EnumItensAdicionais)item);
                    itensAdicioinais.Add(descricao);
                }
            }
            else 
            {
                foreach (var item in Enum.GetValues(typeof(EnumItensAdicionais)))
                {
                    var descricao = GetDescription((EnumItensAdicionais)item);
                    itensAdicioinais.Add(descricao);
                }

            }

            return itensAdicioinais;
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
