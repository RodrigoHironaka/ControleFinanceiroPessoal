
alter table ContaArquivos  drop foreign key FK_72DE5358
;

alter table FluxoCaixas  drop foreign key FK_D59D810C
;

alter table FluxoCaixas  drop foreign key FK_2056DEA5
;

alter table FluxoCaixas  drop foreign key FK_43D86958
;

alter table FluxoCaixas  drop foreign key FK_95CF74AB
;

alter table SubGrupoGastos  drop foreign key FK_ED2BD212
;

alter table Bancos  drop foreign key FK_1E48502F
;

alter table Bancos  drop foreign key FK_8C4ECE36
;

alter table Caixas  drop foreign key FK_1DD50A06
;

alter table Caixas  drop foreign key FK_6402C960
;

alter table Caixas  drop foreign key FK_D1D5DDED
;

alter table Cofres  drop foreign key FK_FE16542B
;

alter table Cofres  drop foreign key FK_1B7FF394
;

alter table Cofres  drop foreign key FK_34B96F4A
;

alter table Cofres  drop foreign key FK_A1EFEF63
;

alter table Contas  drop foreign key FK_27A3E395
;

alter table Contas  drop foreign key FK_63270F71
;

alter table Contas  drop foreign key FK_108F2CD5
;

alter table Contas  drop foreign key FK_4BC4A722
;

alter table Contas  drop foreign key FK_747B3355
;

alter table ContasPagamento  drop foreign key FK_1109C842
;

alter table ContasPagamento  drop foreign key FK_77677524
;

alter table HorasExtra  drop foreign key FK_9FA1A2B6
;

alter table PessoaTipoRendas  drop foreign key FK_2F54ECD9
;

