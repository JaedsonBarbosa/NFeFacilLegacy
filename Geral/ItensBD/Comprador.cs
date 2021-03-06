﻿using System;

namespace BaseGeral.ItensBD
{
    public sealed class Comprador : IStatusAtivacao, IGuidId
    {
        public Guid Id { get; set; }
        public Guid IdEmpresa { get; set; }
        public bool Ativo { get; set; } = true;
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime UltimaData { get; set; }
    }
}
