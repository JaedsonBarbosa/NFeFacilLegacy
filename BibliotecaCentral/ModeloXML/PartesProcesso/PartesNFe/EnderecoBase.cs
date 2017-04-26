﻿using System.Xml.Serialization;

namespace BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe
{
    public abstract class enderecoBase
    {
        [XmlElement("xLgr")]
        public string Logradouro { get; set; }
        [XmlElement("nro")]
        public string Numero { get; set; }
        [XmlElement("xCpl")]
        public string Complemento { get; set; }
        [XmlElement("xBairro")]
        public string Bairro { get; set; }
        [XmlElement("cMun")]
        public int CodigoMunicipio { get; set; }
        [XmlElement("xMun")]
        public string NomeMunicipio { get; set; }
        [XmlElement("UF")]
        public string SiglaUF { get; set; }
    }
}