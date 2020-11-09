using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ_Consumidor.Domain
{
    public enum AcaoEsteira
    {
        Aprovar = 1,
        Reprovar = 2,
        Pendenciar = 3,
        PendenciarErro = 4
    }
}
