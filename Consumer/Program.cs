using Consumer.Config;
using Consumer.Consumers;
using Backend.Clients;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.AddHostedService<OmsOrderCreatedConsumer>();
builder.Services.AddHttpClient<OmsClient>(c => c.BaseAddress = new Uri(builder.Configuration["HttpClient:Oms:BaseAddress"]));
var app = builder.Build();
await app.RunAsync();