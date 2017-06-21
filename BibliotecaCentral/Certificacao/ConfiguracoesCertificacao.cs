﻿using Windows.Storage;

namespace BibliotecaCentral.Certificacao
{
    public static class ConfiguracoesCertificacao
    {
        private static ApplicationDataContainer Pasta = ApplicationData.Current.LocalSettings;

        public static string IPServidorCertificacao
        {
            get => Pasta.Values[nameof(IPServidorCertificacao)] as string;
            set => Pasta.Values[nameof(IPServidorCertificacao)] = value;
        }

        public static OrigemCertificado Origem
        {
            get
            {
                if (string.IsNullOrEmpty(IPServidorCertificacao))
                {
                    return OrigemCertificado.Importado;
                }
                else
                {
                    return OrigemCertificado.Servidor;
                }
            }
        }
    }

    public enum OrigemCertificado
    {
        Importado,
        Servidor
    }
}
