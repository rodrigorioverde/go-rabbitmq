using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ_Produtor.Service
{
    public class ProdutorService
    {
        //Constrói instâncias de IConnection
        private readonly ConnectionFactory connectionFactory;

        //Representa uma conexão AMQP
        private readonly IConnection connection;

        //Representa um canal AMQP e fornece a maioria das operações (métodos de protocolo)
        private readonly IModel model;

        public ProdutorService()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            connection = connectionFactory.CreateConnection();
            model = connection.CreateModel();
        }


        public void Publish(string routingKey, string jsonContent)
        {
            string queueName = "mb.esteira.validacao-cpf";
            string exchangeName = "mb.esteira.direct-exchange";
           

            //Caso não exista a exchange, ela será criada em tempo de execução.
            model.ExchangeDeclare(
                exchange: exchangeName,
                ///Direct 
                ///Fanout
                ///Headers
                ///Topic 
                type: ExchangeType.Direct,
                durable: false, //Quando ativo, a exchange permanece ativa quando o servidor é reiniciado.
                autoDelete: false //Quando ativo, a exchange é automaticamente excluída após todos os consumidores terminarem de usá-la.
                );

            //Caso não exista a queue, ela será criada em tempo de execução.
            model.QueueDeclare(
                queue: queueName,
                durable: false, //Quando ativo, a fila permanece ativa quando o servidor é reiniciado.
                exclusive: false, //Filas exclusivas só podem ser acessadas através da conexão corrent e são excluídas quando a conexão é fechada.
                autoDelete: false, //Quando ativo, a fila é automaticamente excluída após todos os consumidores terminarem de usá-la.
                arguments: null
                );

            //Relaciona a fila criada à exchange criada.
            model.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            //Você pode usar variaveis sobrecarregadas para especificar propriedades de mensagens.
            var props = model.CreateBasicProperties();
            props.DeliveryMode = 2; //Modo de entrega persistente.
            props.Expiration = "36000000"; //Tempo de vida da mensagem.

            //Funciona passando routingKey="Nome da fila" e exchange="" (vazio). Neste caso o rabbit direciona para a exchange Default.
            model.BasicPublish(
                exchange: exchangeName, 
                routingKey: routingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(jsonContent));
        }

    }
}
