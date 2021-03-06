﻿using BaseGeral.ItensBD;

namespace RegistroComum.RelatorioProduto01
{
    sealed class ExibicaoProduto
    {
        internal bool Adicionado;
        internal readonly string Codigo;
        internal readonly string Nome;
        internal readonly string Preco;
        internal readonly string Estoque;

        internal ExibicaoProduto(ProdutoDI produto, double estoque)
        {
            Adicionado = false;
            Codigo = produto.CodigoProduto;
            Nome = produto.Descricao;
            Preco = produto.ValorUnitario.ToString("C");
            Estoque = double.IsNaN(estoque) ? "Sem dados" : estoque.ToString("C");
        }
    }
}
