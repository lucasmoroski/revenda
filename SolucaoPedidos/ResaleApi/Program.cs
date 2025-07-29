using MediatR;
using FluentValidation.AspNetCore;
using ResaleApi.Infrastructure.Contextos;
using ResaleApi.Infrastructure.Interfaces;
using ResaleApi.Infrastructure.Repositories;
using ResaleApi.Infrastructure.Services;
using Common.Interfaces;
using ResaleApi.Features.Validations;
using MongoDB.Driver;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
builder.Services.AddSingleton(s => s.GetService<IMongoClient>().GetDatabase("resale_db"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddTransient<IRevendaRepository, RevendaRepository>();
builder.Services.AddTransient<IPedidoClienteRepository, PedidoClienteRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CriarRevendaCommandValidator>());

// builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["AzureServiceBus:ConnectionString"]));
builder.Services.AddSingleton<IMessagingService, ServiceBusService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();