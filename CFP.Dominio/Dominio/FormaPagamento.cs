﻿using CFP.Dominio.Dominio;
using Dominio.ObjetoValor;
using System;

namespace Dominio.Dominio
{
    public class FormaPagamento : Base
    {
        public virtual Int32 QtdParcelas { get; set; }
        public virtual Int32 DiasParaVencimento { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
