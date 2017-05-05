﻿using BibliotecaCentral.ItensBD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BibliotecaCentral.Importacao
{
    public sealed class ImportarNotaFiscal : Importacao
    {
        public ImportarNotaFiscal() : base(".xml") { }

        public override async Task<RelatorioImportacao> ImportarAsync()
        {
            var arquivos = await ImportarArquivos();
            var retorno = new RelatorioImportacao();
            Dictionary<NFeDI, XElement> conjuntos = new Dictionary<NFeDI, XElement>();
            for (int i = 0; i < arquivos.Count; i++)
            {
                using (var stream = await arquivos[i].OpenStreamForReadAsync())
                {
                    var xmlAtual = XElement.Load(stream);
                    if (xmlAtual.Name.LocalName != "nfeProc" && xmlAtual.Name.LocalName != "NFe")
                    {
                        retorno.Erros.Add(new XmlNaoReconhecido(arquivos[i].Name, xmlAtual.Name.LocalName, "nfeProc", "NFe"));
                    }
                    else
                    {
                        var diAtual = NFeDI.Converter(xmlAtual);
                        if (conjuntos.Keys.Count(x => x.Id == diAtual.Id) == 0)
                        {
                            conjuntos.Add(diAtual, xmlAtual);
                        }
                        else
                        {
                            var atual = conjuntos.Single(x => x.Key.Id == diAtual.Id);
                            if (atual.Key.Status < diAtual.Status)
                            {
                                conjuntos.Remove(atual.Key);
                                conjuntos.Add(diAtual, xmlAtual);
                            }
                        }
                    }
                }
            }
            using (var db = new Repositorio.MudancaOtimizadaBancoDados())
            {
                await db.AdicionarNotasFiscais(conjuntos);
            }
            return retorno;
        }
    }
}