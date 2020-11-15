# Go-RabbitMQ

**Go-RabbitMQ** é uma aplicação introdutória para utilização da solução de mensageria RabbitMQ e contem exemplos para utilização básica e principais configurações.

## Configuração do ambiente

Antes de iniciar, será necessário configurar o RabbitMQ localmente, utilizamos neste exemplo o Docker Desktop.

Faça o download do [Docker Desktop](https://docs.docker.com/docker-for-windows/install-windows-home/). 

Para iniciar a utilização, obtenha o aquivo **docker-compose.yaml** na pasta Uteis da solução e exceute:

```
$ docker-compose up -d
```

![](https://github.com/rodrigorioverde/go-rabbitmq/blob/master/Uteis/docker-compose.png?w=200)

O RabbitMQ estará em execução no ambiente Docker:

![](https://github.com/rodrigorioverde/go-rabbitmq/blob/master/Uteis/docker-run.png?w=512)

Acesse o gerenciador (http://localhost:15672/) utilizando o usuário e senha definidos no docker-compose.yaml.

![](https://github.com/rodrigorioverde/go-rabbitmq/blob/master/Uteis/rabbitmq-login.png?w=512)
![](https://github.com/rodrigorioverde/go-rabbitmq/blob/master/Uteis/rabbitmq-management.png?w=512)

Seu RabbitMQ estará em execução e já poderá ser utilizado pela aplicação. 
