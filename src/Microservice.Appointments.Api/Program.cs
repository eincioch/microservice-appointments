using Microservice.Appointments.Api.DependencyInjection;
using Microservice.Appointments.Api.Initializers;
using Microservice.Appointments.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

RabbitMqInitializer.SetupRabbitMq();

builder.Services.AddControllers();
builder.Services.AddUseCases();
builder.Services.AddEvents(builder.Configuration);
builder.Services.AddMappers();
builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();