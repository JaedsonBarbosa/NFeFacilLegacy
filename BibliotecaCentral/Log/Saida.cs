﻿using System.Diagnostics;

namespace BibliotecaCentral.Log
{
    public class Saida : ILog
    {
        public void Escrever(TitulosComuns título, string texto)
        {
#if DEBUG
            Debug.WriteLine(string.Empty);
            Debug.WriteLine($"{Titulos.ObterString(título)}:");
            Debug.WriteLine(texto);
#endif
        }
    }
}