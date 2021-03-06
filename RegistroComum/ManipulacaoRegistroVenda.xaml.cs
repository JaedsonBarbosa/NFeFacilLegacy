﻿using BaseGeral;
using BaseGeral.Controles;
using BaseGeral.ItensBD;
using BaseGeral.Log;
using BaseGeral.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace RegistroComum
{
    [DetalhePagina("\uEC59", "Registro de venda")]
    public sealed partial class ManipulacaoRegistroVenda : Page
    {
        RegistroVenda ItemBanco { get; set; }
        Visibility CondicoesPagamentoVisiveis;
        ObservableCollection<string> CondicoesPagamento;
        ObservableCollection<Comprador> Compradores { get; set; }
        Dictionary<Guid, Comprador[]> CompradoresPorCliente;

        void AtualizarTotal()
        {
            ItemBanco.DescontoTotal = ItemBanco.Produtos.Sum(x => x.Desconto);

            txtTotalAdicionais.Text = ItemBanco.Produtos.Sum(x => x.DespesasExtras).ToString("C");
            txtTotalDesconto.Text = ItemBanco.DescontoTotal.ToString("C");
            txtTotalFrete.Text = ItemBanco.Produtos.Sum(x => x.Frete).ToString("C");
            txtTotalLiquido.Text = ItemBanco.Produtos.Sum(x => x.TotalLíquido).ToString("C");
            txtTotalSeguro.Text = ItemBanco.Produtos.Sum(x => x.Seguro).ToString("C");
        }

        Guid Cliente
        {
            get => ItemBanco.Cliente;
            set
            {
                ItemBanco.Cliente = value;
                ItemBanco.Comprador = default(Guid);
                var compradores = CompradoresPorCliente[value];
                if (compradores?.Length > 0)
                {
                    Compradores.Clear();
                    for (int i = 0; i < compradores.Length; i++)
                    {
                        Compradores.Add(compradores[i]);
                    }
                }
                cmbComprador.IsEnabled = compradores?.Length > 0;
            }
        }

        Guid Motorista
        {
            get => ItemBanco.Motorista;
            set => ItemBanco.Motorista = value;
        }

        string Observacoes
        {
            get => ItemBanco.Observações;
            set => ItemBanco.Observações = value;
        }

        internal double ValorFrete
        {
            get => ItemBanco.Produtos.Sum(x => x.Frete);
            set
            {
                for (int i = 0; i < ItemBanco.Produtos.Count; i++)
                {
                    var atual = ItemBanco.Produtos[i];
                    atual.Frete = value / ItemBanco.Produtos.Count;
                    atual.CalcularTotalLíquido();
                    ItemBanco.Produtos[i] = atual;
                }
                AtualizarTotal();
            }
        }
        internal string TipoFrete
        {
            get => ItemBanco.TipoFrete;
            set => ItemBanco.TipoFrete = value;
        }

        DateTimeOffset PrazoEntrega
        {
            get => ItemBanco.PrazoEntrega;
            set => ItemBanco.PrazoEntrega = value.DateTime;
        }

        internal DateTimeOffset PrazoPagamento
        {
            get => ItemBanco.PrazoPagamento == null ? PrazoPagamento = DefinicoesTemporarias.DateTimeOffsetNow : DateTimeOffset.Parse(ItemBanco.PrazoPagamento);
            set => ItemBanco.PrazoPagamento = value.ToString("dd/MM/yyyy");
        }

        string FormaPagamento
        {
            get => ItemBanco.FormaPagamento;
            set => ItemBanco.FormaPagamento = value;
        }

        DateTimeOffset DataHoraVenda
        {
            get => ItemBanco.DataHoraVenda;
            set => ItemBanco.DataHoraVenda = value.DateTime;
        }

        public ManipulacaoRegistroVenda()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            using (var repo = new BaseGeral.Repositorio.Leitura())
            {
                Compradores = new ObservableCollection<Comprador>();
                CompradoresPorCliente = repo.ObterCompradoresPorCliente();
            }
            ItemBanco = (RegistroVenda)e.Parameter;
            AtualizarTotal();
            txtValorDesejado.Number = ItemBanco.Produtos.Sum(x => x.Quantidade * x.ValorUnitario);
            cmbComprador.IsEnabled = ItemBanco.Comprador != default(Guid);
            sldDesconto.Value = ItemBanco.DescontoTotal * 100 / ItemBanco.Produtos.Sum(x => x.ValorUnitario * x.Quantidade);
            CondicoesPagamento = new ObservableCollection<string>();
            var condicoes = await CondicaoPagamento.GerenciadorCondicaoPagamento.Obter();
            foreach (var item in condicoes)
                CondicoesPagamento.Add(item);
            CondicoesPagamentoVisiveis = CondicoesPagamento.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Finalizar(object sender, RoutedEventArgs e)
        {
            if (ItemBanco.Cliente == default(Guid))
            {
                Popup.Current.Escrever(TitulosComuns.Atenção, "Escolha primeiro um cliente.");
            }
            else if (ItemBanco.Produtos.Count == 0)
            {
                Popup.Current.Escrever(TitulosComuns.Atenção, "Não é possível salvar um registro de venda que não tenha nenhum produto.");
            }
            else
            {
                using (var repo = new BaseGeral.Repositorio.Escrita())
                {
                    repo.SalvarRV(ItemBanco, DefinicoesTemporarias.DateTimeNow);
                }

                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                if (Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType == typeof(VisualizacaoRegistroVenda))
                    Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                PageStackEntry entrada = new PageStackEntry(typeof(VisualizacaoRegistroVenda), ItemBanco, null);
                Frame.BackStack.Add(entrada);

                BasicMainPage.Current.Retornar();
            }
        }

        private void ValorDesejadoChanged(EntradaNumerica sender, NumeroChangedEventArgs e)
        {
            var totalOriginal = ItemBanco.Produtos.Sum(x => x.Quantidade * x.ValorUnitario);
            var porcentagemDesejada = e.NovoNumero / totalOriginal;
            for (int i = 0; i < ItemBanco.Produtos.Count; i++)
            {
                var atual = ItemBanco.Produtos[i];
                var valorOriginal = atual.ValorUnitario * atual.Quantidade;
                var desconto = valorOriginal * porcentagemDesejada;
                atual.Desconto = desconto;
                atual.CalcularTotalLíquido();
            }
            var porcentagem = 100 - (porcentagemDesejada * 100);
            sldDesconto.Value = porcentagem;

            ItemBanco.DescontoTotal = ItemBanco.Produtos.Sum(x => x.Desconto);
            AtualizarTotal();
        }

        private void sldDesconto_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            for (int i = 0; i < ItemBanco.Produtos.Count; i++)
            {
                var atual = ItemBanco.Produtos[i];
                var valorOriginal = atual.ValorUnitario * atual.Quantidade;
                var porcentagemUsada = e.NewValue / 100;
                var desconto = valorOriginal * porcentagemUsada;
                atual.Desconto = desconto;
                atual.CalcularTotalLíquido();
            }
            var valorDesejado = ItemBanco.Produtos.Sum(x => x.Quantidade * x.ValorUnitario - x.Desconto);
            txtValorDesejado.Number = valorDesejado;

            ItemBanco.DescontoTotal = ItemBanco.Produtos.Sum(x => x.Desconto);
            AtualizarTotal();
        }

        void Voltar(object sender, RoutedEventArgs e)
        {
            var ultPage = Frame.BackStack[Frame.BackStack.Count - 1];
            var controle = (ControleViewProduto)ultPage.Parameter;
            controle.AtualizarControle(ItemBanco);
            controle.Voltar();
        }
    }
}
