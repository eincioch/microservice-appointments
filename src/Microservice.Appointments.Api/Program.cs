using Microservice.Appointments.Api.DependencyInjection;
using Microservice.Appointments.Api.Initializers.Core;
using Microservice.Appointments.Api.Middlewares;
using Microservice.Appointments.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppointmentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddUseCases();
builder.Services.AddEvents(builder.Configuration);
builder.Services.AddMappers();
builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

ServiceInitializer.InitializeInfrastructure(app.Services);

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