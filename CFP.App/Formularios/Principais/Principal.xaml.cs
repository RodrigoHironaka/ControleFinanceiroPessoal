﻿using CFP.App.Formularios.Cadastros;
using CFP.App.Formularios.Financeiros;
using CFP.App.Formularios.Financeiros.Consultas;
using CFP.App.Formularios.ModeloBase.UserControls;
using CFP.App.Formularios.Principais;
using CFP.Dominio.Dominio;
using CFP.Ferramentas;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using NHibernate;
using SGE.Repositorio.Configuracao;
using System;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFP.App
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Usuario Logado
        public static Usuario UsuarioLogado;
        #endregion

        #region Session
        private static ISession session;
        protected static ISession Session
        {
            get
            {
                if (session == null || !session.IsOpen)
                {
                    if (session != null)
                        session.Dispose();
                    session = NHibernateHelper.GetSession();
                }
                return session;
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonPopUpSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexOpcoesMenu = ListViewMenu.SelectedIndex;

            switch (indexOpcoesMenu)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new ucConsultaContas(Session));
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlContas(new Conta(), Session));
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCaixa(Session));
                    break;
                case 4:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCofre(Session));
                    break;
                default:
                    break;
            }
        }

        private void Principal_Loaded(object sender, RoutedEventArgs e)
        {
            #region Login
            Principal.Visibility = Visibility.Hidden;
            Login login = new Login();
            bool? confirmacao = login.ShowDialog();
            if ((bool)confirmacao)
            {
                UsuarioLogado = login.UsuarioLogado;
                lblNomeUsuario.Text = UsuarioLogado.Nome.ToUpper();
                Principal.Visibility = Visibility.Visible;
                login.Close();
            }
            else
            {
                Close();
            }
            #endregion
        }

        private void ButtonConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlConfiguracoes());
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = System.DateTime.Now.ToShortDateString().Replace("/", "");
                string hora = System.DateTime.Now.ToLongTimeString().Replace(":", "");
                string caminhoPadrao = new RepositorioConfiguracao(Session).ObterTodos().FirstOrDefault().CaminhoBackup;
                string backupSalvar = caminhoPadrao + "\\CFP_" + data + "_" + hora + ".sql";

                if (caminhoPadrao != null)
                {
                    using (MySqlConnection conn = new MySqlConnection(ArquivosXML.StringConexao()))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup bk = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                Mouse.OverrideCursor = Cursors.Wait;
                                bk.ExportToFile(backupSalvar);
                                Mouse.OverrideCursor = null;
                                conn.Close();
                                MessageBox.Show("Backup Salvo em " + backupSalvar);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Defina o caminho padrão para realizar o backup em configurações!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtConta_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlContas(new Conta(), Session));
        }

        private void btCaixa_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlCaixa(Session));
        }

        private void btCofre_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlCofre(Session));
        }

        private void btHorasTrabalho_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlHoraExtra(Session));
        }

        private void btFormaPagamento_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlCadastroFormaPagamento(new FormaPagamento(), Session));
        }

        private void btTipoGasto_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlTipoGasto(new GrupoGasto(), Session));
        }

        private void btTipoRenda_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlTipoRenda(new TipoRenda(), Session));
        }

        private void btPessoa_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlPessoa(new Pessoa(), Session));
        }

        private void btBanco_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlBanco(new Banco(), Session));
        }

        private void btTipoSubGasto_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlTipoSubGasto(new SubGrupoGasto(), Session));
        }

        private void ButtonUsuarios_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlUsuario(new Usuario(), Session));
        }
    }
}
