﻿using BibliotecaCentral;
using BibliotecaCentral.IBGE;
using BibliotecaCentral.WebService;
using BibliotecaCentral.WebService.Pacotes;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Consulta : Page, IEsconde
    {
        public Consulta()
        {
            InitializeComponent();
            MainPage.Current.SeAtualizar(Telas.Consulta, Symbol.Find, "Consultar NF-e");
            cmbUF.ItemsSource = Estados.EstadosCache.GerarObs();
        }

        private async void btnAnalisar_Click(object sender, RoutedEventArgs e)
        {
            btnAnalisar.IsEnabled = false;
            MostrarCarregamento.Begin();
            var codigo = txtCodigo.Text;
            try
            {
                var resp = await new GerenciadorGeral<ConsSitNFe, RetConsSitNFe>((Estado)cmbUF.SelectedItem, Operacoes.Consultar, false)
                    .EnviarAsync(new ConsSitNFe(codigo));
                DataContext = resp;
            }
            catch (Exception erro)
            {
                var msg = new MessageDialog($"Que pena, algo deu errado, a mensagem do erro é {erro.Message}", "Erro");
                await msg.ShowAsync();
            }
            btnAnalisar.IsEnabled = true;
            await Task.Delay(1000);
            OcultarCarregamento.Begin();
            lstOqAcontece.Items.Clear();
        }

        async Task IEsconde.EsconderAsync()
        {
            OcultarGrid.Begin();
            await Task.Delay(250);
        }
    }
}
