﻿using Newtonsoft.Json;
using RabbitMQ_Produtor.Domain;
using RabbitMQ_Produtor.Service;
using System;

namespace RabbitMQ_Produtor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****Enviar Messagnes*****");

            var chaveFila = "mb.esteira.validacao-cpf";


            var produtorService = new ProdutorService();


            ///Loop para reproduzir um consumo externo de APIs
            for (int i = 0; i < 10; i++)
            {
                var entrada = new Entrada
                {
                    Proposta = 1000 + i,
                    Cpf = GerarCpf(),
                    Observacao = string.Format("Mensagem {0}", i),
                    DataRegistro = DateTime.Now
                };

                var json = JsonConvert.SerializeObject(entrada);

                produtorService.Publish(chaveFila, json);

                Console.WriteLine("Mensagem {0} enviada}", i.ToString());
            }

            Console.ReadKey();
        }

        public static long GerarCpf()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            Random rnd = new Random();
            string semente = rnd.Next(100000000, 999999999).ToString();

            for (int i = 0; i < 9; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            return Convert.ToInt64(semente);
        }
    }
}