﻿using Microsoft.EntityFrameworkCore;
using NFeFacil.ItensBD;
using NFeFacil.Log;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewRegistroVenda
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class ManipulacaoRegistroVenda : Page
    {
        RegistroVenda ItemBanco { get; set; } = new RegistroVenda();

        ObservableCollection<ExibicaoProdutoVenda> ListaProdutos { get; set; }
        ObservableCollection<ClienteDI> Clientes { get; set; }
        ObservableCollection<MotoristaDI> Motoristas { get; set; }

        void AtualizarTotal()
        {
            ItemBanco.DescontoTotal = ItemBanco.Produtos.Sum(x => x.Desconto);

            txtTotalBruto.Text = ItemBanco.Produtos.Sum(x => x.Quantidade * x.ValorUnitario).ToString("C");
            txtAdicionais.Text = ItemBanco.Produtos.Sum(x => x.Seguro + x.Frete + x.DespesasExtras).ToString("C");
            txtDescontoTotal.Text = ItemBanco.DescontoTotal.ToString("C");
            txtTotal.Text = ItemBanco.Produtos.Sum(x => x.TotalLíquido).ToString("C");
        }

        ClienteDI Cliente
        {
            get => Clientes.FirstOrDefault(x => x.Id == ItemBanco.Cliente);
            set
            {
                ItemBanco.Cliente = value.Id;
                if (!string.IsNullOrEmpty(value.CNPJ))
                {
                    DefinirComprador(value);
                }
            }
        }

        async void DefinirComprador(ClienteDI client)
        {
            using (var db = new AplicativoContext())
            {
                var compradores = db.Compradores.Where(x => x.Ativo && x.IdEmpresa == client.Id);
                var nomes = compradores.Select(x => x.Nome);
                var caixa = new DefinirComprador(nomes);
                if (await caixa.ShowAsync() == ContentDialogResult.Primary)
                {
                    var escolhido = compradores.First(x => x.Nome == caixa.Escolhido);
                    ItemBanco.Comprador = escolhido.Id;
                }
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

        DateTimeOffset PrazoEntrega
        {
            get => ItemBanco.PrazoEntrega;
            set => ItemBanco.PrazoEntrega = value.DateTime;
        }

        public ManipulacaoRegistroVenda()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            using (var db = new AplicativoContext())
            {
                Clientes = db.Clientes.Where(x => x.Ativo).OrderBy(x => x.Nome).GerarObs();
                Motoristas = db.Motoristas.Where(x => x.Ativo).OrderBy(x => x.Nome).GerarObs();

                ItemBanco = new RegistroVenda
                {
                    Emitente = Propriedades.EmitenteAtivo.Id,
                    Vendedor = Propriedades.VendedorAtivo?.Id ?? Guid.Empty,
                    Produtos = new System.Collections.Generic.List<ProdutoSimplesVenda>(),
                    DataHoraVenda = Propriedades.DateTimeNow,
                    PrazoEntrega = Propriedades.DateTimeNow
                };
                ListaProdutos = new ObservableCollection<ExibicaoProdutoVenda>();
                AtualizarTotal();
            }
        }

        private void RemoverProduto(object sender, RoutedEventArgs e)
        {
            var prod = (ExibicaoProdutoVenda)((FrameworkElement)sender).DataContext;
            ListaProdutos.Remove(prod);
            ItemBanco.Produtos.Remove(prod.Base);
        }

        private async void AdicionarProduto(object sender, RoutedEventArgs e)
        {
            var caixa = new AdicionarProduto(ListaProdutos.Select(x => x.Base.IdBase).ToArray());
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                var novoProdBanco = new ProdutoSimplesVenda
                {
                    IdBase = caixa.ProdutoSelecionado.Base.Id,
                    ValorUnitario = caixa.ProdutoSelecionado.PrecoDouble,
                    Quantidade = caixa.Quantidade,
                    Frete = 0,
                    Seguro = caixa.Seguro,
                    DespesasExtras = caixa.DespesasExtras
                };
                novoProdBanco.CalcularTotalLíquido();
                var novoProdExib = new ExibicaoProdutoVenda
                {
                    Base = novoProdBanco,
                    Descricao = caixa.ProdutoSelecionado.Nome,
                    Quantidade = novoProdBanco.Quantidade,
                };
                ListaProdutos.Add(novoProdExib);
                ItemBanco.Produtos.Add(novoProdBanco);
                AtualizarTotal();
            }
        }

        private void Finalizar(object sender, RoutedEventArgs e)
        {
            if (ItemBanco.Cliente == default(Guid))
            {
                Popup.Current.Escrever(TitulosComuns.Atenção, "Escolha primeiro um cliente.");
            }
            else
            {
                var produtosOrignal = ItemBanco.Produtos;
                using (var db = new AplicativoContext())
                {
                    ItemBanco.UltimaData = Propriedades.DateTimeNow;
                    ItemBanco.Produtos = null;
                    db.Vendas.Add(ItemBanco);
                    db.SaveChanges();

                    for (int i = 0; i < produtosOrignal.Count; i++)
                    {
                        var produto = produtosOrignal[i];
                        var estoque = db.Estoque.Include(x => x.Alteracoes).FirstOrDefault(x => x.Id == produto.IdBase);
                        if (estoque != null)
                        {
                            estoque.UltimaData = Propriedades.DateTimeNow;
                            estoque.Alteracoes.Add(new AlteracaoEstoque
                            {
                                Alteração = produto.Quantidade * -1,
                                MomentoRegistro = Propriedades.DateTimeNow
                            });

                            db.Estoque.Update(estoque);
                        }
                    }
                    db.SaveChanges();
                }

                using (var db = new AplicativoContext())
                {
                    ItemBanco.Produtos = produtosOrignal;
                    db.Vendas.Update(ItemBanco);
                    db.SaveChanges();

                    var log = Popup.Current;
                    log.Escrever(TitulosComuns.Sucesso, "Registro de venda salvo com sucesso.");
                }

                var ultPage = Frame.BackStack[Frame.BackStack.Count - 1];
                PageStackEntry entrada = new PageStackEntry(typeof(VisualizacaoRegistroVenda), ItemBanco, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
                Frame.BackStack.Add(entrada);
                MainPage.Current.Retornar(true);
            }
        }

        private async void AplicarDesconto(object sender, RoutedEventArgs e)
        {
            var caixa = new CalculoDesconto(ItemBanco.Produtos);
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                var prods = caixa.Produtos;
                for (int i = 0; i < prods.Count; i++)
                {
                    var atual = prods[i];
                    atual.CalcularTotalLíquido();
                    var antigo = ListaProdutos[i];
                    antigo.Base = atual;
                    ListaProdutos[i] = antigo;
                    ItemBanco.Produtos[i] = antigo.Base;
                }
                ItemBanco.DescontoTotal = prods.Sum(x => x.Desconto);
                AtualizarTotal();
            }
        }

        async void AplicarFrete(object sender, RoutedEventArgs e)
        {
            var caixa = new AplicarFrete();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                for (int i = 0; i < ListaProdutos.Count; i++)
                {
                    var atual = ListaProdutos[i];
                    atual.Base.Frete = caixa.Valor / ListaProdutos.Count;
                    atual.Base.CalcularTotalLíquido();
                    ListaProdutos[i] = atual;
                    ItemBanco.Produtos[i] = atual.Base;
                }
                AtualizarTotal();

                ItemBanco.TipoFrete = caixa.TipoFrete;
            }
        }

        async void DefinirPagamento(object sender, RoutedEventArgs e)
        {
            var caixa = new InfoPagamento();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                ItemBanco.PrazoPagamento = caixa.Prazo.ToString("dd/MM/yyyy");
                ItemBanco.FormaPagamento = caixa.FormaPagamento;
            }
            else
            {
                var input = (AppBarToggleButton)sender;
                input.IsChecked = false;
            }
        }

        void RemoverPagamento(object sender, RoutedEventArgs e)
        {
            ItemBanco.PrazoPagamento = null;
            ItemBanco.FormaPagamento = null;
        }

        async void DefinirDataVenda(object sender, RoutedEventArgs e)
        {
            var caixa = new DefinirDataVenda();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                ItemBanco.DataHoraVenda = caixa.Data.DateTime;
            }
        }
    }

    struct ExibicaoProdutoVenda
    {
        public ProdutoSimplesVenda Base { get; set; }
        public string Descricao { get; set; }
        public double Quantidade { get; set; }
        public string TotalLiquido => Base.TotalLíquido.ToString("C");
    }
}