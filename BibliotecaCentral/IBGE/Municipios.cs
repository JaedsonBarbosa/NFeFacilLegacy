﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BibliotecaCentral.IBGE
{
    public static class Municipios
    {
        private static Municipio[] MunicipiosCache { get; set; }

        public static IEnumerable<Municipio> Get(Estado est)
        {
            if (est == null || est.Codigo == 0) return new ObservableCollection<Municipio>();
            var estados = Estados.EstadosCache;
            return from mun in MunicipiosCache
                   where mun.CodigoUF == est.Codigo
                   select mun;
        }

        public static IEnumerable<Municipio> Get(string nomeSigla)
        {
            if (string.IsNullOrEmpty(nomeSigla)) return new ObservableCollection<Municipio>();
            var estado = Estados.EstadosCache.FirstOrDefault(x => (nomeSigla.Length == 2 ? x.Sigla : x.Nome) == nomeSigla);
            if (estado == null) return new ObservableCollection<Municipio>();
            else return from mun in MunicipiosCache
                   where mun.CodigoUF == estado.Codigo
                   select mun;
        }

        public static IEnumerable<Municipio> Get(ushort codigo)
        {
            if (codigo == 0) return new ObservableCollection<Municipio>();
            return from mun in MunicipiosCache
                   where mun.CodigoUF == codigo
                   select mun;
        }

        internal static void Buscar()
        {
            if (MunicipiosCache == null)
            {
                var xml = new XML(nameof(Municipios)).Retornar();
                var filhos = xml.Elements().ToArray();
                var quant = filhos.Length;
                MunicipiosCache = new Municipio[quant];
                for (int i = 0; i < quant; i++)
                {
                    MunicipiosCache[i] = new Municipio(filhos[i]);
                }
            }
        }
    }
}
