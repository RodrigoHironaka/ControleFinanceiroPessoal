﻿using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using LinqKit;
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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para ConfirmacaoPagamentoParcela.xaml
    /// </summary>
    public partial class ConfirmacaoPagamentoParcela : Window
    {
        ISession Session;
        IList<ContaPagamento> linhaContaPagemento;
        ContaPagamento contaPagamento;
        Boolean editarParcela;
        FluxoCaixa fluxoCaixa;
        Caixa caixa;
        Decimal valorReaj = 0;

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbFormaPagamento.SelectedItem = 0;
        }
        #endregion

        #region Repositorio

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

        private RepositorioFluxoCaixa _repositorioFluxoCaixa;
        public RepositorioFluxoCaixa RepositorioFluxoCaixa
        {
            get
            {
                if (_repositorioFluxoCaixa == null)
                    _repositorioFluxoCaixa = new RepositorioFluxoCaixa(Session);

                return _repositorioFluxoCaixa;
            }
            set { _repositorioFluxoCaixa = value; }
        }

        private RepositorioCaixa _repositorioCaixa;
        public RepositorioCaixa RepositorioCaixa
        {
            get
            {
                if (_repositorioCaixa == null)
                    _repositorioCaixa = new RepositorioCaixa(Session);

                return _repositorioCaixa;
            }
            set { _repositorioCaixa = value; }
        }
        #endregion

        #region PreencheCampos
        private void PreencheCampos()
        {
            string numParcela = string.Empty;
            decimal somaValorParcela = 0;
            decimal somaValorJuros = 0;
            decimal somaValorDesconto = 0;
            decimal somaValorPago = 0;
            foreach (var linha in linhaContaPagemento)
            {
                numParcela = numParcela != string.Empty ? String.Format("{0}, {1}", numParcela, linha.Numero) : linha.Numero.ToString();
                somaValorParcela = somaValorParcela + linha.ValorParcela;
                somaValorJuros += linha.JurosValor;
                somaValorDesconto += linha.DescontoValor;
                somaValorPago += linha.ValorPago;

                txtCodigoConta.Text = linha.Conta.Codigo.ToString();
                txtNumeroParcela.Text = numParcela;
                txtValorParcela.Text = somaValorParcela != 0 ? somaValorParcela.ToString("N2") : string.Empty;
                txtDataPagamento.SelectedDate = linha.DataPagamento != null ? linha.DataPagamento : null;
                txtDataVencimento.SelectedDate = linha.DataVencimento != null ? linha.DataVencimento : null;
                txtJurosPorcentagem.Text = linha.JurosPorcentual != 0 ? linha.JurosPorcentual.ToString("N2") : string.Empty;
                txtJurosValor.Text = somaValorJuros != 0 ? somaValorJuros.ToString("N2") : string.Empty;
                txtDescontoPorcentagem.Text = linha.DescontoPorcentual != 0 ? linha.DescontoPorcentual.ToString("N2") : string.Empty;
                txtDescontoValor.Text = somaValorDesconto != 0 ? somaValorDesconto.ToString("N2") : string.Empty;
                CalculaValorReajustado();
                txtValorPago.Text = somaValorPago != 0 ? somaValorPago.ToString("N2") : string.Empty;
                txtValorRestante.Text = linha.ValorRestante != 0 ? linha.ValorRestante.ToString("N2") : string.Empty;
                cmbFormaPagamento.SelectedItem = linha.FormaPagamento;
            }
        }
        #endregion

        #region Calculo Juros e Desconto (% e $)
        private decimal CalculaValorPorcentagem(decimal valor, decimal porcentagem)
        {
            decimal resultado = Math.Round(valor * (porcentagem / 100), 3);
            return resultado;
        }

        private decimal CalculaPorcentagemValor(decimal valor, decimal valor2)
        {
            decimal porcentagem = Math.Round((valor2 * 100) / valor, 3);
            return porcentagem;
        }

        private void CalculaValorReajustado()
        {
            decimal vparcela = txtValorParcela.Text != string.Empty ? Decimal.Parse(txtValorParcela.Text) : 0;
            decimal vjuros = txtJurosValor.Text != string.Empty ? Decimal.Parse(txtJurosValor.Text) : 0;
            decimal vdesconto = txtDescontoValor.Text != string.Empty ? Decimal.Parse(txtDescontoValor.Text) : 0;
            decimal vpago = txtValorPago.Text != string.Empty ? Decimal.Parse(txtValorPago.Text) : 0;
            valorReaj = vparcela + vjuros - vdesconto;
            txtValorReajustado.Text = (vparcela + vjuros - vdesconto - vpago).ToString("N2");
        }
        #endregion

        #region Definindo Cor Padrão do botão Pesquisar #FF1F3D68 
        public void CorPadrãoLabel()
        {
            var converter = new System.Windows.Media.BrushConverter();
            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
            txtValorPago.Foreground = HexaToBrush;
        }
        #endregion

        #region Verificando se possui valor Restante
        private void CalculaValorRestante(decimal valorpago, decimal valorreajustado)
        {
            if (valorpago == valorreajustado)
            {
                CorPadrãoLabel();
                txtValorRestante.Clear();
            }
            else if (valorpago < valorreajustado)
            {
                CorPadrãoLabel();
                txtValorRestante.Text = (valorreajustado - valorpago).ToString("N2");
            }
            else
            {
                MessageBox.Show("O valor pago digitado é maior que a parcela!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                txtValorPago.Foreground = Brushes.Red;
                txtValorPago.SelectAll();
                txtValorRestante.Clear();
            }

        }
        #endregion

        #region Preenche Objeto para salvar
        DateTime dataVencimento = DateTime.Now;
        private bool PreencheObjeto()
        {
            var qtdlinhaSelecionadas = linhaContaPagemento.Count();
            var valorPago = txtValorPago.Text != string.Empty ? Decimal.Parse(txtValorPago.Text) : 0;
            try
            {
                if (qtdlinhaSelecionadas > 0)
                {
                    foreach (var linha in linhaContaPagemento.OrderBy(x => x.Numero))
                    {
                        linha.DataPagamento = txtDataPagamento.SelectedDate;
                        linha.DataVencimento = txtDataVencimento.SelectedDate;
                        linha.FormaPagamento = (FormaPagamento)cmbFormaPagamento.SelectedItem;
                        linha.ValorParcela = editarParcela != true ? linha.ValorParcela : decimal.Parse(txtValorParcela.Text);
                        linha.SituacaoParcelas = editarParcela != true ? SituacaoParcela.Pago : linha.SituacaoParcelas;

                        linha.JurosPorcentual = txtJurosPorcentagem.Text != string.Empty ? Math.Round(Decimal.Parse(txtJurosPorcentagem.Text), 3) : 0;
                        linha.JurosValor = linha.JurosPorcentual != 0 ? Math.Round(CalculaValorPorcentagem(linha.ValorParcela, linha.JurosPorcentual), 2) : 0;
                        linha.DescontoPorcentual = txtDescontoPorcentagem.Text != string.Empty ? Math.Round(Decimal.Parse(txtDescontoPorcentagem.Text), 3) : 0;
                        linha.DescontoValor = linha.DescontoPorcentual != 0 ? Math.Round(CalculaValorPorcentagem(linha.ValorParcela, linha.DescontoPorcentual), 2) : 0;
                        var parcela = Math.Round(linha.ValorParcela + linha.JurosValor - linha.DescontoValor, 2);

                        decimal res = 0;
                        if (valorPago >= parcela)
                        {
                            valorPago -= parcela;
                            linha.ValorReajustado = txtValorReajustado.Text != string.Empty ? Math.Round(parcela, 2) : 0;
                            linha.ValorPago = txtValorPago.Text != string.Empty ? Math.Round(parcela, 2) : 0;
                            linha.ValorRestante = 0;
                        }
                        else
                        {
                            res = valorPago != 0 ? parcela - valorPago : 0;
                            linha.ValorReajustado = Decimal.Parse(txtValorReajustado.Text); // valorPago != 0 ? Math.Round(parcela, 2) : 0;
                            linha.ValorPago = txtValorPago.Text != string.Empty ? Math.Round(valorPago, 2) : 0;
                            linha.ValorRestante = Math.Round(res, 2);
                            dataVencimento = linha.DataVencimento.Value;
                        }
                    }
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception(ex.ToString());
            }
        }
        #endregion

        #region Validacoes de campos
        private void ValidacoesCampos()
        {
            if (editarParcela)
            {
                if (contaPagamento.Conta.FaturaCartaoCredito == null)
                {
                    txtValorParcela.IsReadOnly = false;
                    txtDataVencimento.IsEnabled = true;
                    var converter = new BrushConverter();
                    gridCor.Background = (Brush)converter.ConvertFromString("#FF1368BD");
                    txtTituloTela.Text = "EDITAR PARCELA";
                }
                else
                {
                    txtValorParcela.IsEnabled = false;
                }

            }
            else
            {
                txtValorParcela.IsReadOnly = true;
                txtDataVencimento.IsEnabled = false;

                var converter = new BrushConverter();
                gridCor.Background = (Brush)converter.ConvertFromString("#FF1F3D68");
                txtTituloTela.Text = "PAGAR/RECEBER PARCELA";
            }
        }
        #endregion

        #region Verificando se caixa esta aberto
        private bool VerificaCaixa()
        {
            caixa = RepositorioCaixa.ObterPorParametros(x => x.Situacao == SituacaoCaixa.Aberto && x.UsuarioAbertura == MainWindow.UsuarioLogado).FirstOrDefault();
            if (caixa == null || caixa.Situacao == SituacaoCaixa.Fechado)
            {
                MessageBoxResult avisoCaixa = MessageBox.Show("Caixa esta fechado! Deseja abrir?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (avisoCaixa == MessageBoxResult.Yes)
                {
                    caixa = new Caixa();
                    MessageBoxResult colocarValor = MessageBox.Show("Deseja digitar um valor inicial?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (colocarValor == MessageBoxResult.Yes)
                    {
                        ConfirmaValorInicialCaixa janela = new ConfirmaValorInicialCaixa();
                        bool? res = janela.ShowDialog();
                        if ((bool)res)
                            caixa.ValorInicial = Decimal.Parse(janela.valorDigitado);
                        else
                            caixa.ValorInicial = 0;
                    }
                    else
                        caixa.ValorInicial = 0;

                    caixa.Codigo = RepositorioCaixa.RetornaUltimoCodigo() + 1;
                    caixa.DataAbertura = DateTime.Now;
                    caixa.UsuarioAbertura = MainWindow.UsuarioLogado;
                    RepositorioCaixa.Salvar(caixa);
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Salvar Fluxo 
        private void SalvarFluxo(IList<ContaPagamento> dados, Boolean estorno)
        {
            if (VerificaCaixa())
            {
                foreach (var item in dados)
                {
                    if (item.SituacaoParcelas == SituacaoParcela.Pago)
                    {
                        fluxoCaixa = new FluxoCaixa();
                        if (item.Conta.TipoConta == TipoConta.Pagar)
                        {
                            if (estorno)
                            {
                                fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                                fluxoCaixa.Nome = String.Format("Estorno parcela número {0} - Conta: {1}", item.Numero, item.Conta.Codigo);
                                fluxoCaixa.Valor = item.ValorPago;
                            }
                            else
                            {
                                fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                                fluxoCaixa.Nome = String.Format("Pagamento parcela número {0} - Conta: {1}", item.Numero, item.Conta.Codigo);
                                fluxoCaixa.Valor = item.ValorPago * -1;
                            }
                        }
                        else
                        {

                            if (estorno)
                            {
                                fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                                fluxoCaixa.Nome = String.Format("Devolucao parcela número {0} - Conta: {1}", item.Numero, item.Conta.Codigo);
                                fluxoCaixa.Valor = item.ValorPago * -1;
                            }
                            else
                            {
                                fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                                fluxoCaixa.Nome = String.Format("Recebimento parcela número {0} - Conta: {1}", item.Numero, item.Conta.Codigo);
                                fluxoCaixa.Valor = item.ValorPago;
                            }
                        }
                        fluxoCaixa.DataGeracao = DateTime.Now;
                        fluxoCaixa.Conta = item.Conta;
                        fluxoCaixa.UsuarioCriacao = MainWindow.UsuarioLogado;
                        fluxoCaixa.Caixa = caixa;
                        fluxoCaixa.FormaPagamento = item.FormaPagamento;
                        RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                    }
                }
            }
        }
        #endregion

        #region Data escolhida é menor q dataAtual
        private bool VerificaData(DateTime data)
        {
            if (data < DateTime.Now)
            {
                return false;
            }
            return true;
        }
        #endregion
        public ConfirmacaoPagamentoParcela(IList<ContaPagamento> _contaPagamento, ISession _session)
        {
            InitializeComponent();
            linhaContaPagemento = _contaPagamento;
            Session = _session;


        }

        public ConfirmacaoPagamentoParcela(Boolean _editarParcela, ContaPagamento _contaPagamento, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            contaPagamento = _contaPagamento;
            editarParcela = _editarParcela;
            linhaContaPagemento = new List<ContaPagamento>();
            linhaContaPagemento.Add(contaPagamento);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombos();
            PreencheCampos();
            ValidacoesCampos();
            VerificaCaixa();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtJurosPorcentagem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosPorcentagem.Text))
            {
                txtJurosValor.Text = CalculaValorPorcentagem(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtJurosPorcentagem.Text)).ToString("N2");
                CalculaValorReajustado();
            }
            else
            {
                txtJurosValor.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtJurosValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosValor.Text))
            {
                txtJurosPorcentagem.Text = CalculaPorcentagemValor(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtJurosValor.Text)).ToString("N3");
                CalculaValorReajustado();
            }
            else
            {
                txtJurosPorcentagem.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtDescontoPorcentagem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDescontoPorcentagem.Text))
            {
                txtDescontoValor.Text = CalculaValorPorcentagem(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtDescontoPorcentagem.Text)).ToString("N2");
                CalculaValorReajustado();
            }
            else
            {
                txtDescontoValor.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtDescontoValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDescontoValor.Text))
            {
                txtDescontoPorcentagem.Text = CalculaPorcentagemValor(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtDescontoValor.Text)).ToString("N3");
                CalculaValorReajustado();
            }
            else
            {
                txtDescontoPorcentagem.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtValorPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var valorPago = !String.IsNullOrEmpty(txtValorPago.Text) ? Decimal.Parse(txtValorPago.Text) : 0;
                    CalculaValorReajustado();
                    CalculaValorRestante(valorPago, valorReaj);
            }
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDataPagamento.Text))
            {
                if (!VerificaData(txtDataPagamento.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59)))
                {
                    MessageBoxResult d = MessageBox.Show("A data escolhida é menor que a data atual!\nDeseja continuar?", " Informação ", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (d == MessageBoxResult.No)
                        return;
                }
            }

            if (!String.IsNullOrEmpty(txtDataVencimento.Text))
            {
                if (!VerificaData(txtDataVencimento.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59)))
                {
                    MessageBoxResult d = MessageBox.Show("A data escolhida é menor que a data atual!\nDeseja continuar?", " Informação ", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (d == MessageBoxResult.No)
                        return;
                }
            }

            CalculaValorReajustado();

            if (PreencheObjeto())
            {
                if (editarParcela)
                {
                    DialogResult = true;
                }
                else
                {
                    if (txtDataPagamento.SelectedDate == null)
                    {
                        MessageBox.Show("Digite a data do pagamento!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtDataPagamento.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(txtValorPago.Text))
                    {
                        MessageBox.Show("Digite o valor Pago!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtValorPago.SelectAll();
                        return;
                    }
                    else
                        CalculaValorRestante(Decimal.Parse(txtValorPago.Text), valorReaj);

                    if (valorReaj < decimal.Parse(txtValorPago.Text))
                    {
                        MessageBox.Show("O valor pago digitado é maior que a parcela!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtValorPago.SelectAll();
                        return;
                    }

                    if (cmbFormaPagamento.SelectedItem == null)
                    {
                        MessageBox.Show("Defina a forma de pagamento!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                        cmbFormaPagamento.Focus();
                        return;
                    }

                    if (!String.IsNullOrEmpty(txtValorRestante.Text))
                    {
                        linhaContaPagemento.Add(new ContaPagamento()
                        {
                            SituacaoParcelas = SituacaoParcela.Parcial,
                            Numero = new RepositorioConta(Session).ObterPorId(linhaContaPagemento.First().Conta.Id).ContaPagamentos.Count(),
                            ValorParcela = Decimal.Parse(txtValorRestante.Text),
                            ValorReajustado = Decimal.Parse(txtValorRestante.Text),
                            DataVencimento = dataVencimento
                        });

                    }

                    var contasPagar = linhaContaPagemento.Where(x => x.SituacaoParcelas == SituacaoParcela.Pago && x.Conta.TipoConta == TipoConta.Pagar &&
                        x.FormaPagamento.RemoveCofre == SimNao.Sim).ToList();
                    if (contasPagar.Count > 0)
                    {
                        DebitoAutomaticoBanco janela = new DebitoAutomaticoBanco(contasPagar, Session, caixa);
                        bool? res = janela.ShowDialog();
                        if (!(bool)res)
                        {
                            foreach (var item in linhaContaPagemento)
                            {
                                item.DataPagamento = null;
                                item.DescontoPorcentual = 0;
                                item.DescontoValor = 0;
                                item.JurosPorcentual = 0;
                                item.JurosValor = 0;
                                item.ValorPago = 0;
                                item.ValorReajustado = item.ValorParcela;
                                item.ValorRestante = 0;
                                item.SituacaoParcelas = SituacaoParcela.Pendente;
                                item.FormaPagamento = null;
                            }
                            return;
                        }
                    }
                    DialogResult = true;
                }


            }
        }

        private void txtJurosPorcentagem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtJurosValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtDescontoPorcentagem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtDescontoValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtValorPago_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtJurosPorcentagem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtJurosValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtDescontoPorcentagem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtDescontoValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorPago_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtValorParcela_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculaValorReajustado();
            txtValorRestante.Clear();
        }

        private void txtValorParcela_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValorParcela.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValorParcela.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorParcela.Clear();
                    txtValorParcela.Focus();
                }
            }
        }

        private void txtJurosPorcentagem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosPorcentagem.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtJurosPorcentagem.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtJurosPorcentagem.Clear();
                    txtJurosPorcentagem.Focus();
                }
            }
        }

        private void txtDescontoPorcentagem_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!String.IsNullOrEmpty(txtDescontoPorcentagem.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtDescontoPorcentagem.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDescontoPorcentagem.Clear();
                    txtDescontoPorcentagem.Focus();
                }
            }
        }

        private void txtJurosValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosValor.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtJurosValor.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtJurosValor.Clear();
                    txtJurosValor.Focus();
                }
            }
        }

        private void txtDescontoValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDescontoValor.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtDescontoValor.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDescontoValor.Clear();
                    txtDescontoValor.Focus();
                }
            }
        }

        private void txtValorPago_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValorPago.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValorPago.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorPago.Clear();
                    txtValorPago.Focus();
                }
            }
        }

        private void txtValorParcela_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtDataPagamento_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculaValorReajustado();
        }
    }
}
