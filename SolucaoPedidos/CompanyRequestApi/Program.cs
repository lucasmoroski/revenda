using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Net;
using CompanyRequestApi.Infrastructure.Contextos;
using CompanyRequestApi.Infrastructure.Interfaces;
using CompanyRequestApi.Infrastructure.Repositories;
using Common.Interfaces;
using CompanyRequestApi.Workers;
using CompanyRequestApi.Features.Validations;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using CompanyRequestApi.Infrastructure.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Configuração do PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configuração do FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProcessarPedidoEmpresaCommandValidator>()); // Crie este validator

// Configuração dos Repositórios
builder.Services.AddTransient<IPedidoEmpresaRepository, PedidoEmpresaRepository>();

// Configuração do HttpClient para API da Empresa com Polly
builder.Services.AddHttpClient<IEmpresaApiClient, EmpresaApiClient>()
    .AddStandardResilienceHandler(options =>
    {
        // Configuração da estratégia de Retry (NOVA SINTAXE)
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.BackoffType = DelayBackoffType.Exponential; // Para backoff exponencial
        options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>() // Lida com exceções de rede
            .HandleResult(response =>
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.RequestTimeout ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable);

        // Configuração da estratégia de Circuit Breaker (NOVA SINTAXE)
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 10;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

        // Configuração da estratégia de Timeout (sem alterações)
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
    });

// Configuração do HttpClient para API da Revenda
builder.Services.AddHttpClient<IRevendaApiClient, RevendaApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RevendaApi:BaseUrl"]);
});

// Configuração do serviço de mensageria (RabbitMQ)
builder.Services.AddSingleton<IMessagingService, ServiceBusService>();

// Adicionar o Background Service consumidor de fila
builder.Services.AddHostedService<PedidoClienteReceivedConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations on startup (for production, consider a more robust migration strategy)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();