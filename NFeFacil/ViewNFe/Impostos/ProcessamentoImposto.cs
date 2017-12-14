﻿using NFeFacil.Log;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using System.Globalization;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos
{
    public abstract class ProcessamentoImposto
    {
        protected static CultureInfo CulturaPadrao = CultureInfo.InvariantCulture;

        public IDetalhamentoImposto Detalhamento { protected get; set; }
        public PrincipaisImpostos Tipo => Detalhamento.Tipo;

        public abstract bool ValidarEntradaDados(object Tela);
        public abstract bool ValidarDados(ILog log);
        public abstract ImpostoBase[] Processar(DetalhesProdutos prod);
    }
}
