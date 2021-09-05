﻿using CFP.App.Formularios.Cadastros;
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

namespace CFP.App.Formularios.ModeloBase.UserControls
{
    /// <summary>
    /// Interação lógica para UserControlCadastros.xam
    /// </summary>
    public partial class UserControlCadastros : UserControl
    {
        public UserControlCadastros()
        {
            InitializeComponent();
        }

        private void btFormaPagamento_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlCadastroFormaPagamento());
        }

        private void btTipoGasto_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlTipoGasto());
        }

        private void btTipoRenda_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlTipoRenda());
        }

        private void btUsuario_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlUsuario());
        }

        private void btPessoa_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlPessoa());
        }

        private void btBanco_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlBanco());
        }
    }
}
