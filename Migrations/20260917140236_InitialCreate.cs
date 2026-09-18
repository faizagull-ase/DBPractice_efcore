using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlowDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicLocation",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Address = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicLocation", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisCode",
                columns: table => new
                {
                    diagnosis_code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisCode", x => x.diagnosis_code);
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.MedicationId);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    HomeAddress = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayerName = table.Column<string>(type: "varchar(100)", nullable: false),
                    PlanName = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.PolicyId);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Credentials = table.Column<string>(type: "varchar(50)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.ProviderId);
                });

            migrationBuilder.CreateTable(
                name: "Specialty",
                columns: table => new
                {
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialty", x => x.SpecialtyId);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Room_ClinicLocation_LocationId",
                        column: x => x.LocationId,
                        principalTable: "ClinicLocation",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientPolicy",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientPolicy", x => new { x.PatientId, x.PolicyId });
                    table.ForeignKey(
                        name: "FK_PatientPolicy_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientPolicy_Policy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policy",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProviderLocation",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderLocation", x => new { x.ProviderId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_ProviderLocation_ClinicLocation_LocationId",
                        column: x => x.LocationId,
                        principalTable: "ClinicLocation",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderLocation_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSpecialty",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSpecialty", x => new { x.ProviderId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_ProviderSpecialty_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderSpecialty_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Reason = table.Column<string>(type: "varchar(255)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Encounter",
                columns: table => new
                {
                    EncounterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    EncounterDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NoteText = table.Column<string>(type: "varchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encounter", x => x.EncounterId);
                    table.ForeignKey(
                        name: "FK_Encounter_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncounterDiagnosis",
                columns: table => new
                {
                    EncounterId = table.Column<int>(type: "int", nullable: false),
                    diagnosis_code = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterDiagnosis", x => new { x.EncounterId, x.diagnosis_code });
                    table.ForeignKey(
                        name: "FK_EncounterDiagnosis_DiagnosisCode_diagnosis_code",
                        column: x => x.diagnosis_code,
                        principalTable: "DiagnosisCode",
                        principalColumn: "diagnosis_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EncounterDiagnosis_Encounter_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounter",
                        principalColumn: "EncounterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncounterId = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "varchar(50)", nullable: false),
                    Instructions = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_Prescription_Encounter_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounter",
                        principalColumn: "EncounterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescription_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ProviderId",
                table: "Appointment",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_RoomId",
                table: "Appointment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicLocation_Address",
                table: "ClinicLocation",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClinicLocation_Name",
                table: "ClinicLocation",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Encounter_AppointmentId",
                table: "Encounter",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncounterDiagnosis_diagnosis_code",
                table: "EncounterDiagnosis",
                column: "diagnosis_code");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Name",
                table: "Medication",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Email",
                table: "Patient",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Phone",
                table: "Patient",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientPolicy_PolicyId_MemberId",
                table: "PatientPolicy",
                columns: new[] { "PolicyId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_PatientPolicy_OnePrimary",
                table: "PatientPolicy",
                column: "PatientId",
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_PayerName_PlanName",
                table: "Policy",
                columns: new[] { "PayerName", "PlanName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_EncounterId",
                table: "Prescription",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicationId",
                table: "Prescription",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Email",
                table: "Provider",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Phone",
                table: "Provider",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocation_LocationId",
                table: "ProviderLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSpecialty_SpecialtyId",
                table: "ProviderSpecialty",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_LocationId_Name",
                table: "Room",
                columns: new[] { "LocationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialty_Name",
                table: "Specialty",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncounterDiagnosis");

            migrationBuilder.DropTable(
                name: "PatientPolicy");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "ProviderLocation");

            migrationBuilder.DropTable(
                name: "ProviderSpecialty");

            migrationBuilder.DropTable(
                name: "DiagnosisCode");

            migrationBuilder.DropTable(
                name: "Policy");

            migrationBuilder.DropTable(
                name: "Encounter");

            migrationBuilder.DropTable(
                name: "Medication");

            migrationBuilder.DropTable(
                name: "Specialty");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "ClinicLocation");
        }
    }
}
