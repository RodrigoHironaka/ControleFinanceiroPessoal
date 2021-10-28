﻿using CFP.Ferramentas;
using Dominio.Dominio;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CFP.App.Formularios.Principais
{
    /// <summary>
    /// Lógica interna para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
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

        #region Repositorio
        private RepositorioUsuario _repositorio;
        public RepositorioUsuario Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioUsuario(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        Usuario usuario;

        internal string UsuarioLogado;
        internal string UsuarioTipo;

        public Login()
        {
            InitializeComponent();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtUsuario.Text) && !String.IsNullOrEmpty(txtSenha.Password))
            {
                usuario = Repositorio.ObterPorParametros(x => x.NomeAcesso == txtUsuario.Text).FirstOrDefault();

                if (usuario != null)
                {
                    UsuarioLogado = usuario.Nome;
                    UsuarioTipo = usuario.TipoUsuario.ToString();
                    var retorno = new Criptografia(SHA512.Create()).VerificarHash(txtSenha.Password, usuario.Senha);
                    if(retorno)
                        this.DialogResult = true;
                    else
                        MessageBox.Show("Senha Inválida!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Usuario não encontrado!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Usuário/Senha inválidos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btCancelar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(27)) //ESC
            {
                btCancelar_Click(sender, e);
            }
        }
    }
}
