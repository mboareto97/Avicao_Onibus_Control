using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IRepository
    {
        IList<Viagem> ListarViagem();
        IList<Veiculo> ListarVeiculo();
        IList<Funcionario> ListarFuncionario();
        IList<ItemAdicionalVeiculo> ListItemAdicionalVeiculo();

        void AdicionarListaViagem(List<Viagem> viagem);
        void AdicionarListaVeiculo(List<Veiculo> veiculo);
        void AdicionarListaFuncionario(List<Funcionario> funcionario);
        void AdicionarListaItemAdicionalVeiculo(List<ItemAdicionalVeiculo> funcionario);
    }
}
