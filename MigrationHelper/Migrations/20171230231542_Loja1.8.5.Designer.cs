﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BaseGeral;

namespace BaseGeral.Migrations
{
    [DbContext(typeof(AplicativoContext))]
    [Migration("20171230231542_Loja1.8.5")]
    partial class Loja185
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("BaseGeral.ItensBD.AlteracaoEstoque", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Alteração");

                    b.Property<Guid?>("EstoqueId");

                    b.Property<DateTime>("MomentoRegistro");

                    b.HasKey("Id");

                    b.HasIndex("EstoqueId");

                    b.ToTable("AlteracaoEstoque");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.CancelamentoRegistroVenda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("MomentoCancelamento");

                    b.Property<string>("Motivo");

                    b.HasKey("Id");

                    b.ToTable("CancelamentosRegistroVenda");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.ClienteDI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<string>("Bairro");

                    b.Property<string>("CEP");

                    b.Property<string>("CNPJ");

                    b.Property<string>("CPF");

                    b.Property<int>("CPais");

                    b.Property<int>("CodigoMunicipio");

                    b.Property<string>("Complemento");

                    b.Property<string>("Email");

                    b.Property<string>("ISUF");

                    b.Property<string>("IdEstrangeiro");

                    b.Property<int>("IndicadorIE");

                    b.Property<string>("InscricaoEstadual");

                    b.Property<string>("Logradouro");

                    b.Property<string>("Nome");

                    b.Property<string>("NomeMunicipio");

                    b.Property<string>("Numero");

                    b.Property<string>("SiglaUF");

                    b.Property<string>("Telefone");

                    b.Property<DateTime>("UltimaData");

                    b.Property<string>("XPais");

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.Comprador", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<string>("Email");

                    b.Property<Guid>("IdEmpresa");

                    b.Property<string>("Nome");

                    b.Property<string>("Telefone");

                    b.Property<DateTime>("UltimaData");

                    b.HasKey("Id");

                    b.ToTable("Compradores");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.EmitenteDI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Bairro");

                    b.Property<string>("CEP");

                    b.Property<string>("CNAE");

                    b.Property<string>("CNPJ");

                    b.Property<int>("CPais");

                    b.Property<int>("CodigoMunicipio");

                    b.Property<string>("Complemento");

                    b.Property<string>("Email");

                    b.Property<string>("IEST");

                    b.Property<string>("IM");

                    b.Property<string>("InscricaoEstadual");

                    b.Property<string>("Logradouro");

                    b.Property<string>("Nome");

                    b.Property<string>("NomeFantasia");

                    b.Property<string>("NomeMunicipio");

                    b.Property<string>("Numero");

                    b.Property<int>("RegimeTributario");

                    b.Property<string>("SiglaUF");

                    b.Property<string>("Telefone");

                    b.Property<DateTime>("UltimaData");

                    b.Property<string>("XPais");

                    b.HasKey("Id");

                    b.ToTable("Emitentes");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.Estoque", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LocalizacaoGenerica");

                    b.Property<string>("Locação");

                    b.Property<string>("Prateleira");

                    b.Property<string>("Segmento");

                    b.Property<DateTime>("UltimaData");

                    b.HasKey("Id");

                    b.ToTable("Estoque");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.Imagem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Bytes");

                    b.Property<DateTime>("UltimaData");

                    b.HasKey("Id");

