﻿using NFeFacil.ViewNFe.CaixasImpostos;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoPIS
{
    [DetalhePagina("PIS")]
    public sealed partial class DetalharAmbos : Page
    {
        public double Aliquota { get; private set; }
        public double Valor { get; private set; }
        bool CalculoAliquota => TipoCalculo == TiposCalculo.PorAliquota;
        bool CalculoValor => TipoCalculo == TiposCalculo.PorValor;
        public TiposCalculo TipoCalculo { get; private set; }

        public DetalharAmbos()
        {
            InitializeComponent();
        }
    }
}