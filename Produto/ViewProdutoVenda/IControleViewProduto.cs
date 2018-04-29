﻿using System;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.ViewProdutoVenda
{
    public interface IControleViewProduto
    {
        bool Concluido { get; }
        bool PodeConcluir { get; }
        bool PodeDetalhar { get; }
        Guid[] ProdutosAdicionados { get; }

        bool AnalisarDetalhamentoProduto(ExibicaoProdutoAdicao produto);
        ExibicaoProdutoListaGeral Adicionar(AdicionarProduto caixa);
        void Remover(ExibicaoProdutoListaGeral produto);
        void Detalhar();

        bool Validar();
        void Avancar();
        void Concluir();
    }
}
