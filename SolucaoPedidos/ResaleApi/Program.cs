using MediatR;
using FluentValidation.AspNetCore;
using System.Reflection;
using ResaleApi.Infrastructure.Contextos;
using ResaleApi.Infrastructure.Interfaces;
using ResaleApi.Infrastructure.Repositories;
using ResaleApi.Infrastructure.Services;
using Common.Interfaces;
using ResaleApi.Features.Validations;

var builder = WebApplication.CreateBuilder(args);

// Configuração do MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>(); // Singleton é ok para o contexto do DB, ele gerencia a conexão
builder.Services.AddTransient<IRevendaRepository, RevendaRepository>();
builder.Services.AddTransient<IPedidoClienteRepository, PedidoClienteRepository>();

// Configuração do MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configuração do FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CriarRevendaCommandValidator>());

// Configuração do serviço de mensageria (RabbitMQ)
builder.Services.AddSingleton<IMessagingService, ServiceBusService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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