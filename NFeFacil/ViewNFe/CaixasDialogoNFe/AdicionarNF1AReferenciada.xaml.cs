﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesIdentificacao;
using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.CaixasDialogoNFe
{
    public sealed partial class AdicionarNF1AReferenciada : ContentDialog
    {
        public NF1AReferenciada Contexto { get; } = new NF1AReferenciada();

        public AdicionarNF1AReferenciada()
        {
            InitializeComponent();
        }
    }
}