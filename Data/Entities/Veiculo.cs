using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Veiculo : BaseEntity
    {
        [Display(Name = "Marca")]
        public string Marca { get; set; }

        [Display(Name = "Modelo")]
        public string Modelo { get; set; }

        public List<ItemAdicionalVeiculo> ItensAdicionais { get; set; }

        public List<Viagem> Viagens { get; set; }

        [NotMapped]
        public string ItensVeiculo;
    }
}
