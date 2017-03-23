﻿using System.Xml.Serialization;

namespace NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes
{
    public class Compra
    {
        /// <summary>
        /// (Opcional)
        /// Nota de Empenho.
        /// </summary>
        [XmlElement("xNEmp")]
        public string XNEmp { get; set; }

        /// <summary>
        /// (Opcional)
        /// Pedido.
        /// </summary>
        [XmlElement("xPed")]
        public string XPed { get; set; }

        /// <summary>
        /// (Opcional)
        /// Contrato.
        /// </summary>
        [XmlElement("xCont")]
        public string XCont { get; set; }
    }
}