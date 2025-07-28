using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Common.Interfaces;

namespace ResaleApi.Infrastructure.Services
{
    public class ServiceBusService : IMessagingService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ILogger<ServiceBusService> _logger;
        private readonly List<ServiceBusProcessor> _processors = new();

        public ServiceBusService(IConfiguration configuration, ILogger<ServiceBusService> logger)
        {
            _logger = logger;
            var connectionString = configuration["ServiceBus:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão do Service Bus não foi encontrada em 'ServiceBus:ConnectionString'.");
            }
            _client = new ServiceBusClient(connectionString);
        }

        public async Task PublishAsync<T>(T message, string queueName) where T : class
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "O nome da fila é obrigatório para o Azure Service Bus.");
            }

            // Cria um remetente para a fila específica. É leve e pode ser criado por chamada.
            await using var sender = _client.CreateSender(queueName);

            try
            {
                var messageBody = JsonSerializer.Serialize(message);
                var serviceBusMessage = new ServiceBusMessage(messageBody);

                await sender.SendMessageAsync(serviceBusMessage);
                _logger.LogInformation("Mensagem do tipo {MessageType} publicada na fila {QueueName}.", typeof(T).Name, queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem na fila {QueueName}.", queueName);
                throw;
            }
        }

        public void Subscribe<T>(string queueName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken) where T : class
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "O nome da fila é obrigatório para o Azure Service Bus.");
            }

            // O ServiceBusProcessor é a forma recomendada para consumir mensagens continuamente.
            var processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false, // Processaremos o ACK/NACK manualmente.
                MaxConcurrentCalls = 1 // Processa uma mensagem por vez. Aumente se necessário.
            });

            // 1. Define o handler para processar a mensagem
            processor.ProcessMessageAsync += async (args) =>
            {
                var body = args.Message.Body.ToString();
                try
                {
                    var message = JsonSerializer.Deserialize<T>(body);
                    if (message != null)
                    {
                        await handler(message, args.CancellationToken);

                        // ✅ Confirma que a mensagem foi processada com sucesso e a remove da fila.
                        await args.CompleteMessageAsync(args.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}.", queueName);

                    // ☠️ Abandona a mensagem. Ela ficará disponível para ser reprocessada.
                    // Após um número de tentativas, ela irá automaticamente para a Dead-Letter Queue.
                    await args.AbandonMessageAsync(args.Message);
                }
            };

            // 2. Define o handler para erros do próprio processador (ex: perda de conexão)
            processor.ProcessErrorAsync += (args) =>
            {
                _logger.LogError(args.Exception, "Erro inesperado no processador da fila {QueueName}.", args.EntityPath);
                return Task.CompletedTask;
            };

            // 3. Inicia o processamento
            processor.StartProcessingAsync(cancellationToken).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "Falha ao iniciar o processador para a fila {queueName}", queueName);
                }
            });

            // Adiciona à lista para garantir o Dispose correto no shutdown da aplicação
            _processors.Add(processor);
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Fechando processadores e conexão com o Service Bus...");
            foreach (var processor in _processors)
            {
                await processor.StopProcessingAsync();
                await processor.DisposeAsync();
            }
            await _client.DisposeAsync();
        }
    }
}