﻿using Dominio.Dominio;
using Dominio.ObejtoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Mapeamentos
{
    public class CofreMAP : ClassMapping<Cofre>
    {
        public CofreMAP()
        {
            Table("Cofres");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.Caixa);
            ManyToOne(x => x.Banco);
            Property(x => x.Valor);
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoCofre>>());

        }
    }
}