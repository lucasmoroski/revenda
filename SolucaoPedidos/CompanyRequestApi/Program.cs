using MediatR;
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
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProcessarPedidoEmpresaCommandValidator>());


builder.Services.AddTransient<IPedidoEmpresaRepository, PedidoEmpresaRepository>();


builder.Services.AddHttpClient<IEmpresaApiClient, EmpresaApiClient>()
    .AddStandardResilienceHandler(options =>
    {

        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(response =>
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.RequestTimeout ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable);

        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 10;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
    });


builder.Services.AddHttpClient<IRevendaApiClient, RevendaApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RevendaApi:BaseUrl"]);
});

// builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["AzureServiceBus:ConnectionString"]));
builder.Services.AddSingleton<IMessagingService, ServiceBusService>();

builder.Services.AddHostedService<PedidoClienteReceivedConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();