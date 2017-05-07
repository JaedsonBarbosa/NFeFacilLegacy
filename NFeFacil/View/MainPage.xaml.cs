﻿using BibliotecaCentral.Log;
using System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Propriedades públicas
        internal Frame FramePrincipal => frmPrincipal;
        internal IconElement Icone
        {
            set => symTitulo.Content = value;
        }
        internal string Titulo
        {
            get => txtTitulo.Text;
            set => txtTitulo.Text = value;
        }
        internal int IndexHamburguer
        {
            get { return lstFunções.SelectedIndex; }
            set { lstFunções.SelectedIndex = value; }
        }
        private bool avisoOrentacaoHabilitado;
        internal bool AvisoOrentacaoHabilitado
        {
            get => avisoOrentacaoHabilitado;
            set
            {
                avisoOrentacaoHabilitado = value;
                AnalisarOrientacao();
            }
        }
        #endregion

        internal static MainPage Current { get; private set; }

        private ApplicationView View { get; }

        private int ultimoIndex;
        public MainPage()
        {
            InitializeComponent();
            Current = this;
            View = ApplicationView.GetForCurrentView();
            View.VisibleBoundsChanged += (x, y) => AnalisarOrientacao();
            ProcessarAsync();
            Propriedades.Intercambio = new IntercambioTelas();
            SystemNavigationManager.GetForCurrentView().BackRequested += Propriedades.Intercambio.RetornoEvento;
            AbrirFunção(nameof(Inicio));
        }

        private void AnalisarOrientacao()
        {
            if (AvisoOrentacaoHabilitado)
            {
                if (View.Orientation == ApplicationViewOrientation.Landscape)
                {
                    grdAvisoRotacao.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdAvisoRotacao.Visibility = Visibility.Visible;
                }
            }
            else
            {
                grdAvisoRotacao.Visibility = Visibility.Collapsed;
            }
        }

        private static async void ProcessarAsync()
        {
            var familia = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (familia.Contains("Mobile"))
                await StatusBar.GetForCurrentView().HideAsync();
            else if (familia.Contains("Desktop"))
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                new Saida().Escrever(TitulosComuns.ErroSimples, "Tipo não reconhecido de dispositivo, não é possível mudar a barra de título.");
        }

        private void btnHamburguer_Click(object sender, RoutedEventArgs e)
        {
            hmbMenu.IsPaneOpen = !hmbMenu.IsPaneOpen;
        }

        private void AbrirFunção(object sender, ItemClickEventArgs e)
        {
            AbrirFunção((e.ClickedItem as FrameworkElement).Name);
        }

        private async void AbrirFunção(string tela)
        {
            if (FramePrincipal?.Content is IValida validar && !await validar.Verificar())
            {
                IndexHamburguer = ultimoIndex;
                return;
            }
            await Propriedades.Intercambio.AbrirFunçaoAsync(tela);
            ultimoIndex = IndexHamburguer;
        }
    }
}
