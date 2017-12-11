﻿using NFeFacil.ItensBD;
using NFeFacil.Log;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesTransporte;

namespace NFeFacil.Validacao
{
    public sealed class ValidadorMotorista : IValidavel
    {
        private Motorista Mot;

        public ValidadorMotorista(Motorista mot)
        {
            Mot = mot;
        }

        public ValidadorMotorista(MotoristaDI mot)
        {
            Mot = mot.ToMotorista();
        }

        public bool Validar(ILog log)
        {
            return new ValidarDados().ValidarTudo(log,
                new ConjuntoAnalise(string.IsNullOrEmpty(Mot.UF), "Não foi definido uma UF"),
                new ConjuntoAnalise(string.IsNullOrEmpty(Mot.XMun), "Não foi definido um município"),
                new ConjuntoAnalise(string.IsNullOrEmpty(Mot.Nome), "Não foi informado o nome do motorista"));
        }
    }
}