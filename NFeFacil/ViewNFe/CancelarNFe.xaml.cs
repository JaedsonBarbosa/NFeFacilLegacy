﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe
{
    public sealed partial class CancelarNFe : ContentDialog
    {
        internal string Motivo { get; private set; }

        public CancelarNFe()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(Motivo))
            {
                args.Cancel = true;
            }
        }
    }
}