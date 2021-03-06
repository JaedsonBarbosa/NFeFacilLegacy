﻿using Microsoft.EntityFrameworkCore;
using BaseGeral.ItensBD;

namespace BaseGeral
{
    public class AplicativoContext : DbContext
    {
        public DbSet<ClienteDI> Clientes { get; set; }
        public DbSet<EmitenteDI> Emitentes { get; set; }
        public DbSet<MotoristaDI> Motoristas { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<ProdutoDI> Produtos { get; set; }
        public DbSet<Estoque> Estoque { get; set; }
        public DbSet<VeiculoDI> Veiculos { get; set; }
        public DbSet<NFeDI> NotasFiscais { get; set; }
        public DbSet<RegistroVenda> Vendas { get; set; }
        public DbSet<RegistroCancelamento> Cancelamentos { get; set; }
        public DbSet<CancelamentoRegistroVenda> CancelamentosRegistroVenda { get; set; }
        public DbSet<Imagem> Imagens { get; set; }
        public DbSet<Comprador> Compradores { get; set; }
        public DbSet<Inutilizacao> Inutilizacoes { get; set; }
        public DbSet<FornecedorDI> Fornecedores { get; set; }
        public DbSet<CategoriaDI> Categorias { get; set; }

        static readonly string ArquivoBD = "informacoes";

        public AplicativoContext()
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={ArquivoBD}.db");
        }
    }
}
