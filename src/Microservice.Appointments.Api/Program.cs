using Microservice.Appointments.Api.DependencyInjection;
using Microservice.Appointments.Api.Initializers.Core;
using Microservice.Appointments.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

ServiceInitializer.UseConfiguration(builder.Configuration, environment);
ServiceInitializer.InitializeContainers(environment);

IntegrityAssuranceInitializer.RecreateDatabase(environment);

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddUseCases();
builder.Services.AddEvents(builder.Configuration);
builder.Services.AddMappers();
builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

DbContext.AddMigrations(app.Services);
IntegrityAssuranceInitializer.ExecuteScripts(environment);

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();