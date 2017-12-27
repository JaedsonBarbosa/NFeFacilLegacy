﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoICMS.DadosSN
{
    public class Tipo101 : BaseSN
    {
        public string pCredSN { get; set; }
        public string vCredICMSSN { get; set; }

        public Tipo101() { }
        public Tipo101(TelasSN.Tipo101 tela)
        {
            pCredSN = tela.pCredSN;
            vCredICMSSN = tela.vCredICMSSN;
        }

        public override object Processar(DetalhesProdutos prod)
        {
            return new ICMSSN101()
            {
                CSOSN = CSOSN,
                Orig = Origem,
                pCredSN = pCredSN,
                vCredICMSSN = vCredICMSSN
            };
        }
    }
}
