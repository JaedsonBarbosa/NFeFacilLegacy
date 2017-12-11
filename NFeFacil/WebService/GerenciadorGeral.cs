﻿using NFeFacil.Certificacao;
using NFeFacil.Pacotes;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NFeFacil.IBGE;
using System;
using NFeFacil.Certificacao.LAN;

namespace NFeFacil.WebService
{
    public struct GerenciadorGeral<Envio, Resposta>
    {
        public DadosServico Enderecos { get; }
        int CodigoUF { get; }
        string VersaoDados{get;}

        public GerenciadorGeral(Estado uf, Operacoes operacao, bool teste)
        {
            Enderecos = new EnderecosConexao(uf.Sigla).ObterConjuntoConexao(teste, operacao);
            CodigoUF = uf.Codigo;
            VersaoDados = operacao == Operacoes.RecepcaoEvento ? Enderecos.VersaoRecepcaoEvento : "3.10";
        }

        public GerenciadorGeral(string siglaOuNome, Operacoes operacao, bool teste)
        {
            var uf = Estados.Buscar(siglaOuNome);
            Enderecos = new EnderecosConexao(uf.Sigla).ObterConjuntoConexao(teste, operacao);
            CodigoUF = uf.Codigo;
            VersaoDados = operacao == Operacoes.RecepcaoEvento ? Enderecos.VersaoRecepcaoEvento : "3.10";
        }

        public GerenciadorGeral(ushort codigo, Operacoes operacao, bool teste)
        {
            var uf = Estados.Buscar(codigo);
            Enderecos = new EnderecosConexao(uf.Sigla).ObterConjuntoConexao(teste, operacao);
            CodigoUF = uf.Codigo;
            VersaoDados = operacao == Operacoes.RecepcaoEvento ? Enderecos.VersaoRecepcaoEvento : "3.10";
        }

        public async Task<Resposta> EnviarAsync(Envio corpo, bool addNamespace = false)
        {
            var origem = ConfiguracoesCertificacao.Origem;
            if (origem == OrigemCertificado.Importado)
            {
                using (var proxy = new HttpClient(new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    UseDefaultCredentials = true
                }, true))
                {
                    proxy.DefaultRequestHeaders.Add("SOAPAction", Enderecos.Metodo);
                    var str = ObterConteudoRequisicao(corpo, addNamespace);
                    var conteudo = new StringContent(str, Encoding.UTF8, "text/xml");
                    var resposta = await proxy.PostAsync(Enderecos.Endereco, conteudo);
                    var xml = XElement.Load(await resposta.Content.ReadAsStreamAsync());
                    return ObterConteudoCorpo(xml).FromXElement<Resposta>();
                }
            }
            else
            {
                var envio = new RequisicaoEnvioDTO()
                {
                    Cabecalho = new CabecalhoRequisicao()
                    {
                        Nome = "SOAPAction",
                        Valor = Enderecos.Metodo
                    },
                    Conteudo = XElement.Parse(ObterConteudoRequisicao(corpo, addNamespace)),
                    Uri = Enderecos.Endereco
                };

                using (var cliente = new HttpClient())
                {
                    var uri = new Uri($"http://{OperacoesServidor.RootUri}:1010/EnviarRequisicao");
                    var xml = envio.ToXElement<RequisicaoEnvioDTO>().ToString(SaveOptions.DisableFormatting);
                    var conteudo = new StringContent(xml, Encoding.UTF8, "text/xml");
                    var resposta = await cliente.PostAsync(uri, conteudo);
                    using (var stream = await resposta.Content.ReadAsStreamAsync())
                    {
                        return stream.FromXElement<Resposta>();
                    }
                }
            }

            XNode ObterConteudoCorpo(XElement soap)
            {
                var nome = XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/");
                var item = soap.Element(nome);
                if (item == null)
                {
                    nome = XName.Get("Body", "http://www.w3.org/2003/05/soap-envelope");
                    item = soap.Element(nome);
                }
                var casca = (XElement)item.FirstNode;
                return casca.FirstNode;
            }
        }

        string ObterConteudoRequisicao(Envio corpo, bool addNamespace)
        {
            var xml = corpo.ToXElement<Envio>();
            if (addNamespace)
            {
                const string namespaceNFe = "http://www.portalfiscal.inf.br/nfe";
                xml.Element(XName.Get("NFe", namespaceNFe)).SetAttributeValue("xmlns", namespaceNFe);
            }
            return string.Format(ExtensoesPrincipal.ObterRecurso("RequisicaoSOAP"),
                Enderecos.Servico, CodigoUF, VersaoDados,
                xml.ToString(SaveOptions.DisableFormatting));
        }
    }
}