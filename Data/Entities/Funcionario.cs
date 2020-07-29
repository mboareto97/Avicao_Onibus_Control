using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities
{
    public class Funcionario : BaseEntity
    {
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        public List<Viagem> Viagens { get; set; }
    }
}
