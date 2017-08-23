﻿using Comum.Primitivos;
using System;
using System.ComponentModel;
using System.Windows.Input;
using NFeFacil.Log;
using NFeFacil.Certificacao;
using System.Collections.Generic;
using NFeFacil.Importacao;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using NFeFacil.Certificacao.LAN;
using Windows.System.Profile;

namespace NFeFacil.ViewModel
{
    public sealed class ConfiguracoesDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ConfiguracoesDataContext()
        {
            ImportarNotaFiscalCommand = new Comando(ImportarNotaFiscal);
            ImportarDadoBaseCommand = new Comando(ImportarDadoBase);

            AttLista();
        }

        private readonly ILog LogPopUp = Popup.Current;

        #region Certificação

        public ObservableCollection<CertificadoExibicao> ListaCertificados { get; private set; }

        public bool InstalacaoLiberada => AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Desktop");
        public bool ServidorCadastrado => ConfiguracoesCertificacao.Origem == OrigemCertificado.Servidor;

        public ICommand ImportarCertificado => new Comando(async () =>
        {
            if (await new ImportarCertificado().ImportarEAdicionarAsync()) AttLista();
        });
        public ICommand ConectarServidor => new Comando(async () =>
        {
            if (await InformacoesConexao.Cadastrar()) AttLista();
        });
        public ICommand EsquecerServidor => new Comando(() => InformacoesConexao.Esquecer());
        public ICommand InstalarServidor => new Comando(async () => await new Exportacao(LogPopUp).Exportar("ServidorCertificacao", "Servidor de certificação", "Arquivo comprimido", "zip"));
        public ICommand RemoverCertificado => new Comando<CertificadoExibicao>(x =>
        {
            using (var loja = new X509Store())
            {
                loja.Open(OpenFlags.ReadWrite);
                var cert = loja.Certificates.Find(X509FindType.FindBySerialNumber, x.SerialNumber, true)[0];
                loja.Remove(cert);
            }
            AttLista();
        });

        async void AttLista()
        {
            try
            {
                ListaCertificados = await Certificados.ObterCertificadosAsync(OrigemCertificado.Importado);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServidorCadastrado)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListaCertificados)));
            }
            catch (Exception e)
            {
                e.ManipularErro();
            }
        }

        #endregion

        #region Importação

        public ICommand ImportarNotaFiscalCommand { get; }
        public ICommand ImportarDadoBaseCommand { get; }

        private async void ImportarNotaFiscal()
        {
            var resultado = await new ImportarNotaFiscal().ImportarAsync();
            if (resultado.Count == 0)
            {
                LogPopUp.Escrever(TitulosComuns.Sucesso, "As notas fiscais foram importadas com sucesso.");
            }
            else
            {
                StringBuilder stringErros = new StringBuilder();
                stringErros.AppendLine("As seguintes notas fiscais não foram reconhecidas por terem a tag raiz diferente de nfeProc e de NFe.");
                resultado.ForEach(y =>
                {
                    if (y is XmlNaoReconhecido x)
                    {
                        stringErros.AppendLine($"Nome arquivo: {x.NomeArquivo}; Tag raiz: Encontrada: {x.TagRaiz}");
                    }
                    else
                    {
                        stringErros.AppendLine($"Mensagem erro: {y.Message}.");
                    }
                });
                LogPopUp.Escrever(TitulosComuns.Erro, stringErros.ToString());
            }
        }

        private async void ImportarDadoBase()
        {
            var resultado = await new ImportarDadoBase(TipoBásicoSelecionado).ImportarAsync();
            if (resultado.Count == 0)
            {
                LogPopUp.Escrever(TitulosComuns.Sucesso, "As informações base foram importadas com sucesso.");
            }
            else
            {
                StringBuilder stringErros = new StringBuilder();
                stringErros.AppendLine("Os seguintes dados base não foram reconhecidos por terem a tag raiz diferente do esperado.");
                resultado.ForEach(y =>
                {
                    if (y is XmlNaoReconhecido x)
                    {
                        stringErros.AppendLine($"Nome arquivo: {x.NomeArquivo}; Tag raiz encontrada: {x.TagRaiz}; Tags raiz esperadas: {x.TagsEsperadas[0]} ou {x.TagsEsperadas[1]}");
                    }
                    else
                    {
                        stringErros.AppendLine($"Mensagem erro: {y.Message}.");
                    }
                });
                LogPopUp.Escrever(TitulosComuns.Erro, stringErros.ToString());
            }
        }

        public IEnumerable<TiposDadoBasico> TiposBásicos => NFeFacil.ExtensoesPrincipal.ObterItens<TiposDadoBasico>();
        public TiposDadoBasico TipoBásicoSelecionado { get; set; }

        #endregion
    }
}
