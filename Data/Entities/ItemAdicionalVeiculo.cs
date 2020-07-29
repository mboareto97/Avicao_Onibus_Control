using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities
{
    public class ItemAdicionalVeiculo : BaseEntity
    {
        [Display(Name = "Id do Item Adicional")]
        public int ItemAdicionalId { get; set; }

        [Display(Name = "Id do Veiculo")]
        public int VeiculoId { get; set; }

        public Veiculo Veiculo { get; set; }
    }
}
