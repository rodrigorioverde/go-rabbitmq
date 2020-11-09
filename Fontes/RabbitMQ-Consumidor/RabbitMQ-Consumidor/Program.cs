using RabbitMQ_Consumidor.Service;
using System;

namespace RabbitMQ_Consumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("****Recebendo Mensagens*****");

            var consumidorService = new ConsumidorService();

            var queueName = "mb.esteira.validacao-cpf";

            consumidorService.MonitorarFila(queueName, pullApi:false);

            Console.ReadKey();
        }
    }
}
