﻿using NFeFacil.Log;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NFeFacil.Certificacao.LAN
{
    public struct Exportacao
    {
        ILog log;

        public Exportacao(ILog log)
        {
            this.log = log;
        }

        public async Task Exportar(string nomeOriginal, string novoNome, string nomeFormato, string extensao)
        {
            var salvador = new FileSavePicker()
            {
                SuggestedFileName = novoNome,
                DefaultFileExtension = '.' + extensao
            };
            salvador.FileTypeChoices.Add(nomeFormato, new string[1] { ".zip" });
            var arquivo = await salvador.PickSaveFileAsync();
            if (arquivo != null)
            {
                using (var stream = await arquivo.OpenStreamForWriteAsync())
                {
                    var recurso = Extensoes.Retornar(this, $"NFeFacil.Certificacao.LAN.Arquivos.{nomeOriginal}.{extensao}");
                    recurso.CopyTo(stream);
                }
                log.Escrever(TitulosComuns.Sucesso, "Arquivo salvo com sucesso, inicie o repositório remoto com o Iniciar.bat");
            }
        }
    }
}
