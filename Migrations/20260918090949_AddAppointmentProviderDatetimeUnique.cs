using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlowDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentProviderDatetimeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointment_ProviderId",
                table: "Appointment");

            migrationBuilder.CreateIndex(
                name: "UQ_Appointment_Provider_Datetime",
                table: "Appointment",
                columns: new[] { "ProviderId", "AppointmentDatetime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Appointment_Provider_Datetime",
                table: "Appointment");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ProviderId",
                table: "Appointment",
                column: "ProviderId");
        }
    }
}
