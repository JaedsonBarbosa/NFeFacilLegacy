﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoICMSUFDest
{
    [DetalhePagina("ICMS para a UF de destino")]
    public sealed partial class Detalhar : Page, IDadosICMSUFDest
    {
        public ICMSUFDest Imposto { get; } = new ICMSUFDest();

        public Detalhar()
        {
            InitializeComponent();
        }
    }
}