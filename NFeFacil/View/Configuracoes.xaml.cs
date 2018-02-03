﻿using NFeFacil.Certificacao;
using NFeFacil.Sincronizacao;
using System;
using System.IO;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    [DetalhePagina(Symbol.Setting, "Configurações")]
    public sealed partial class Configuracoes : Page
    {
        readonly ComprasInApp Compra = new ComprasInApp(ComprasInApp.Compras.Personalizacao);

        public Configuracoes()
        {
            InitializeComponent();
        }

        bool Servidor
        {
            get => ConfiguracoesSincronizacao.Tipo == TipoAppSincronizacao.Servidor;
            set => ConfiguracoesSincronizacao.Tipo = value ? TipoAppSincronizacao.Servidor : TipoAppSincronizacao.Cliente;
        }

        bool Cliente => !Servidor;

        int OrigemCertificacao
        {
            get => (int)ConfiguracoesCertificacao.Origem;
            set => ConfiguracoesCertificacao.Origem = (OrigemCertificado)value;
        }
        bool InstalacaoLiberada => AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Desktop");

        bool DesconsiderarHorarioVerao
        {
            get => DefinicoesPermanentes.SuprimirHorarioVerao;
            set => DefinicoesPermanentes.SuprimirHorarioVerao = value;
        }

        bool CalcularNumeroNFe
        {
            get => DefinicoesPermanentes.CalcularNumeroNFe;
            set => DefinicoesPermanentes.CalcularNumeroNFe = value;
        }

        bool UsarSOAP11 => !UsarSOAP12;
        bool UsarSOAP12
        {
            get => DefinicoesPermanentes.UsarSOAP12;
            set => DefinicoesPermanentes.UsarSOAP12 = value;
        }

        int ModoBuscaProduto
        {
            get => DefinicoesPermanentes.ModoBuscaProduto;
            set => DefinicoesPermanentes.ModoBuscaProduto = value;
        }

        int ModoBuscaCliente
        {
            get => DefinicoesPermanentes.ModoBuscaCliente;
            set => DefinicoesPermanentes.ModoBuscaCliente = value;
        }

        int ModoBuscaComprador
        {
            get => DefinicoesPermanentes.ModoBuscaComprador;
            set => DefinicoesPermanentes.ModoBuscaComprador = value;
        }

        int ModoBuscaMotorista
        {
            get => DefinicoesPermanentes.ModoBuscaMotorista;
            set => DefinicoesPermanentes.ModoBuscaMotorista = value;
        }

        int ModoBuscaVendedor
        {
            get => DefinicoesPermanentes.ModoBuscaVendedor;
            set => DefinicoesPermanentes.ModoBuscaVendedor = value;
        }
        
        async void UsarImagem(object sender, TappedRoutedEventArgs e)
        {
            if (await Compra.AnalisarCompra())
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
        }

        async void UsarCor(object sender, TappedRoutedEventArgs e)
        {
            if (await Compra.AnalisarCompra())
            {
                MainPage.Current.DefinirTipoBackground(TiposBackground.Cor);
            }
        }

        async void EscolherTransparencia(object sender, TappedRoutedEventArgs e)
        {
            if (await Compra.AnalisarCompra())
            {
                var caixa = new EscolherTransparencia(DefinicoesPermanentes.OpacidadeBackground);
                if (await caixa.ShowAsync() == ContentDialogResult.Primary)
                {
                    DefinicoesPermanentes.OpacidadeBackground = caixa.Opacidade;
                    MainPage.Current.DefinirOpacidadeBackground(caixa.Opacidade);
                }
            }
        }

        async void Resetar(object sender, TappedRoutedEventArgs e)
        {
            if (await Compra.AnalisarCompra())
            {
                DefinicoesPermanentes.OpacidadeBackground = 1;
                MainPage.Current.DefinirTipoBackground(TiposBackground.Padrao);
            }
        }

        async void SalvarBackup(object sender, TappedRoutedEventArgs e)
        {
            var objeto = new ConjuntoBanco();
            objeto.AtualizarPadrao();
            var xml = objeto.ToXElement().ToString();

            var caixa = new FileSavePicker();
            caixa.FileTypeChoices.Add("Arquivo XML", new string[] { ".xml" });
            var arq = await caixa.PickSaveFileAsync();
            if (arq != null)
            {
                var stream = await arq.OpenStreamForWriteAsync();
                using (StreamWriter escritor = new StreamWriter(stream))
                {
                    await escritor.WriteAsync(xml);
                    await escritor.FlushAsync();
                }
            }
        }
    }
}
