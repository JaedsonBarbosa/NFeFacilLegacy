﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class GerenciarDadosBase : Page, IHambuguer
    {
        public GerenciarDadosBase()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainPage.Current.SeAtualizar(Symbol.Manage, "Dados base");
        }

        public IEnumerable ConteudoMenu
        {
            get
            {
                var retorno = new ObservableCollection<Controles.ItemHambuguer>
                {
                    new Controles.ItemHambuguer(Symbol.People, "Clientes"),
                    new Controles.ItemHambuguer(Symbol.People, "Motoristas"),
                    new Controles.ItemHambuguer(Symbol.Shop, "Produtos"),
                    new Controles.ItemHambuguer(Symbol.People, "Vendedores")
                };
                flipView.SelectionChanged += (sender, e) => MainMudou?.Invoke(this, new NewIndexEventArgs { NewIndex = flipView.SelectedIndex });
                return retorno;
            }
        }

        public event EventHandler MainMudou;
        public void AtualizarMain(int index) => flipView.SelectedIndex = index;
    }
}
