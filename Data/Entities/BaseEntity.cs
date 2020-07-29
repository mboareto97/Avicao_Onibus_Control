using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities
{
    public class BaseEntity
    {
        [Display(Name = "Sequencial")]
        public int Id { get; set; }
    }
}
