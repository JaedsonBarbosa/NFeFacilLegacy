﻿using System;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.ViewProdutoVenda
{
    public abstract class ProdutoGenericoVenda
    {
        public Guid IdBase { get; set; }

        public double ValorUnitario { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public double Quantidade { get; set; }
        public double Frete { get; set; }
        public double Seguro { get; set; }
        public double DespesasExtras { get; set; }
        public double Desconto { get; set; }
    }
}
