using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ_Consumidor.Domain
{
    public class Entrada
    {
        public int Proposta { get; set; }
        public long Cpf { get; set; }
        public DateTime DataRegistro { get; set; }
        public string Observacao { get; set; }
    }
}
