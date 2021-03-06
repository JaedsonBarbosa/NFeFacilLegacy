﻿using BaseGeral;
using BaseGeral.Controles;
using BaseGeral.ItensBD;
using BaseGeral.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace RegistroComum
{
    [DetalhePagina(Symbol.Library, "Vendas")]
    public sealed partial class RegistrosVenda : Page, IHambuguer
    {
        ObservableCollection<ExibicaoVenda> Vendas { get; } = new ObservableCollection<ExibicaoVenda>();
        List<ExibicaoVenda> Validas { get; } = new List<ExibicaoVenda>();
        List<ExibicaoVenda> Canceladas { get; } = new List<ExibicaoVenda>();

        public ObservableCollection<ItemHambuguer> ConteudoMenu => new ObservableCollection<ItemHambuguer>
        {
            new ItemHambuguer(Symbol.Accept, "Válidas"),
            new ItemHambuguer(Symbol.Cancel, "Canceladas")
        };

        public int SelectedIndex
        {
            set
            {
                Vendas.Clear();
                (value == 0 ? Validas : Canceladas).ForEach(Vendas.Add);
            }
        }

        public RegistrosVenda()
        {
            InitializeComponent();
            using (var repo = new BaseGeral.Repositorio.Leitura())
            {
                var registros = repo.ObterRegistrosVenda(DefinicoesTemporarias.EmitenteAtivo.Id);
                int contador = 0, quant = 0;
                foreach (var (rv, vendedor, cliente, momento) in registros)
                {
                    quant++;
                    if (!rv.Cancelado && rv.Produtos.Count == 0)
                        contador++;
                    (rv.Cancelado ? Canceladas : Validas).Add(new ExibicaoVenda
                    {
                        Base = rv,
                        NomeCliente = cliente,
                        NomeVendedor = vendedor,
                        DataHoraVenda = momento
                    });
                }
                if (contador > 0 && quant > 0)
                    BaseGeral.Log.Popup.Current.Escrever(BaseGeral.Log.TitulosComuns.Atenção, $"Existem {contador} registros de venda sem produto.");
            }
        }

        private void Exibir(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var item = (MenuFlyoutItem)sender;
            var venda = (ExibicaoVenda)item.DataContext;
            BasicMainPage.Current.Navegar<VisualizacaoRegistroVenda>(venda.Base);
        }
    }

    public struct ExibicaoVenda
    {
        public RegistroVenda Base { get; set; }
        public string NomeVendedor { get; set; }
        public string NomeCliente { get; set; }
        public string DataHoraVenda { get; set; }
    }
}
