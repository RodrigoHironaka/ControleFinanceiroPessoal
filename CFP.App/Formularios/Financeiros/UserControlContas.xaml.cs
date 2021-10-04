﻿using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlContas.xam
    /// </summary>
    public partial class UserControlContas : UserControl
    {
        ISession Session;
        Conta conta;

        #region Carrega Combos
        private void CarregaCombos()
        {
            lblSituacao.Text = SituacaoConta.Aberto.ToString();
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbTipoConta.SelectedIndex = 0;
            cmbTipoPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbTipoPeriodo.SelectedIndex = 0;

            cmbTipoGasto.ItemsSource = new RepositorioGrupoGasto(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList<GrupoGasto>();
            cmbTipoGasto.SelectedIndex = 0;

            if (conta.Id != 0)
            {
                cmbTipoSubGasto.ItemsSource = new RepositorioSubGrupoGasto(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .Where(x => x.GrupoGasto == conta.GrupoGasto)
               .OrderBy(x => x.Nome)
               .ToList<SubGrupoGasto>();
            }


            cmbFormaCompra.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.QtdParcelas > 0)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbFormaCompra.SelectedIndex = 0;

            DatagridCmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();

            cmbReferenciaPessoa.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<Pessoa>();


        }
        #endregion

        #region Repositorio
        private RepositorioConta _repositorio;
        public RepositorioConta Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConta(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }

        private RepositorioContaPagamento _repositorioContaPagamento;
        public RepositorioContaPagamento RepositorioContaPagamento
        {
            get
            {
                if (_repositorioContaPagamento == null)
                    _repositorioContaPagamento = new RepositorioContaPagamento(Session);

                return _repositorioContaPagamento;
            }
            set { _repositorioContaPagamento = value; }
        }
        #endregion

        #region Controle de acessos Inicial e Cadastro
        private void ControleAcessoInicial()
        {
            //Bloqueando
            GridCampos.IsEnabled = !GridCampos.IsEnabled;
            btSalvar.IsEnabled = !btSalvar.IsEnabled;
            btExcluir.IsEnabled = !btExcluir.IsEnabled;

            //Desbloqueando
            btPesquisar.IsEnabled = true;
            txtCodigo.IsEnabled = true;

            txtCodigo.Focus();
            txtCodigo.SelectAll();
        }

        private void ControleAcessoCadastro()
        {
            //Bloqueando
            btPesquisar.IsEnabled = !btPesquisar.IsEnabled;
            txtCodigo.IsEnabled = !txtCodigo.IsEnabled;

            //Desbloqueando
            GridCampos.IsEnabled = true;
            btSalvar.IsEnabled = true;
            btExcluir.IsEnabled = true;

            //define o foco no primeiro campo
            txtNome.Focus();
            txtNome.Select(txtNome.Text.Length, 0);

        }
        #endregion

        #region Selecão e Foco no campo Codigo 
        private void FocoNoCampoCodigo()
        {
            txtCodigo.SelectAll();
            txtCodigo.Focus();
        }
        #endregion

        #region Limpa os campos do Cadastro
        public void LimpaCampos()
        {
            lblSituacao.Text = SituacaoConta.Aberto.ToString();
            contaPagamento.Clear();
            btSalvar.Visibility = Visibility.Visible;
            btExcluir.Visibility = Visibility.Visible;
            foreach (var item in GridControls.Children)
            {
                if (item is TabControl)
                {
                    tabItemGeral.IsSelected = true;
                    DataGridContaPagamento.IsEnabled = false;
                    foreach (var gridControls2 in GridControls2.Children)
                    {
                        if (gridControls2 is TextBox)
                            (gridControls2 as TextBox).Text = string.Empty;
                        if (gridControls2 is ComboBox)
                            (gridControls2 as ComboBox).SelectedIndex = 0;
                        if (gridControls2 is CheckBox)
                            (gridControls2 as CheckBox).IsChecked = false;
                        if (gridControls2 is RadioButton)
                            (gridControls2 as RadioButton).IsChecked = false;
                        if (gridControls2 is DatePicker)
                            (gridControls2 as DatePicker).Text = string.Empty;

                        if (gridControls2 is Grid)
                        {
                            foreach (var gridRow1 in GridRow1.Children)
                            {
                                if (gridRow1 is TextBox)
                                    (gridRow1 as TextBox).Text = string.Empty;
                                if (gridRow1 is ComboBox)
                                    (gridRow1 as ComboBox).SelectedIndex = 0;

                            }
                            foreach (var gridRow2 in GridRow2.Children)
                            {
                                if (gridRow2 is TextBox)
                                    (gridRow2 as TextBox).Text = string.Empty;
                                if (gridRow2 is ComboBox)
                                    (gridRow2 as ComboBox).SelectedIndex = 0;
                            }
                            foreach (var gridRow3 in GridRow3.Children)
                            {
                                if (gridRow3 is TextBox)
                                    (gridRow3 as TextBox).Text = string.Empty;
                                if (gridRow3 is ComboBox)
                                    (gridRow3 as ComboBox).SelectedIndex = 0;
                                if (gridRow3 is DatePicker)
                                    (gridRow3 as DatePicker).Text = string.Empty;
                            }
                            foreach (var gridRow4 in GridRow4.Children)
                            {
                                if (gridRow4 is TextBox)
                                    (gridRow4 as TextBox).Text = string.Empty;
                                if (gridRow4 is ComboBox)
                                    (gridRow4 as ComboBox).SelectedIndex = 0;
                                txtQtdParcelas.IsEnabled = false;
                                btGerarParcelas.IsEnabled = false;
                            }
                            foreach (var gridRow5 in GridRow5.Children)
                            {
                                if (gridRow5 is TextBox)
                                    (gridRow5 as TextBox).Text = string.Empty;
                                if (gridRow5 is ComboBox)
                                    (gridRow5 as ComboBox).SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                //tab Geral
                conta.Nome = txtNome.Text;
                conta.TipoConta = (TipoConta)cmbTipoConta.SelectedIndex;
                conta.GrupoGasto = (GrupoGasto)cmbTipoGasto.SelectedItem;
                conta.SubGrupoGasto = cmbTipoSubGasto.SelectedItem != null ? (SubGrupoGasto)cmbTipoSubGasto.SelectedItem : null;
                conta.TipoPeriodo = (TipoPeriodo)cmbTipoPeriodo.SelectedIndex;
                conta.Situacao = ((SituacaoConta)Enum.Parse(typeof(SituacaoConta), lblSituacao.Text));
                conta.DataEmissao = (DateTime)(txtEmissao.Text != string.Empty ? txtEmissao.SelectedDate : conta.DataEmissao.GetValueOrDefault());
                conta.DataPrimeiroVencimento = (DateTime)(txtPrimeiroVencimento.Text != string.Empty ? txtPrimeiroVencimento.SelectedDate : conta.DataPrimeiroVencimento.GetValueOrDefault());
                conta.ValorTotal = txtValorTotal.Text != string.Empty ? Decimal.Parse(txtValorTotal.Text) : 0;
                conta.QtdParcelas = txtQtdParcelas.Text != string.Empty ? Int64.Parse(txtQtdParcelas.Text) : 0;
                conta.Pessoa = (Pessoa)(cmbReferenciaPessoa.SelectedItem ?? null);
                conta.NumeroDocumento = txtNumDocumento.Text != string.Empty ? Int64.Parse(txtNumDocumento.Text) : 0;
                conta.FormaCompra = (FormaPagamento)cmbFormaCompra.SelectedItem;
                conta.Observacao = txtObservacao.Text;

                //tab Pagamento
                conta.ContaPagamentos = (IList<ContaPagamento>)DataGridContaPagamento.ItemsSource;
                return true;
            }
            catch (Exception)
            {
                //throw new Exception(ex.ToString());
                return false;
            }
        }

        private void PreencheObjetoComListaDadaGrid()
        {
            try
            {
                if (conta.ContaPagamentos.Count != 0)
                {
                    foreach (var item in conta.ContaPagamentos)
                    {
                        ContaPagamento contaPagamento = new ContaPagamento();
                        contaPagamento.Numero = item.Numero;
                        contaPagamento.ValorParcela = item.ValorParcela;
                        contaPagamento.DataVencimento = item.DataVencimento;
                        contaPagamento.DataPagamento = item.DataPagamento;
                        contaPagamento.JurosPorcentual = item.JurosPorcentual;
                        contaPagamento.JurosValor = item.JurosValor;
                        contaPagamento.DescontoPorcentual = item.DescontoPorcentual;
                        contaPagamento.DescontoValor = item.DescontoValor;
                        contaPagamento.ValorReajustado = item.ValorReajustado;
                        contaPagamento.ValorPago = item.ValorPago;
                        contaPagamento.SituacaoParcelas = item.SituacaoParcelas;
                        contaPagamento.FormaPagamento = item.FormaPagamento;
                        contaPagamento.Conta = item.Conta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            if (conta != null)
            {
                txtCodigo.Text = conta.Id.ToString();
                txtNome.Text = conta.Nome.ToString();
                cmbTipoConta.SelectedIndex = conta.TipoConta.GetHashCode();
                cmbTipoGasto.SelectedItem = conta.GrupoGasto;
                cmbTipoSubGasto.SelectedItem = conta.SubGrupoGasto;
                cmbTipoPeriodo.SelectedIndex = conta.TipoPeriodo.GetHashCode();
                lblSituacao.Text = conta.Situacao.ToString();
                txtEmissao.Text = conta.DataEmissao != DateTime.MinValue ? conta.DataEmissao.ToString() : string.Empty;
                txtPrimeiroVencimento.Text = conta.DataPrimeiroVencimento != DateTime.MinValue ? conta.DataPrimeiroVencimento.ToString() : string.Empty;
                txtValorTotal.Text = conta.ValorTotal > 0 ? conta.ValorTotal.ToString() : string.Empty;
                txtQtdParcelas.Text = conta.QtdParcelas > 0 ? conta.QtdParcelas.ToString() : string.Empty;
                if (!String.IsNullOrEmpty(txtQtdParcelas.Text))
                {
                    txtQtdParcelas.IsEnabled = true;
                    btGerarParcelas.IsEnabled = true;
                }
                cmbReferenciaPessoa.SelectedItem = conta.Pessoa;
                txtNumDocumento.Text = conta.NumeroDocumento > 0 ? conta.NumeroDocumento.ToString() : string.Empty;
                cmbFormaCompra.SelectedItem = conta.FormaCompra;
                txtObservacao.Text = conta.Observacao;
                var listaContaPagamentos = conta.ContaPagamentos;
                foreach (var item in listaContaPagamentos)
                {
                    contaPagamento.Add(item);
                }
            }
        }
        #endregion

        #region Definindo Cor Padrão do botão Pesquisar #FF1F3D68 
        public void CorPadrãoBotaoPesquisar()
        {
            var converter = new System.Windows.Media.BrushConverter();
            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
            btPesquisar.Background = HexaToBrush;
        }
        #endregion

        #region Vericando Tipo de Periodo
        private void VerificaTipoPeriodo()
        {
            switch (cmbTipoPeriodo.SelectedIndex)
            {
                case 0:
                    txtQtdParcelas.IsEnabled = false;
                    btGerarParcelas.IsEnabled = false;
                    txtQtdParcelas.Clear();
                    break;
                case 1:
                    txtQtdParcelas.IsEnabled = true;
                    btGerarParcelas.IsEnabled = true;
                    break;
                case 2:
                    txtQtdParcelas.IsEnabled = false;
                    btGerarParcelas.IsEnabled = false;
                    txtQtdParcelas.Clear();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Divisao Valor Total e Qtd de Parcelas
        private decimal DivisaoTotalPorQtd(string total, string qtd)
        {
            Decimal valortotal = total != string.Empty ? Decimal.Parse(total) : 0;
            Int64 quantidade = qtd != string.Empty ? Int64.Parse(qtd) : 0;
            try
            {
                if (!String.IsNullOrEmpty(txtValorTotal.Text) || !String.IsNullOrEmpty(txtQtdParcelas.Text))
                {
                    var resultado = valortotal / quantidade;
                    return resultado;
                }
                else
                {
                    MessageBox.Show("Verifique se os campos foram preeenchidos corretamente!");
                    return 0;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique se os campos foram preeenchidos corretamente!");
                return 0;
            }

        }
        #endregion

        #region ValidaCampos
        public bool ValidaCampos()
        {
            if (String.IsNullOrEmpty(txtNome.Text) || String.IsNullOrEmpty(txtEmissao.Text) || String.IsNullOrEmpty(txtPrimeiroVencimento.Text))
            {
                MessageBox.Show(" Os campos  Nome, Data emissão e Data Vencimento são Obrigatorios, por favor verifique!");
                return false;
            }
            if (txtPrimeiroVencimento.SelectedDate < txtEmissao.SelectedDate)
            {
                MessageBox.Show("Data de vencimento não pode ser menor que data de emissão!");
                return false;
            }
            if ((TipoPeriodo)cmbTipoPeriodo.SelectedIndex == TipoPeriodo.Unica)
            {
                GerarParcelas(txtValorTotal.Text, "1", txtPrimeiroVencimento.SelectedDate.Value);
                DataGridContaPagamento.Items.Refresh();
            }
            return true;

        }
        #endregion

        #region Verifcando situações das parcelas
        private bool VerificaSituacaoParcelas()
        {
            if (conta.ContaPagamentos != null)
            {
                foreach (var item in conta.ContaPagamentos)
                {
                    if (item.SituacaoParcelas != SituacaoParcela.Pendente)
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region Remove todos os itens da lista Conta Pagamento e do Banco
        private void RemoveTodosOsItensDaListaEBanco()
        {
            if(conta.ContaPagamentos != null)
            {
                var lista = new List<ContaPagamento>(contaPagamento);
                foreach (var item in lista)
                {
                    contaPagamento.Remove(item);
                    conta.ContaPagamentos.Remove(item);
                    RepositorioContaPagamento.Excluir(item);
                }
            }
        }
        #endregion

        #region Gerar Parcelas
        private void GerarParcelas(string vTotal, string qtd, DateTime primeiroVencimento)
        {
            if (VerificaSituacaoParcelas())
            {
                RemoveTodosOsItensDaListaEBanco();
                #region Gerando as Parcelas
                Decimal valorTotal = Decimal.Parse(vTotal);
                Int32 qtdParcelas = Int32.Parse(qtd);
                DateTime dataPrimeiroVencimento = primeiroVencimento;
                if (!valorTotal.Equals(0) || !qtdParcelas.Equals(0))
                {

                    Decimal valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
                    Decimal valorDiferenca = valorTotal - valorParcela * qtdParcelas;

                    PreencheDataGrid();
                    for (int i = 0; i < qtdParcelas; i++)
                    {
                        String numeroParcela = (i + 1).ToString().PadLeft(2, '0');
                        String valorParcelaCorrigido = !(i + 1 == qtdParcelas) ? valorParcela.ToString() : (valorParcela + valorDiferenca).ToString();
                        String dataVencimento = dataPrimeiroVencimento.AddMonths(i).ToShortDateString();

                        contaPagamento.Add(new ContaPagamento()
                        {
                            SituacaoParcelas = SituacaoParcela.Pendente,
                            Numero = Int32.Parse(numeroParcela),
                            ValorParcela = Decimal.Parse(valorParcelaCorrigido),
                            DataVencimento = Convert.ToDateTime(dataVencimento)
                        });
                        DataGridContaPagamento.Items.Refresh();
                    }
                    DataGridContaPagamento.Items.Refresh();
                }
                #endregion 
            }
            else
            {
                MessageBoxResult d = MessageBox.Show("Existem parcelas modificadas! Deseja REFAZER mesmo assim? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    RemoveTodosOsItensDaListaEBanco();
                    #region Gerando as Parcelas
                    Decimal valorTotal = Decimal.Parse(vTotal);
                    Int32 qtdParcelas = Int32.Parse(qtd);
                    DateTime dataPrimeiroVencimento = primeiroVencimento;
                    if (!valorTotal.Equals(0) || !qtdParcelas.Equals(0))
                    {

                        Decimal valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
                        Decimal valorDiferenca = valorTotal - valorParcela * qtdParcelas;

                        PreencheDataGrid();
                        for (int i = 0; i < qtdParcelas; i++)
                        {
                            String numeroParcela = (i + 1).ToString().PadLeft(2, '0');
                            String valorParcelaCorrigido = !(i + 1 == qtdParcelas) ? valorParcela.ToString() : (valorParcela + valorDiferenca).ToString();
                            String dataVencimento = dataPrimeiroVencimento.AddMonths(i).ToShortDateString();

                            contaPagamento.Add(new ContaPagamento()
                            {
                                SituacaoParcelas = SituacaoParcela.Pendente,
                                Numero = Int32.Parse(numeroParcela),
                                ValorParcela = Decimal.Parse(valorParcelaCorrigido),
                                DataVencimento = Convert.ToDateTime(dataVencimento)
                            });
                            DataGridContaPagamento.Items.Refresh();
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region PreencheDataGrid
        private ObservableCollection<ContaPagamento> contaPagamento;
        public void PreencheDataGrid()
        {
            contaPagamento = new ObservableCollection<ContaPagamento>();
            DataGridContaPagamento.ItemsSource = contaPagamento;
        }
        #endregion

        #region Iniciando o Form Cancelado
        private void VerificandoSituacaoConta()
        {
            switch(lblSituacao.Text)
            {
                case "Aberto":
                    GridControls2.IsEnabled = true;
                    GridItemPagamento.IsEnabled = true;
                    btSalvar.Visibility = Visibility.Visible;
                    break;
                case "Cancelado":
                    GridControls2.IsEnabled = false;
                    GridItemPagamento.IsEnabled = false;
                    btSalvar.Visibility = Visibility.Hidden;
                    btExcluir.Visibility = Visibility.Hidden;
                    break;
                case "Finalizado":
                    GridControls2.IsEnabled = false;
                    GridItemPagamento.IsEnabled = false;
                    btSalvar.Visibility = Visibility.Hidden;
                    break;
            }
        }
        #endregion

        public UserControlContas(Conta _conta, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            conta = _conta;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                ControleAcessoInicial();
                FocoNoCampoCodigo();
                LimpaCampos();
                cmbReferenciaPessoa.SelectedIndex = -1;
            }
            else
            {
                (this.Parent as StackPanel).Children.Remove(this);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ControleAcessoInicial();
            CarregaCombos();
            PreencheDataGrid();
            
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                if (String.IsNullOrEmpty(txtCodigo.Text))
                {
                    conta = new Conta();
                    LimpaCampos();
                    cmbReferenciaPessoa.SelectedIndex = -1;
                    //lblSituacao.Text = SituacaoConta.Aberto.ToString();
                    VerificaTipoPeriodo();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        conta = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (conta != null)
                        {
                            //LimpaCampos();
                            PreencheDataGrid();
                            PreencheCampos();
                            ControleAcessoCadastro();
                            CorPadrãoBotaoPesquisar();
                            VerificandoSituacaoConta();
                        }
                        else
                        {
                            txtCodigo.SelectAll();
                            txtCodigo.Focus();
                            btPesquisar.Background = Brushes.DarkRed;
                        }
                    }
                    catch (Exception)
                    {
                        conta = null;
                    }
                }
            }
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtCodigo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            //PesquisaContas p = new PesquisaContas();
            //p.ShowDialog();
            //if (p.objeto != null)
            //{
            //    conta = p.objeto;
            //    PreencheCampos();
            //    ControleAcessoCadastro();
            //    CorPadrãoBotaoPesquisar();
            //    VerificandoSituacaoConta();
            //}
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidaCampos())
            {
                if (PreencheObjeto())
                {
                    if ((conta.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                    {
                        conta.DataGeracao = DateTime.Now;
                        conta.ContaPagamentos.ToList().ForEach(x => x.Conta = conta);
                        Repositorio.Salvar(conta);
                        PreencheObjetoComListaDadaGrid();
                        txtCodigo.Text = conta.Id.ToString();
                    }
                    else
                    {
                        IList<ContaPagamento> novoContaPagamentos = new List<ContaPagamento>(conta.ContaPagamentos.Count);
                        foreach (var item in (IList<ContaPagamento>)DataGridContaPagamento.ItemsSource)
                            novoContaPagamentos.Add(item);

                        conta.ContaPagamentos.Clear();

                        foreach (var item in novoContaPagamentos)
                        {
                            if (item.Conta == null)
                                item.Conta = this.conta;
                            conta.ContaPagamentos.Add(item);
                        }
                       
                        conta.DataAlteracao = DateTime.Now;
                        Repositorio.Alterar(conta);
                    }

                    ControleAcessoInicial();
                    FocoNoCampoCodigo();
                }
            }

        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (VerificaSituacaoParcelas())
            {
                if (conta != null)
                {
                    MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + conta.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        Repositorio.Excluir(conta);
                        LimpaCampos();
                        ControleAcessoInicial();

                    }
                }
            }
            else
            {
                if (conta != null)
                {
                    MessageBoxResult d = MessageBox.Show("Essa Conta não pode ser excluída!!! Deseja cancelar?!", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        conta.Situacao = SituacaoConta.Cancelado;
                        Repositorio.Alterar(conta);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
            }

        }

        private void cmbTipoPeriodo_LostFocus(object sender, RoutedEventArgs e)
        {
            VerificaTipoPeriodo();
        }

        private void txtQtdParcelas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtQtdParcelas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorParcela_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorParcela_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtPrimeiroVencimento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPrimeiroVencimento.SelectedDate < txtEmissao.SelectedDate)
            {
                MessageBox.Show("Data de vencimento não pode ser menor que data de emissão!");
                return;
            }
        }

        private void btGerarParcelas_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtPrimeiroVencimento.Text))
                GerarParcelas(txtValorTotal.Text, txtQtdParcelas.Text, txtPrimeiroVencimento.SelectedDate.Value);
        }

        private void cmbTipoGasto_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmbTipoGasto.SelectedIndex != -1)
            {
                cmbTipoSubGasto.ItemsSource = new RepositorioSubGrupoGasto(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .Where(x => x.GrupoGasto == cmbTipoGasto.SelectedItem)
               .OrderBy(x => x.Nome)
               .ToList<SubGrupoGasto>();
            }

        }

        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lblSituacao.Text != SituacaoConta.Cancelado.ToString() || lblSituacao.Text != SituacaoConta.Finalizado.ToString())
            {
                if (DataGridContaPagamento.IsEnabled == false)
                    DataGridContaPagamento.IsEnabled = true;
                else
                    DataGridContaPagamento.IsEnabled = false;
            }
        }

        private void btCancelarParcela_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridContaPagamento.IsEnabled)
            {
                var selecao = DataGridContaPagamento.SelectedItem;
                if(selecao != null)
                {
                    MessageBoxResult d = MessageBox.Show("Deseja cancelar esta Parcela?", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        foreach (var item in conta.ContaPagamentos)
                        {
                            if (item == selecao)
                            {
                                item.SituacaoParcelas = SituacaoParcela.Cancelado;
                                RepositorioContaPagamento.Alterar(item);
                                break;
                            }
                        }
                        DataGridContaPagamento.Items.Refresh();
                    }
                }
                else
                    MessageBox.Show("Selecione uma parcela!", "Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Habilite a tela para selecionar uma parcela!", "Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
