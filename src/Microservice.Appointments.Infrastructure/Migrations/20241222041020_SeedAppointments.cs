using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microservice.Appointments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Appointments",
                columns: ["Id", "Title", "StartTime", "EndTime", "Description", "Status"],
                values: new object[,]
                {
                    { 1, "Daily Standup", new DateTime(2024, 12, 22, 9, 30, 0), new DateTime(2024, 12, 22, 9, 45, 0), "Daily team standup to discuss progress and blockers.", 1 },
                    { 2, "Sprint Planning", new DateTime(2024, 12, 23, 10, 0, 0), new DateTime(2024, 12, 23, 12, 0, 0), "Planning session for the upcoming sprint.", 1 },
                    { 3, "Code Review Session", new DateTime(2024, 12, 24, 15, 0, 0), new DateTime(2024, 12, 24, 16, 30, 0), "Team review of pull requests and recent code changes.", 0 },
                    { 4, "One-on-One with Team Lead", new DateTime(2024, 12, 26, 11, 0, 0), new DateTime(2024, 12, 26, 11, 30, 0), "Monthly one-on-one to discuss performance and career goals.", 1 },
                    { 5, "Tech Demo for Stakeholders", new DateTime(2024, 12, 27, 14, 0, 0), new DateTime(2024, 12, 27, 15, 0, 0), "Presentation of new features to stakeholders.", 3 },
                    { 6, "Retrospective Meeting", new DateTime(2024, 12, 28, 16, 0, 0), new DateTime(2024, 12, 28, 17, 0, 0), "Sprint retrospective to discuss improvements.", 0 },
                    { 7, "Pair Programming Session", new DateTime(2024, 12, 29, 10, 0, 0), new DateTime(2024, 12, 29, 11, 30, 0), "Collaborative coding session with a team member.", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValues: [1, 2, 3, 4, 5, 6, 7]);
        }
    }
}