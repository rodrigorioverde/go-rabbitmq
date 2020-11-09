using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Consumidor.Adapter;
using RabbitMQ_Consumidor.Domain;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace RabbitMQ_Consumidor.Service
{
    public class ConsumidorService
    {
        //Constrói instâncias de IConnection
        private readonly ConnectionFactory connectionFactory;

        //Representa uma conexão AMQP
        private readonly IConnection connection;

        //Representa um canal AMQP e fornece a maioria das operações (métodos de protocolo)
        private readonly IModel model;

        public ConsumidorService()
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

        public void ConfiguraFila(string queueName, IModel model)
        {
            model.QueueDeclare(
               queue: queueName,
               durable: false, //Quando ativo, a fila permanece ativa quando o servidor é reiniciado.
               exclusive: false, //Filas exclusivas só podem ser acessadas através da conexão corrent e são excluídas quando a conexão é fechada.
               autoDelete: false, //Quando ativo, a fila é automaticamente excluída após todos os consumidores terminarem de usá-la.
               arguments: null
               );
        }

        public void MonitorarFila(string queueName, bool pullApi)
        {
            if (pullApi)
                MonitoraFilaPullApi(queueName);
            else
                MonitoraFilaPushApi(queueName);
        }

        /// <summary>
        /// Esta modalidade para obtenção de fila, realiza um "Push" na fila, recebendo todas as mensagens da fila pelo canal aberto.
        /// Pode ser entendido como Listener.
        /// </summary>
        /// <param name="queueName"></param>
        private void MonitoraFilaPushApi(string queueName)
        {
            ConfiguraFila(queueName, model);

            var consumidor = new EventingBasicConsumer(model);

            consumidor.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.Span;

                var message = Encoding.UTF8.GetString(body);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    var sucesso = ProcessaEvento(message);

                    if (sucesso)
                        model.BasicAck(eventArgs.DeliveryTag, false);
                    else
                        model.BasicReject(eventArgs.DeliveryTag, requeue: true);
                }

                Console.WriteLine($"Mensagem recebida: {message}");

            };

            model.BasicConsume(
                  queue: queueName,
                  autoAck: false,
                  consumer: consumidor
                   );

        }

        /// <summary>
        /// Esta modelidade para obtenção de fila, realiza um "Pull" na fila, obetendo uma mensagem por requisição
        /// Possui desempenho inferior.
        /// </summary>
        /// <param name="queueName"></param>
        private void MonitoraFilaPullApi(string queueName)
        {
            bool mensagensDisponiveis = true;

            while (mensagensDisponiveis)
            {
                BasicGetResult result = model.BasicGet(queueName, autoAck: false);

                IBasicProperties basicProperties = result.BasicProperties;
                var body = result.Body;

                var message = Encoding.UTF8.GetString(body.Span);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    var sucesso = ProcessaEvento(message);

                    if (sucesso)
                        model.BasicAck(result.DeliveryTag, false);
                    else
                        model.BasicReject(result.DeliveryTag, requeue: true);
                }
                else
                    model.BasicReject(result.DeliveryTag, true);

                Console.WriteLine($"Mensagem recebida: {message}");

                mensagensDisponiveis = result.MessageCount > 0;
            }
        }     

        private bool ProcessaEvento(string message)
        {
            var entrada = DesserializarMensagem(message);
            bool cpfValido;
            bool sucesso = false;

            try
            {
                //Representa uma serviço para execução de uma regra de négócio externa
                cpfValido = ValidaCpfAdapter.ValidarCpf(entrada.Cpf.ToString());
                if (cpfValido)
                {
                    //Representa um serviço para execução de um processo pós validação da regra de negócio externa
                    EsteiraAdapter.AcionarEsteira(entrada.Proposta, AcaoEsteira.Aprovar);
                }
                else
                {
                    EsteiraAdapter.AcionarEsteira(entrada.Proposta, AcaoEsteira.Reprovar);
                }

                sucesso = true;
            }
            catch
            {
                EsteiraAdapter.AcionarEsteira(entrada.Proposta, AcaoEsteira.PendenciarErro);
            }

            return sucesso;

        }

        private static Entrada DesserializarMensagem(string message)
        {
            return JsonConvert.DeserializeObject<Entrada>(message);
        }
    }
}
