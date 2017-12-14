﻿using NFeFacil.Log;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;
using static NFeFacil.ExtensoesPrincipal;

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoICMS.DadosRN
{
    abstract class BaseRN : IDadosICMS
    {
        public string CST { protected get; set; }
        public int Origem { protected get; set; }
        public abstract object Processar(DetalhesProdutos prod);
        //public abstract bool Validar(ILog log);

        protected double CalcularBC(DetalhesProdutos detalhes)
        {
            var prod = detalhes.Produto;
            var totalBruto = prod.ValorTotal;
            var frete = string.IsNullOrEmpty(prod.Frete) ? 0 : Parse(prod.Frete);
            var seguro = string.IsNullOrEmpty(prod.Seguro) ? 0 : Parse(prod.Seguro);
            var despesas = string.IsNullOrEmpty(prod.DespesasAcessorias) ? 0 : Parse(prod.DespesasAcessorias);
            var desconto = string.IsNullOrEmpty(prod.Desconto) ? 0 : Parse(prod.Desconto);

            return totalBruto + frete + seguro + despesas - desconto;
        }

        protected double ObterIPI(DetalhesProdutos detalhes)
        {
            var impCriados = detalhes.Impostos.impostos;
            for (int i = 0; i < impCriados.Count; i++)
            {
                if (impCriados[i] is IPI ipi && ipi.Corpo is IPITrib trib)
                {
                    return Parse(trib.vIPI);
                }
            }
            return 0;
        }
    }
}
