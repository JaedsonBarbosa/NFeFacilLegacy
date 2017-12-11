﻿using NFeFacil.ModeloXML.PartesProcesso;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos
{
    [DetalhePagina("Detalhamento de impostos", SimboloSymbol = Symbol.List)]
    public sealed partial class DetalhamentoGeral : Page
    {
        RoteiroAdicaoImpostos roteiro;

        public DetalhamentoGeral()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            roteiro = (RoteiroAdicaoImpostos)e.Parameter;
            Avancar();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (roteiro.Voltar())
            {
                frmImposto.GoBack();
                e.Cancel = true;
            }
        }

        private void Avancar(object sender, RoutedEventArgs e) => Avancar();

        void Avancar()
        {
            var atual = frmImposto.Content as Page;
            if (roteiro.Validar(atual))
            {
                if (roteiro.Avancar())
                {
                    if (roteiro.Current == null)
                    {
                        Avancar();
                    }
                    else
                    {
                        frmImposto.Navigate(roteiro.Current);
                    }
                }
                else
                {
                    Concluir();
                }
            }
        }

        private void Voltar(object sender, RoutedEventArgs e)
        {
            if (roteiro.Voltar())
            {
                frmImposto.Navigate(roteiro.Current);
            }
            else
            {
                MainPage.Current.Retornar();
            }
        }

        async void Concluir()
        {
            await Task.Delay(500);
            var produto = roteiro.Finalizar();

            //Remove tela de manipulação do produto e de escolha dos impostos
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);

            var parametro = Frame.BackStack[Frame.BackStack.Count - 1].Parameter as NFe;
            var info = parametro.Informacoes;

            if (produto.Número == 0)
            {
                produto.Número = info.produtos.Count + 1;
                info.produtos.Add(produto);
            }
            else
            {
                info.produtos[produto.Número - 1] = produto;
            }
            info.total = new Total(info.produtos);

            MainPage.Current.Retornar();
        }

        private void ImpostoTrocado(object sender, NavigationEventArgs e)
        {
            var infoTipo = e.Content.GetType().GetTypeInfo();
            var detalhe = infoTipo.GetCustomAttribute<DetalhePagina>();
            txtTitulo.Text = detalhe.Titulo;
        }
    }
}