using Microservice.Appointments.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Infrastructure;

public class AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; } = null!;
}