                    b.ToTable("Imagens");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.Inutilizacao", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CNPJ");

                    b.Property<int>("FimRange");

                    b.Property<bool>("Homologacao");

                    b.Property<int>("InicioRange");

                    b.Property<DateTime>("MomentoProcessamento");

                    b.Property<long>("NumeroProtocolo");

                    b.Property<int>("Serie");

                    b.Property<string>("XMLCompleto");

                    b.HasKey("Id");

                    b.ToTable("Inutilizacoes");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.MotoristaDI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<string>("CNPJ");

                    b.Property<string>("CPF");

                    b.Property<string>("Email");

                    b.Property<string>("InscricaoEstadual");

                    b.Property<string>("Nome");

                    b.Property<string>("Telefone");

                    b.Property<string>("UF");

                    b.Property<DateTime>("UltimaData");

                    b.Property<Guid>("Veiculo");

                    b.Property<string>("VeiculosSecundarios");

                    b.Property<string>("XEnder");

                    b.Property<string>("XMun");

                    b.HasKey("Id");

                    b.ToTable("Motoristas");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.NFeDI", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CNPJEmitente")
                        .IsRequired();

                    b.Property<string>("DataEmissao")
                        .IsRequired();

                    b.Property<bool>("Exportada");

                    b.Property<bool>("Impressa");

                    b.Property<string>("NomeCliente")
                        .IsRequired();

                    b.Property<string>("NomeEmitente")
                        .IsRequired();

                    b.Property<int>("NumeroNota");

                    b.Property<ushort>("SerieNota");

                    b.Property<int>("Status");

                    b.Property<DateTime>("UltimaData");

                    b.Property<string>("XML")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("NotasFiscais");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.ProdutoDI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<string>("CEST");

                    b.Property<string>("CFOP");

                    b.Property<string>("CodigoBarras");

                    b.Property<string>("CodigoBarrasTributo");

                    b.Property<string>("CodigoProduto");

                    b.Property<string>("Descricao");

                    b.Property<string>("EXTIPI");

                    b.Property<string>("ICMS");

                    b.Property<string>("ImpostosSimples");

                    b.Property<string>("NCM");

                    b.Property<string>("ProdutoEspecial");

                    b.Property<DateTime>("UltimaData");

                    b.Property<string>("UnidadeComercializacao");

                    b.Property<string>("UnidadeTributacao");

                    b.Property<double>("ValorUnitario");

                    b.Property<double>("ValorUnitarioTributo");

                    b.HasKey("Id");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.ProdutoSimplesVenda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Desconto");

                    b.Property<double>("DespesasExtras");

                    b.Property<double>("Frete");

                    b.Property<Guid>("IdBase");

                    b.Property<double>("Quantidade");

                    b.Property<Guid?>("RegistroVendaId");

                    b.Property<double>("Seguro");

                    b.Property<double>("TotalLíquido");

                    b.Property<double>("ValorUnitario");

                    b.HasKey("Id");

                    b.HasIndex("RegistroVendaId");

                    b.ToTable("ProdutoSimplesVenda");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.RegistroCancelamento", b =>
                {
                    b.Property<string>("ChaveNFe")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataHoraEvento");

                    b.Property<int>("TipoAmbiente");

                    b.Property<string>("XML");

                    b.HasKey("ChaveNFe");

                    b.ToTable("Cancelamentos");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.RegistroVenda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Cancelado");

                    b.Property<Guid>("Cliente");

                    b.Property<Guid>("Comprador");

                    b.Property<DateTime>("DataHoraVenda");

                    b.Property<double>("DescontoTotal");

                    b.Property<Guid>("Emitente");

                    b.Property<string>("FormaPagamento");

                    b.Property<Guid>("Motorista");

                    b.Property<string>("NotaFiscalRelacionada");

                    b.Property<string>("Observações");

                    b.Property<DateTime>("PrazoEntrega");

                    b.Property<string>("PrazoPagamento");

                    b.Property<string>("TipoFrete");

                    b.Property<DateTime>("UltimaData");

                    b.Property<Guid>("Vendedor");

                    b.HasKey("Id");

                    b.ToTable("Vendas");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.VeiculoDI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<string>("Descricao");

                    b.Property<string>("Placa");

                    b.Property<string>("RNTC");

                    b.Property<string>("UF");

                    b.Property<DateTime>("UltimaData");

                    b.HasKey("Id");

                    b.ToTable("Veiculos");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.Vendedor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Ativo");

                    b.Property<long>("CPF");

                    b.Property<string>("CPFStr");

                    b.Property<string>("Endereço")
                        .IsRequired();

                    b.Property<string>("Nome")
                        .IsRequired();

                    b.Property<DateTime>("UltimaData");

                    b.HasKey("Id");

                    b.ToTable("Vendedores");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.AlteracaoEstoque", b =>
                {
                    b.HasOne("BaseGeral.ItensBD.Estoque")
                        .WithMany("Alteracoes")
                        .HasForeignKey("EstoqueId");
                });

            modelBuilder.Entity("BaseGeral.ItensBD.ProdutoSimplesVenda", b =>
                {
                    b.HasOne("BaseGeral.ItensBD.RegistroVenda")
                        .WithMany("Produtos")
                        .HasForeignKey("RegistroVendaId");
                });
        }
    }
}
