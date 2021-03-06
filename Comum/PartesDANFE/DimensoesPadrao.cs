﻿using Windows.UI.Xaml;
using static BaseGeral.ExtensoesPrincipal;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Comum.PartesDANFE
{
    public sealed class DimensoesPadrao
    {
        public GridLength AlturaLinhaPadrao => CMToLength(0.85);
        public double AlturaLinha => AlturaLinhaPadrao.Value;
        public Thickness MargemBloco => new Thickness(0, 0, 0, CMToPixel(0.1));
        public double LarguraTotal => LarguraTotalStatic;
        internal static double LarguraTotalStatic => CMToPixel(19);
    }
}
