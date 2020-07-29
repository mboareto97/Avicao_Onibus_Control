using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Viagem : BaseEntity
    {
        [Display(Name = "Id do Veículo")]
        public int VeiculoId { get; set; }

        [Display(Name = "Id do Funcionário")]
        public int FuncionarioId { get; set; }

        [Display(Name = "Id do Municipio de Origem")]
        public int OrigemId { get; set; }

        [Display(Name = "Id do Municipio de Destino")]
        public int DestinoId { get; set; }

        [Display(Name = "Horário de partida da Viagem")]
        public TimeSpan HoraPartida { get; set; }

        [Display(Name = "Horário de chegada da Viagem")]
        public TimeSpan HoraChegada { get; set; }

        [Display(Name = "Data da Viagem")]
        public DateTime DataViagem { get; set; }

        [Display(Name = "Posição do Veículo")]
        public int PosicaoVeiculo { get; set; }

        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        public Veiculo Veiculo { get; set; }
        public Funcionario Funcionario { get; set; }

        [NotMapped]
        public string OrigemDescricao;

        [NotMapped]
        public string DestinoDescricao;

        [NotMapped]
        public string NomeFuncionario;

        [NotMapped]
        public string ItensVeiculo;
    }
}
