﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.Impostos
{
    public sealed partial class EscolherTipoPISouCOFINS : ContentDialog, IEscolherImpSimples
    {
        public EscolherTipoPISouCOFINS()
        {
            InitializeComponent();
        }

        string cst;
        public string CST
        {
            get => cst;
            set
            {
                cst = value;
                var valor = int.Parse(value);

                if (valor == 1 || valor == 2)
                {
                    TipoCalculo = TiposCalculo.PorAliquota;
                    cmbTipoCalculo.IsEnabled = false;
                }
                else if (valor == 3)
                {
                    TipoCalculo = TiposCalculo.PorValor;
                    cmbTipoCalculo.IsEnabled = false;
                }
                else if (valor >= 4 && valor <= 9 && valor != 5)
                {
                    TipoCalculo = TiposCalculo.Inexistente;
                    cmbTipoCalculo.IsEnabled = false;
                }
                else
                {
                    cmbTipoCalculo.IsEnabled = true;
                }

                cmbTipoCalculo.SelectedIndex = -1;
            }
        }

        public TiposCalculo TipoCalculo { get; private set; }

        private void TipoCalculoMudou(object sender, SelectionChangedEventArgs e)
        {
            var novoItem = (ComboBoxItem)e.AddedItems[0];
            var tag = int.Parse(novoItem.Tag.ToString());
            if (tag != -1) TipoCalculo = (TiposCalculo)tag;
        }
    }
}
