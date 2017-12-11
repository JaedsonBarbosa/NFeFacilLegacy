﻿using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NFeFacil.IBGE
{
    public sealed class Municipio
    {
        public byte CodigoUF { get; set; }
        public string Nome { get; set; }
        public int Codigo { get; set; }

        public Municipio() { }

        public Municipio(XElement xmlMunicípio)
        {
            var elementos = xmlMunicípio.Elements().GetEnumerator();
            elementos.MoveNext();
            CodigoUF = byte.Parse(elementos.Current.Value);
            elementos.MoveNext();
            Nome = RemoverAcentuacao(elementos.Current.Value);
            elementos.MoveNext();
            Codigo = int.Parse(elementos.Current.Value);
            elementos.Dispose();
        }

        public override bool Equals(object obj)
        {
            if (obj is Municipio mun)
            {
                return GetHashCode() == mun.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CodigoUF * Codigo * Nome.Length;
        }

        private static string RemoverAcentuacao(string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(x => char.IsLetter(x) || x == ' ')
                .ToArray());
        }
    }
}