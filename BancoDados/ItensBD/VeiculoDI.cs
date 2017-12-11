﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesTransporte;
using System;

namespace NFeFacil.ItensBD
{
    public sealed class VeiculoDI
    {
        public Guid Id { get; set; }

        public string Descricao { get; set; }
        public string Placa { get; set; }
        public string UF { get; set; }
        public string RNTC { get; set; }

        public Veiculo ToVeiculo()
        {
            return new Veiculo()
            {
                Placa = Placa,
                UF = UF,
                RNTC = RNTC
            };
        }
    }
}