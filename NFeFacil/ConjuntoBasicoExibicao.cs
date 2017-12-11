﻿using System;
using Windows.UI.Xaml.Media;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil
{
    internal class ConjuntoBasicoExibicao
    {
        public Guid Id { get; set; }
        public ImageSource Imagem { get; set; }
        public string Principal { get; set; }
        public string Secundario { get; set; }
    }
}