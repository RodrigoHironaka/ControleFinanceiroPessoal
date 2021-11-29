﻿using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.Consultas
{
    /// <summary>
    /// Interação lógica para ucAlertaContas.xam
    /// </summary>
    public partial class ucAlertaContas : UserControl
    {
        ISession Session;
        public ucAlertaContas(ISession _session)
        {
            InitializeComponent();
            Session = _session;
            dgAlertaContas.ItemsSource = new RepositorioConta(Session).ObterTodos();
        }

        private void ShowHideDetails(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Teste");
        }
    }
}
