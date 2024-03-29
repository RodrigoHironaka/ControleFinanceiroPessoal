﻿using CFP.Ferramentas;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Lógica interna para PesquisaPessoas.xaml
    /// </summary>
    public partial class PesquisaPessoas : Window
    {
        #region Session
        ISession Session;
        //private ISession session;
        //protected ISession Session
        //{
        //    get
        //    {
        //        if (session == null || !session.IsOpen)
        //        {
        //            if (session != null)
        //                session.Dispose();

        //            session = NHibernateHelper.GetSession();

        //        }
        //        return session;
        //    }
        //}
        #endregion

        #region Repositório
        private RepositorioPessoa _repositorio;
        public RepositorioPessoa Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioPessoa(Session);
                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        public Pessoa objeto;

        private void SelecionaeFecha()
        {
            if (DataGridPesquisa.Items.Count != 0)
            {
                objeto = DataGridPesquisa.SelectedItem as Pessoa;
                Close();
            }
        }

        private void CarregaDados()
        {
            DataGridPesquisa.ItemsSource = Repositorio.ObterPorParametros(x => x.UsuarioCriacao == MainWindow.UsuarioLogado);
        }

        private void PesquisarDados()
        {
            //recebe todos os dados do dataGrid
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridPesquisa.ItemsSource);
            //verifica se campo de pesquisa é esta vazio, se sim, filtro é nulo
            if (txtPesquisa.Text == string.Empty)
                cv.Filter = null;
            else
            {
                //senão filtra o objeto (neste caso 'Usuario') por Nome e Id
                String pesq = txtPesquisa.Text.ToLower();
                cv.Filter = o =>
                {
                    Pessoa p = o as Pessoa;
                    return Texto.RemoveAcento(p.Nome).Contains(pesq)
                            || p.Id.ToString().Contains(pesq);
                };
            }
        }

        public PesquisaPessoas(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaDados();
            txtPesquisa.Focus();
        }

        private void txtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            PesquisarDados();
        }

        private void DataGridPesquisa_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelecionaeFecha();
        }
    }
}
