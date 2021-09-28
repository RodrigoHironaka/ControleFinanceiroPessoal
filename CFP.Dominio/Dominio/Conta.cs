﻿using CFP.Dominio.Dominio;
using Dominio.Interfaces;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Conta : Base
    {
        public virtual TipoConta TipoConta { get; set; }
        public virtual GrupoGasto GrupoGasto { get; set; }
        public virtual TipoPeriodo TipoPeriodo { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        public virtual DateTime? DataPrimeiroVencimento { get; set; }
        public virtual Decimal? ValorTotal { get; set; }
        public virtual Int64? QtdParcelas { get; set; }
        public virtual Int64? NumeroDocumento { get; set; }
        public virtual FormaPagamento FormaCompra { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual String Observacao { get; set; }
       
    }
}
