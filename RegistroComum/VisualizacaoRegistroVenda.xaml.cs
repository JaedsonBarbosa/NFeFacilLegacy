﻿using Fiscal;
using BaseGeral.ItensBD;
using RegistroComum.DARV;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BaseGeral;
using BaseGeral.Log;
using BaseGeral.View;
using Comum;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace RegistroComum
{
    [DetalhePagina(Symbol.View, "Registro de venda")]
    public sealed partial class VisualizacaoRegistroVenda : Page
    {
        RegistroVenda ItemBanco;

        Visualizacao Registro { get; set; }

        public VisualizacaoRegistroVenda()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ItemBanco = (RegistroVenda)e.Parameter;
            ctrVisualizacao.Content = Registro = new Visualizacao(ItemBanco);
            var semNota = string.IsNullOrEmpty(ItemBanco.NotaFiscalRelacionada);
            btnCriarNFe.IsEnabled = semNota && !ItemBanco.Cancelado;
            btnVerNFe.IsEnabled = !semNota;
            btnEditar.IsEnabled = btnCriarDarv.IsEnabled = btnCancelar.IsEnabled =
                btnCalcularTroco.IsEnabled = !ItemBanco.Cancelado;
        }

        public sealed class Visualizacao
        {
            public string ID { get; set; }
            public string NFeRelacionada { get; set; }
            public string DataVenda { get; set; }
            public EmitenteDI Emitente { get; set; }
            public ClienteDI Cliente { get; set; }
            public Comprador Comprador { get; set; }
            public MotoristaDI Motorista { get; set; }
            public Vendedor Vendedor { get; set; }
            public ProdutoDI[] ProdutosCompletos { get; }
            public List<Produto> Produtos { get; set; }
            public CancelamentoRegistroVenda Cancelamento { get; set; }
            public string Observacoes { get; set; }

            public string TipoFrete { get; set; }
            public string PrazoEntrega { get; set; }
            public string PrazoPagamento { get; set; }
            public string FormaPagamento { get; set; }
            public string CondicaoPagamento { get; set; }

            public struct Produto
            {
                public string Descricao { get; set; }
                public string Quantidade { get; set; }
                public string TotalBruto { get; set; }
            }

            public Visualizacao(RegistroVenda venda)
            {
                ID = venda.Id.ToString().ToUpper();
                NFeRelacionada = venda.NotaFiscalRelacionada;
                DataVenda = venda.DataHoraVenda.ToString("dd-MM-yyyy");
                Emitente = DefinicoesTemporarias.EmitenteAtivo;

                TipoFrete = venda.TipoFrete;
                PrazoEntrega = venda.PrazoEntrega != default(DateTime) ? venda.PrazoEntrega.ToString("dd/MM/yyyy") : null;
                PrazoPagamento = venda.PrazoPagamento;
                FormaPagamento = venda.FormaPagamento;
                CondicaoPagamento = venda.CondicaoPagamento;
                Observacoes = venda.Observações;

                using (var repo = new BaseGeral.Repositorio.Leitura())
                {
                    Cliente = repo.ObterCliente(venda.Cliente);
                    Comprador = venda.Comprador != Guid.Empty ? repo.ObterComprador(venda.Comprador) : null;
                    Motorista = venda.Motorista != Guid.Empty ? repo.ObterMotorista(venda.Motorista) : null;
                    Vendedor = venda.Vendedor != Guid.Empty ? repo.ObterVendedor(venda.Vendedor) : null;
                    ProdutosCompletos = venda.Produtos.Select(x => repo.ObterProduto(x.IdBase)).ToArray();
                    if (venda.Cancelado)
                    {
                        Cancelamento = repo.ObterCRV(venda.Id);
                    }
                }

                Produtos = venda.Produtos.Select(x => new Produto
                {
                    Descricao = ProdutosCompletos.First(k => k.Id == x.IdBase).Descricao,
                    Quantidade = x.Quantidade.ToString("N2"),
                    TotalBruto = (x.Quantidade * x.ValorUnitario).ToString("N2")
                }).ToList();
            }
        }

        private async void CriarNFe(object sender, RoutedEventArgs e)
        {
            try
            {
                var controle = new ControleNFe(ItemBanco.ToNFe());
                var caixa = new Criador(controle);
                if (await caixa.ShowAsync() == ContentDialogResult.Primary)
                {
                    Popup.Current.Escrever(TitulosComuns.Atenção, "Os impostos dos produtos não são adicionados automaticamente, por favor, insira-os editando cada produto.");
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        async void CriarDARV(object sender, RoutedEventArgs e)
        {
            var caixa = new EscolherDimensão();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                double largura = caixa.Largura, altura = caixa.Predefinicao == 0 ? 0 : caixa.Altura;

                BasicMainPage.Current.Navegar<ViewDARV>(new DadosImpressaoDARV
                {
                    Venda = ItemBanco,
                    Dimensoes = new Dimensoes(largura, altura, 1),
                    Cliente = Registro.Cliente,
                    Motorista = Registro.Motorista,
                    Vendedor = Registro.Vendedor,
                    Comprador = Registro.Comprador,
                    ProdutosCompletos = Registro.ProdutosCompletos
                });
            }
        }

        private async void Cancelar(object sender, RoutedEventArgs e)
        {
            var caixa = new MotivoCancelamento();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                var cancelamento = new CancelamentoRegistroVenda()
                {
                    Motivo = caixa.Motivo,
                    MomentoCancelamento = DefinicoesTemporarias.DateTimeNow,
                    Id = ItemBanco.Id
                };
                using (var repo = new BaseGeral.Repositorio.Escrita())
                {
                    repo.CancelarRV(ItemBanco, cancelamento, DefinicoesTemporarias.DateTimeNow);
                }
                Registro.Cancelamento = cancelamento;
                ctrVisualizacao.Content = Registro;
                btnEditar.IsEnabled = btnCriarDarv.IsEnabled = btnCriarNFe.IsEnabled
                    = btnCancelar.IsEnabled = btnCalcularTroco.IsEnabled = false;
            }
        }

        private void VerNFe(object sender, RoutedEventArgs e)
        {
            using (var repo = new BaseGeral.Repositorio.Leitura())
            {
                var item = repo.ObterNota(ItemBanco.NotaFiscalRelacionada);
                var acoes = new AcoesNFe(item);
                BasicMainPage.Current.Navegar<Fiscal.Visualizacao>(acoes);
            }
        }

        async void CalcularTroco(object sender, RoutedEventArgs e)
        {
            var total = ItemBanco.Produtos.Sum(x => x.TotalLíquido);
            await new CalcularTroco(total).ShowAsync();
        }

        async void Editar(object sender, RoutedEventArgs e)
        {
            var caixa = new MotivoEdicaoRV();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                ItemBanco.MotivoEdicao = caixa.Motivo;
                var controle = new ControleViewProduto(ItemBanco);
                BasicMainPage.Current.Navegar<Venda.ViewProdutoVenda.ListaProdutos>(controle);
            }
        }
    }
}
