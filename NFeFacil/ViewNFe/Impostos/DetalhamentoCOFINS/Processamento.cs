﻿using NFeFacil.Log;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoCOFINS
{
    public sealed class Processamento : ProcessamentoImposto
    {
        DadosCOFINS dados;

        public override Imposto[] Processar(ProdutoOuServico prod)
        {
            var resultado = dados.Processar(prod);
            if (resultado is Imposto[] list) return list;
            else return new Imposto[1] { (COFINS)resultado };
        }

        public override bool ValidarDados(ILog log) => true;

        public override bool ValidarEntradaDados(ILog log)
        {
            if (Detalhamento is Detalhamento detalhamento)
            {
                var valida = (AssociacoesSimples.COFINS.ContainsKey(detalhamento.CST)
                    && AssociacoesSimples.COFINS[detalhamento.CST] == Tela?.GetType())
                    || AssociacoesSimples.COFINSPadrao == Tela?.GetType();
                if (valida)
                {
                    if (Tela is DetalharAliquota aliq)
                    {
                        dados = new DadosAliq()
                        {
                            Aliquota = aliq.Aliquota
                        };
                    }
                    else if (Tela is DetalharQtde valor)
                    {
                        dados = new DadosQtde()
                        {
                            Valor = valor.Valor
                        };
                    }
                    else if (Tela is DetalharAmbos outr)
                    {
                        if (detalhamento.CST == 5) dados = new DadosST()
                        {
                            Aliquota = outr.Aliquota,
                            Valor = outr.Valor,
                            TipoCalculo = outr.TipoCalculo
                        };
                        else dados = new DadosOutr()
                        {
                            Aliquota = outr.Aliquota,
                            Valor = outr.Valor,
                            TipoCalculo = outr.TipoCalculo
                        };
                    }
                    else
                    {
                        dados = new DadosNT();
                    }
                    dados.CST = detalhamento.CST.ToString("00");
                    return true;
                }
            }
            return false;
        }
    }
}