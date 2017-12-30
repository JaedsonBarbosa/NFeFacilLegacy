﻿using NFeFacil.Certificacao;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesAssinatura;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NFeFacil.WebService.Pacotes
{
    [XmlRoot("inutNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public struct InutNFe : ISignature
    {
        [XmlAttribute("versao")]
        public string Versao { get; set; }

        [XmlElement("infInut", Order = 0)]
        public PartesInutNFe.InfInut Info { get; set; }

        [XmlElement("Signature", Order = 1, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Assinatura Signature { get; set; }

        public async Task PrepararEventos()
        {
            await new AssinaFacil(this).Assinar<InutNFe>(Info.Id, "infInut");
        }
    }
}
