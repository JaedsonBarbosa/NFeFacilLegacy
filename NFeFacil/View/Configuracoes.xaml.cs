﻿using BaseGeral;
using BaseGeral.Sincronizacao;
using BaseGeral.View;
using RegistroComum.CondicaoPagamento;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    [DetalhePagina(Symbol.Setting, "Configurações")]
    public sealed partial class Configuracoes : Page, INotifyPropertyChanged
    {
        public Configuracoes()
        {
            InitializeComponent();
            AnalisarCompras();
            CarregarCondicoesPagamento();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DefinicoesPermanentes.ConfiguracoesEstoque.SalvarModificacoes();
        }

        int FuncaoSincronizacao
        {
            get => (int)ConfiguracoesSincronizacao.Tipo;
            set => ConfiguracoesSincronizacao.Tipo = (TipoAppSincronizacao)value;
        }

        Visibility PacotePersonalizacaoComprado { get; set; }
        bool FluentDesign
        {
            get => DefinicoesPermanentes.UsarFluent;
            set => DefinicoesPermanentes.UsarFluent = value;
        }

        async void UsarImagem(object sender, RoutedEventArgs e)
        {
            var brushAtual = MainPage.Current.ImagemBackground;
            if (DefinicoesPermanentes.IDBackgroung == default(Guid))
            {
                DefinicoesPermanentes.IDBackgroung = Guid.NewGuid();
            }
            var caixa = new DefinirImagem(DefinicoesPermanentes.IDBackgroung, brushAtual);
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                MainPage.Current.ImagemBackground = caixa.Imagem;
                MainPage.Current.DefinirTipoBackground(TiposBackground.Imagem);
            }
        }

        void UsarCor(object sender, RoutedEventArgs e)
        {
            MainPage.Current.DefinirTipoBackground(TiposBackground.Cor);
        }

        double OpacidadeBackground
        {
            get => DefinicoesPermanentes.OpacidadeBackground;
            set => MainPage.Current.DefinirOpacidadeBackground(value);
        }

        void Resetar(object sender, RoutedEventArgs e)
        {
            DefinicoesPermanentes.OpacidadeBackground = 1;
            MainPage.Current.DefinirTipoBackground(TiposBackground.Padrao);
        }

        async void SalvarBD(object sender, RoutedEventArgs e)
        {
            try
            {
                var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                var file = files[0];

                var caixa = new FileSavePicker();
                caixa.FileTypeChoices.Add("Arquivo BD", new string[] { ".db" });
                var arq = await caixa.PickSaveFileAsync();
                await file.CopyAndReplaceAsync(arq);
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        void AnalisarCompras()
        {
            var comprado = ComprasInApp.Resumo[Compras.NFCe];
            btnComprarNFCe.IsEnabled = !comprado;
            comprado = ComprasInApp.Resumo[Compras.Personalizacao];
            btnComprarBackground.IsEnabled = !comprado;
            PacotePersonalizacaoComprado = comprado ? Visibility.Visible : Visibility.Collapsed;
            comprado = ComprasInApp.Resumo[Compras.RelatorioProdutos01];
            btnComprarRelatorioProduto01.IsEnabled = !comprado;
        }

        async void ComprarNFCe(object sender, RoutedEventArgs e)
        {
            var comprado = await ComprasInApp.Comprar(Compras.NFCe);
            btnComprarNFCe.IsEnabled = !comprado;
        }

        async void ComprarBackground(object sender, RoutedEventArgs e)
        {
            var comprado = await ComprasInApp.Comprar(Compras.Personalizacao);
            btnComprarBackground.IsEnabled = !comprado;
            PacotePersonalizacaoComprado = comprado ? Visibility.Visible : Visibility.Collapsed;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacotePersonalizacaoComprado)));
        }

        async void ComprarRelatorioProduto01(object sender, RoutedEventArgs e)
        {
            var comprado = await ComprasInApp.Comprar(Compras.RelatorioProdutos01);
            btnComprarRelatorioProduto01.IsEnabled = !comprado;
        }

        async void ReanalizarCompras(object sender, RoutedEventArgs e)
        {
            await ComprasInApp.AnalisarCompras();
            AnalisarCompras();
        }

        ObservableCollection<string> CondicoesPagamento { get; set; }

        async void CarregarCondicoesPagamento()
        {
            CondicoesPagamento = new ObservableCollection<string>();
            var condicoes = await GerenciadorCondicaoPagamento.Obter();
            foreach (var item in condicoes)
                CondicoesPagamento.Add(item);
        }

        async void AdicionarCondPagamento(object sender, RoutedEventArgs e)
        {
            var caixa = new AddCondPagamento();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                CondicoesPagamento.Add(caixa.Condicao);
                await GerenciadorCondicaoPagamento.Salvar(CondicoesPagamento);
            }
        }

        async void RemoverCondPagamento(object sender, RoutedEventArgs e)
        {
            var condicao = (string)((MenuFlyoutItem)e.OriginalSource).DataContext;
            CondicoesPagamento.Remove(condicao);
            await GerenciadorCondicaoPagamento.Salvar(CondicoesPagamento);
        }
    }
}
