﻿using BaseGeral;
using BaseGeral.ItensBD;
using BaseGeral.Log;
using BaseGeral.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace RegistroComum.RelatorioProduto01
{
    [DetalhePagina("\uF0E3", "Configurações de geração")]
    public sealed partial class GeradorRelatorioProduto01 : Page
    {
        ObservableCollection<CategoriaDI> CategoriasDisponiveis, CategoriasEscolhidas;
        ObservableCollection<FornecedorDI> FornecedoresDisponiveis, FornecedoresEscolhidos;
        bool InserirProdutosSemCategoria, InserirProdutosSemFornecedor;

        public GeradorRelatorioProduto01()
        {
            IniciarListas();
            InitializeComponent();
        }

        void IniciarListas()
        {
            using (var leitura = new BaseGeral.Repositorio.Leitura())
            {
                CategoriasDisponiveis = leitura.ObterCategorias().GerarObs();
                FornecedoresDisponiveis = leitura.ObterFornecedores().GerarObs();
            }
            CategoriasEscolhidas = new ObservableCollection<CategoriaDI>();
            FornecedoresEscolhidos = new ObservableCollection<FornecedorDI>();
        }

        void CategoriaAdicionada(object sender, ItemClickEventArgs e) => AdicionarCategoria((CategoriaDI)e.ClickedItem);
        void FornecedorAdicionado(object sender, ItemClickEventArgs e) => AdicionarFornecedor((FornecedorDI)e.ClickedItem);
        void CategoriaRemovida(object sender, ItemClickEventArgs e) => RemoverCategoria((CategoriaDI)e.ClickedItem);
        void FornecedorRemovido(object sender, ItemClickEventArgs e) => RemoverFornecedor((FornecedorDI)e.ClickedItem);

        private void TodasCategorias(object sender, RoutedEventArgs e)
        {
            while (CategoriasDisponiveis.Count > 0)
                AdicionarCategoria(CategoriasDisponiveis[0]);
        }

        void TodosFornecedores(object sender, RoutedEventArgs e)
        {
            while (FornecedoresDisponiveis.Count > 0)
                AdicionarFornecedor(FornecedoresDisponiveis[0]);
        }

        void NenhumaCategoria(object sender, RoutedEventArgs e)
        {
            while (CategoriasEscolhidas.Count > 0)
                RemoverCategoria(CategoriasEscolhidas[0]);
        }

        void NenhumFornecedor(object sender, RoutedEventArgs e)
        {
            while (FornecedoresEscolhidos.Count > 0)
                RemoverFornecedor(FornecedoresEscolhidos[0]);
        }

        void AdicionarCategoria(CategoriaDI categoria)
        {
            CategoriasEscolhidas.Add(categoria);
            CategoriasDisponiveis.Remove(categoria);
        }

        void AdicionarFornecedor(FornecedorDI fornecedor)
        {
            FornecedoresEscolhidos.Add(fornecedor);
            FornecedoresDisponiveis.Remove(fornecedor);
        }

        void RemoverCategoria(CategoriaDI categoria)
        {
            CategoriasDisponiveis.Add(categoria);
            CategoriasEscolhidas.Remove(categoria);
        }

        void RemoverFornecedor(FornecedorDI fornecedor)
        {
            FornecedoresDisponiveis.Add(fornecedor);
            FornecedoresEscolhidos.Remove(fornecedor);
        }

        void GerarRelatorio(object sender, RoutedEventArgs e)
        {
            var produtos = new Dictionary<ParCategoriaFornecedor, List<ExibicaoProduto>>();
            using (var leitura = new BaseGeral.Repositorio.Leitura())
            {
                foreach (var prod in leitura.ObterProdutos().ToArray())
                {
                    if (prod.IdCategoria == Guid.Empty && !InserirProdutosSemCategoria ||
                        prod.IdFornecedor == Guid.Empty && !InserirProdutosSemFornecedor) continue;
                    var estoque = leitura.ObterEstoque(prod.Id);
                    var exib = new ExibicaoProduto(prod, estoque?.Alteracoes.Sum(x => x.Alteração) ?? double.NaN);
                    var categoria = CategoriasEscolhidas.FirstOrDefault(x => x.Id == prod.IdCategoria);
                    if ((prod.IdCategoria != default(Guid) && categoria == null) ||
                        (prod.IdCategoria == default(Guid) && !InserirProdutosSemCategoria)) continue;
                    var fornecedor = FornecedoresEscolhidos.FirstOrDefault(x => x.Id == prod.IdFornecedor);
                    if ((prod.IdFornecedor != default(Guid) && fornecedor == null) ||
                        (prod.IdFornecedor == default(Guid) && !InserirProdutosSemFornecedor)) continue;
                    var key = new ParCategoriaFornecedor(categoria, fornecedor);
                    if (produtos.ContainsKey(key))
                        produtos[key].Add(exib);
                    else
                        produtos.Add(key, new List<ExibicaoProduto> { exib });
                }
            }
            if (produtos.Count == 0)
            {
                Popup.Current.Escrever(TitulosComuns.Atenção, "Não há nenhum produto que corresponda aos critérios especificados.");
                return;
            }
            BasicMainPage.Current.Navegar<ImpressaoRelatorioProduto01>(produtos);
        }
    }
}
