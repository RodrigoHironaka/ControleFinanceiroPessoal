﻿using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using Dominio.ObejtoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Cofre : Base
    {
        public virtual Int64 Codigo { get; set; }
        //O que sobrar do balanço pode ser inserido, transferido, depositado, etc., aqui no cofre(banco);
        public virtual Caixa Caixa { get; set; }
        public virtual Banco Banco { get; set; }

        //esse valor deve ser somado ou subtraido ao que ja tem na conta definida pelo usuario;
        public virtual Decimal Valor { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
        public virtual FormaPagamento TransacoesBancarias { get; set; }
        public virtual EntradaSaida Situacao { get; set; }
    }
}
