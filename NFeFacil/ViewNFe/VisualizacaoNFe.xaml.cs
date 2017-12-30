﻿using NFeFacil.ItensBD;
using NFeFacil.Log;
using NFeFacil.ModeloXML;
using NFeFacil.ModeloXML.PartesProcesso;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe;
using NFeFacil.Validacao;
using NFeFacil.WebService;
using NFeFacil.WebService.Pacotes;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe
{
    [View.DetalhePagina(Symbol.View, "Visualizar NFe")]
    public sealed partial class VisualizacaoNFe : Page
    {
        Popup Log = Popup.Current;
        NFeDI ItemBanco { get; set; }
        object ObjetoItemBanco { get; set; }
        Detalhes Visualizacao { get; set; }

        public VisualizacaoNFe()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ItemBanco = (NFeDI)e.Parameter;
            var xml = XElement.Parse(ItemBanco.XML);
            if (ItemBanco.Status < (int)StatusNFe.Emitida)
            {
                var nfe = xml.FromXElement<NFe>();
                ObjetoItemBanco = nfe;
                Visualizacao = nfe.Informacoes;
            }
            else
            {
                var processo = xml.FromXElement<Processo>();
                ObjetoItemBanco = processo;
                Visualizacao = processo.NFe.Informacoes;
            }
            AtualizarBotoesComando();
        }

        private void Editar(object sender, RoutedEventArgs e)
        {
            var nfe = (NFe)ObjetoItemBanco;
            var analisador = new AnalisadorNFe(ref nfe);
            analisador.Desnormalizar();
            ItemBanco.Status = (int)StatusNFe.Edição;
            MainPage.Current.Navegar<ManipulacaoNotaFiscal>(nfe);
        }

        private void Salvar(object sender, RoutedEventArgs e)
        {
            ItemBanco.Status = (int)StatusNFe.Salva;
            AtualizarDI();
            AtualizarBotoesComando();
            Log.Escrever(TitulosComuns.Sucesso, "Nota fiscal salva com sucesso.");
        }

        private async void Assinar(object sender, RoutedEventArgs e)
        {
            var nfe = (NFe)ObjetoItemBanco;
            try
            {
                var assina = new Certificacao.AssinaFacil(nfe);
                await assina.Assinar<NFe>(nfe.Informacoes.Id, "infNFe");

                ItemBanco.Status = (int)StatusNFe.Assinada;
                AtualizarDI();
                AtualizarBotoesComando();
                Log.Escrever(TitulosComuns.Sucesso, "Nota fiscal assinada com sucesso.");
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        private async void Transmitir(object sender, RoutedEventArgs e)
        {
            var nota = (NFe)ObjetoItemBanco;
            try
            {
                var retTransmissao = await new GerenciadorGeral<EnviNFe, RetEnviNFe>(nota.Informacoes.emitente.Endereco.SiglaUF, Operacoes.Autorizar, nota.AmbienteTestes)
                    .EnviarAsync(new EnviNFe(nota), true);
                if (retTransmissao.StatusResposta == 103)
                {
                    var tempoResposta = retTransmissao.DadosRecibo.TempoMedioResposta;
                    await Task.Delay(TimeSpan.FromSeconds(tempoResposta + 5));
                    var resultadoResposta = await new GerenciadorGeral<ConsReciNFe, RetConsReciNFe>(retTransmissao.Estado, Operacoes.RespostaAutorizar, nota.AmbienteTestes)
                        .EnviarAsync(new ConsReciNFe(retTransmissao.TipoAmbiente, retTransmissao.DadosRecibo.NumeroRecibo));
                    if (resultadoResposta.Protocolo.InfProt.cStat == 100)
                    {
                        Log.Escrever(TitulosComuns.Sucesso, resultadoResposta.DescricaoResposta);

                        ObjetoItemBanco = new Processo()
                        {
                            NFe = nota,
                            ProtNFe = resultadoResposta.Protocolo
                        };
                        ItemBanco.Status = (int)StatusNFe.Emitida;
                        AtualizarDI();
                        AtualizarBotoesComando();
                    }
                    else
                    {
                        Log.Escrever(TitulosComuns.Erro, $"A nota fiscal foi processada, mas recusada. Mensagem de retorno:\r\n" +
                            $"{resultadoResposta.Protocolo.InfProt.xMotivo}");
                    }
                }
                else
                {
                    Log.Escrever(TitulosComuns.Erro, $"A NFe não foi aceita. Mensagem de retorno:\r\n" +
                        $"{retTransmissao.DescricaoResposta}\r\n" +
                        $"Por favor, exporte esta nota fiscal e envie o XML gerado para o desenvolvedor do aplicativo para que o erro possa ser corrigido.");
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        private void Imprimir(object sender, RoutedEventArgs e)
        {
            var processo = (Processo)ObjetoItemBanco;
            MainPage.Current.Navegar<DANFE.ViewDANFE>(processo);
            ItemBanco.Impressa = true;
            AtualizarDI();
            AtualizarBotoesComando();
        }

        private async void Exportar(object sender, RoutedEventArgs e)
        {
            XElement xml;
            string id;
            if (ItemBanco.Status < (int)StatusNFe.Emitida)
            {
                var nfe = (NFe)ObjetoItemBanco;
                id = nfe.Informacoes.Id;
                xml = ObjetoItemBanco.ToXElement<NFe>();
            }
            else
            {
                var processo = (Processo)ObjetoItemBanco;
                id = processo.NFe.Informacoes.Id;
                xml = ObjetoItemBanco.ToXElement<Processo>();
            }

            try
            {
                FileSavePicker salvador = new FileSavePicker
                {
                    DefaultFileExtension = ".xml",
                    SuggestedFileName = $"{id}.xml",
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };
                salvador.FileTypeChoices.Add("Arquivo XML", new string[] { ".xml" });
                var arquivo = await salvador.PickSaveFileAsync();
                if (arquivo != null)
                {
                    using (var stream = await arquivo.OpenStreamForWriteAsync())
                    {
                        xml.Save(stream);
                        await stream.FlushAsync();
                    }

                    ItemBanco.Exportada = true;
                    AtualizarDI();
                    Log.Escrever(TitulosComuns.Sucesso, $"Nota fiscal exportada com sucesso.");
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        private void AtualizarDI()
        {
            try
            {
                using (var repo = new Repositorio.Escrita())
                {
                    if (ItemBanco.Status < (int)StatusNFe.Emitida)
                    {
                        ItemBanco.XML = ObjetoItemBanco.ToXElement<NFe>().ToString();
                    }
                    else
                    {
                        ItemBanco.XML = ObjetoItemBanco.ToXElement<Processo>().ToString();
                    }
                    repo.SalvarItemSimples(ItemBanco, DefinicoesTemporarias.DateTimeNow);
                }
            }
            catch (Exception e)
            {
                e.ManipularErro();
            }
        }

        void AtualizarBotoesComando()
        {
            var status = (StatusNFe)ItemBanco.Status;
            btnEditar.IsEnabled = status == StatusNFe.Validada || status == StatusNFe.Salva || status == StatusNFe.Assinada;
            btnSalvar.IsEnabled = status == StatusNFe.Validada;
            btnAssinar.IsEnabled = status == StatusNFe.Salva;
            btnTransmitir.IsEnabled = status == StatusNFe.Assinada;
            btnImprimir.IsEnabled = status == StatusNFe.Emitida;
        }
    }
}
