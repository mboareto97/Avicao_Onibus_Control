using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public class Repository : IRepository
    {
        private readonly DbContextOptionsBuilder<BaseContext> _optionsBuilder;

        public Repository()
        {
            _optionsBuilder = new DbContextOptionsBuilder<BaseContext>();
        }

        public void AdicionarListaFuncionario(List<Funcionario> funcionario)
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                context.Funcionario.AddRange(funcionario);
                context.SaveChanges();
            }
        }

        public void AdicionarListaItemAdicionalVeiculo(List<ItemAdicionalVeiculo> itemAdicionalVeiculo)
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                context.ItemAdicionalVeiculo.AddRange(itemAdicionalVeiculo);
                context.SaveChanges();
            }
        }

        public void AdicionarListaVeiculo(List<Veiculo> veiculo)
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                context.Veiculo.AddRange(veiculo);
                context.SaveChanges();
            }
        }

        public void AdicionarListaViagem(List<Viagem> viagem)
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                context.Viagem.AddRange(viagem);
                context.SaveChanges();
            }
        }

        public IList<Funcionario> ListarFuncionario()
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                return context.Funcionario.ToList();
            }
        }

        public IList<Veiculo> ListarVeiculo()
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                return context.Veiculo.ToList();
            }
        }

        public IList<Viagem> ListarViagem()
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                return context.Viagem.ToList();
            }
        }

        public IList<ItemAdicionalVeiculo> ListItemAdicionalVeiculo()
        {
            using (var context = new BaseContext(_optionsBuilder.Options))
            {
                return context.ItemAdicionalVeiculo.ToList();
            }
        }
    }
}
