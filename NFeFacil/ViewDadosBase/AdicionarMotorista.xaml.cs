﻿using NFeFacil.Log;
using NFeFacil.Validacao;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using NFeFacil.ItensBD;
using System.Collections.ObjectModel;
using NFeFacil.IBGE;
using NFeFacil.ModeloXML;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AdicionarMotorista : Page
    {
        public MotoristaDI Motorista { get; set; }
        public ObservableCollection<VeiculoDI> Veiculos { get; }
        ObservableCollection<Municipio> ListaMunicipios { get; } = new ObservableCollection<Municipio>();
        private ILog Log = Popup.Current;

        public string UFEscolhida
        {
            get => Motorista.UF;
            set
            {
                Motorista.UF = value;
                ListaMunicipios.Clear();
                foreach (var item in Municipios.Get(value))
                {
                    ListaMunicipios.Add(item);
                }
            }
        }

        public bool IsentoICMS
        {
            get => Motorista.InscricaoEstadual == "ISENTO";
            set
            {
                Motorista.InscricaoEstadual = value ? "ISENTO" : null;
                txtIE.IsEnabled = !value;
            }
        }

        public int TipoDocumento { get; set; }
        public string Documento
        {
            get => Motorista.Documento;
            set
            {
                var tipo = (TiposDocumento)TipoDocumento;
                Motorista.CPF = tipo == TiposDocumento.CPF ? value : null;
                Motorista.CNPJ = tipo == TiposDocumento.CNPJ ? value : null;
            }
        }

        public AdicionarMotorista()
        {
            InitializeComponent();
            using (var db = new AplicativoContext())
            {
                Veiculos = new ObservableCollection<VeiculoDI>(db.Veiculos);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                Motorista = new MotoristaDI();
            }
            else
            {
                Motorista = (MotoristaDI)e.Parameter;
            }
            TipoDocumento = (int)Motorista.TipoDocumento;
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new ValidadorMotorista(Motorista).Validar(Log))
                {
                    using (var db = new AplicativoContext())
                    {
                        Motorista.UltimaData = Propriedades.DateTimeNow;
                        if (Motorista.Id == Guid.Empty)
                        {
                            db.Add(Motorista);
                            Log.Escrever(TitulosComuns.Sucesso, "Motorista salvo com sucesso.");
                        }
                        else
                        {
                            db.Update(Motorista);
                            Log.Escrever(TitulosComuns.Sucesso, "Motorista alterado com sucesso.");
                        }
                        db.SaveChanges();
                    }
                    MainPage.Current.Retornar();
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Retornar();
        }

        async void AdicionarVeiculo(object sender, RoutedEventArgs e)
        {
            var caixa = new AdicionarVeiculo();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                var veic = caixa.Item;
                using (var db = new AplicativoContext())
                {
                    db.Veiculos.Add(veic);
                    db.SaveChanges();
                    Veiculos.Add(veic);
                }
            }
        }
    }
}