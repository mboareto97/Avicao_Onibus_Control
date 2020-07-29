using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Data.Context
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public BaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Viagem> Viagem { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<ItemAdicionalVeiculo> ItemAdicionalVeiculo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Se nao estiver configurado no projeto IU
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(GetStringConnectionConfig());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemAdicionalVeiculo>()
                .HasOne(p => p.Veiculo)
                .WithMany(b => b.ItensAdicionais)
                .HasForeignKey(p => p.VeiculoId);

            modelBuilder.Entity<Viagem>()
                .HasOne(p => p.Veiculo)
                .WithMany(b => b.Viagens)
                .HasForeignKey(p => p.VeiculoId);

            modelBuilder.Entity<Viagem>()
                .HasOne(p => p.Funcionario)
                .WithMany(b => b.Viagens)
                .HasForeignKey(p => p.FuncionarioId);
        }

        private string GetStringConnectionConfig()
        {
            string strCon = @"Server=DESKTOP-CNSB4KK\SQLEXPRESS;Database=Avaliacao_ubus;Trusted_Connection=True;MultipleActiveResultSets=true";

            return strCon;
        }
    }
}
