﻿using NFeFacil.Log;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;
using static NFeFacil.ExtensoesPrincipal;

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoICMS.DadosRN
{
    class Tipo0 : BaseRN
    {
        public int modBC { get; set; }
        public double pICMS { get; set; }

        public Tipo0(TelasRN.Tipo0 tela)
        {
            modBC = tela.modBC;
            pICMS = tela.pICMS;
        }

        public override object Processar(DetalhesProdutos prod)
        {
            var vBC = CalcularBC(prod);
            var vICMS = vBC * pICMS / 100;

            return new ICMS00()
            {
                CST = CST,
                modBC = modBC.ToString(),
                Orig = Origem,
                pICMS = ToStr(pICMS, "F4"),
                vBC = ToStr(vBC),
                vICMS = ToStr(vICMS)
            };
        }

        //public override bool Validar(ILog log) => true;
    }
}
