﻿using System.Xml.Serialization;

namespace BaseGeral.ModeloXML
{
    [XmlRoot(ElementName = "nfeProc", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public sealed class ProcessoNFCe
    {
        [XmlAttribute("versao")]
        public string Versao { get; set; } = "4.00";
        [XmlElement(nameof(NFe), Order = 0)]
        public NFCe NFe { get; set; }
        [XmlElement("protNFe", Order = 1)]
        public ProtocoloNFe ProtNFe { get; set; }
    }
}