alter table PessoaTipoRendas  drop foreign key FK_DC98FCA8
;
drop table if exists Configuracoes;
drop table if exists ContaArquivos;
drop table if exists FluxoCaixas;
drop table if exists SubGrupoGastos;
drop table if exists Bancos;
drop table if exists Caixas;
drop table if exists Cofres;
drop table if exists Contas;
drop table if exists ContasPagamento;
drop table if exists FormasPagamento;
drop table if exists GrupoGastos;
drop table if exists HorasExtra;
drop table if exists Pessoas;
drop table if exists PessoaTipoRendas;
drop table if exists TiposRenda;
drop table if exists Usuarios;
drop table if exists hibernate_unique_key;
create table Configuracoes (Id BIGINT not null, DataGeracao DATETIME, DataAlteracao DATETIME, CaminhoArquivos VARCHAR(250), CaminhoBackup VARCHAR(250), primary key (Id)) ENGINE=InnoDB;
create table ContaArquivos (Id BIGINT not null, Conta BIGINT, Caminho VARCHAR(250), Nome VARCHAR(250), DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table FluxoCaixas (Id BIGINT not null, Nome VARCHAR(150), DataGeracao DATETIME, DataAlteracao DATETIME, Valor DECIMAL(10, 2), TipoFluxo INTEGER, Conta BIGINT, Caixa BIGINT, UsuarioLogado BIGINT, FormaPagamento BIGINT, primary key (Id)) ENGINE=InnoDB;
create table SubGrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, GrupoGasto BIGINT, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Bancos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, TipoContaBanco INTEGER, Situacao INTEGER, Pessoa BIGINT, UsuarioCriacao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Caixas (Id BIGINT not null, Codigo BIGINT, DataAbertura DATETIME, DataFechamento DATETIME, ValorInicial DECIMAL(10, 2), Situacao INTEGER, Pessoa BIGINT, UsuarioAbertura BIGINT, UsuarioFechamento BIGINT, TotalEntrada DECIMAL(10, 2), TotalSaida DECIMAL(10, 2), BalancoFinal DECIMAL(10, 2), primary key (Id)) ENGINE=InnoDB;
create table Cofres (Id BIGINT not null, Codigo BIGINT, Caixa BIGINT, Banco BIGINT, Valor DOUBLE, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, Situacao INTEGER, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table Contas (Id BIGINT not null, Codigo BIGINT, TipoConta INTEGER, TipoPeriodo INTEGER, Situacao INTEGER, DataEmissao DATETIME, DataPrimeiroVencimento DATETIME, ValorTotal DECIMAL(10, 2), QtdParcelas BIGINT, NumeroDocumento BIGINT, SubGrupoGasto BIGINT, GrupoGasto BIGINT, FormaCompra BIGINT, Pessoa BIGINT, UsuarioCriacao BIGINT, Observacao TEXT, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table ContasPagamento (ID BIGINT not null, Numero INTEGER, ValorParcela DECIMAL(10, 2), DataVencimento DATETIME, DataPagamento DATETIME, JurosPorcentual DECIMAL(19,5), JurosValor DECIMAL(10, 2), DescontoPorcentual DECIMAL(19,5), DescontoValor DECIMAL(10, 2), ValorReajustado DECIMAL(10, 2), ValorPago DECIMAL(10, 2), ValorRestante DECIMAL(10, 2), SituacaoParcelas INTEGER, FormaPagamento BIGINT, Conta BIGINT, primary key (ID)) ENGINE=InnoDB;
create table FormasPagamento (Id BIGINT not null, Nome VARCHAR(70), QtdParcelas INTEGER, DiasParaVencimento INTEGER, Situacao INTEGER, DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table GrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table HorasExtra (ID BIGINT not null, Pessoa BIGINT, Data DATETIME, HoraInicioManha BIGINT, HoraFinalManha BIGINT, TotalManha BIGINT, HoraInicioTarde BIGINT, HoraFinalTarde BIGINT, TotalTarde BIGINT, HoraFinalDia BIGINT, primary key (ID)) ENGINE=InnoDB;
create table Pessoas (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, ValorTotalBruto DECIMAL(10, 2), ValorTotalLiquido DECIMAL(10, 2), Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table PessoaTipoRendas (ID BIGINT not null, RendaBruta DECIMAL(10, 2), RendaLiquida DECIMAL(10, 2), TipoRenda BIGINT, Pessoa BIGINT, primary key (ID)) ENGINE=InnoDB;
create table TiposRenda (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Usuarios (Id BIGINT not null, Nome VARCHAR(70), NomeAcesso VARCHAR(70), Senha VARCHAR(255), ConfirmaSenha VARCHAR(255), TipoUsuario INTEGER, Situacao INTEGER, DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
alter table ContaArquivos add index (Conta), add constraint FK_72DE5358 foreign key (Conta) references Contas (Id);
alter table FluxoCaixas add index (Conta), add constraint FK_D59D810C foreign key (Conta) references Contas (Id);
alter table FluxoCaixas add index (Caixa), add constraint FK_2056DEA5 foreign key (Caixa) references Caixas (Id);
alter table FluxoCaixas add index (UsuarioLogado), add constraint FK_43D86958 foreign key (UsuarioLogado) references Usuarios (Id);
alter table FluxoCaixas add index (FormaPagamento), add constraint FK_95CF74AB foreign key (FormaPagamento) references FormasPagamento (Id);
alter table SubGrupoGastos add index (GrupoGasto), add constraint FK_ED2BD212 foreign key (GrupoGasto) references GrupoGastos (Id);
alter table Bancos add index (Pessoa), add constraint FK_1E48502F foreign key (Pessoa) references Pessoas (Id);
alter table Bancos add index (UsuarioCriacao), add constraint FK_8C4ECE36 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Caixas add index (Pessoa), add constraint FK_1DD50A06 foreign key (Pessoa) references Pessoas (Id);
alter table Caixas add index (UsuarioAbertura), add constraint FK_6402C960 foreign key (UsuarioAbertura) references Usuarios (Id);
alter table Caixas add index (UsuarioFechamento), add constraint FK_D1D5DDED foreign key (UsuarioFechamento) references Usuarios (Id);
alter table Cofres add index (Caixa), add constraint FK_FE16542B foreign key (Caixa) references Caixas (Id);
alter table Cofres add index (Banco), add constraint FK_1B7FF394 foreign key (Banco) references Bancos (Id);
alter table Cofres add index (UsuarioCriacao), add constraint FK_34B96F4A foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Cofres add index (UsuarioAlteracao), add constraint FK_A1EFEF63 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Contas add index (SubGrupoGasto), add constraint FK_27A3E395 foreign key (SubGrupoGasto) references SubGrupoGastos (Id);
alter table Contas add index (GrupoGasto), add constraint FK_63270F71 foreign key (GrupoGasto) references GrupoGastos (Id);
alter table Contas add index (FormaCompra), add constraint FK_108F2CD5 foreign key (FormaCompra) references FormasPagamento (Id);
alter table Contas add index (Pessoa), add constraint FK_4BC4A722 foreign key (Pessoa) references Pessoas (Id);
alter table Contas add index (UsuarioCriacao), add constraint FK_747B3355 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table ContasPagamento add index (FormaPagamento), add constraint FK_1109C842 foreign key (FormaPagamento) references FormasPagamento (Id);
alter table ContasPagamento add index (Conta), add constraint FK_77677524 foreign key (Conta) references Contas (Id);
alter table HorasExtra add index (Pessoa), add constraint FK_9FA1A2B6 foreign key (Pessoa) references Pessoas (Id);
alter table PessoaTipoRendas add index (TipoRenda), add constraint FK_2F54ECD9 foreign key (TipoRenda) references TiposRenda (Id);
alter table PessoaTipoRendas add index (Pessoa), add constraint FK_DC98FCA8 foreign key (Pessoa) references Pessoas (Id);
create table hibernate_unique_key ( next_hi BIGINT );
insert into hibernate_unique_key values ( 1 );
