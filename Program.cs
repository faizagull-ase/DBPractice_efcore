using ClinicFlowDemo;
using ClinicFlowDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Binds the "ConnectionStrings" section of appsettings.json into ConnectionStringsOptions,
// made available anywhere as IOptions<ConnectionStringsOptions> via DI.
builder.Services.Configure<ConnectionStringsOptions>(
    builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddDbContext<ClinicFlowDbContext>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ClinicFlowDbContext>();

Console.WriteLine("Applying migrations...");
db.Database.Migrate();
Console.WriteLine("Database is up to date.");

SeedData.Seed(db);
Console.WriteLine("Seed data inserted.");

Console.WriteLine("\nGenerating bulk data...");
BulkSeedData.Generate(db);
Console.WriteLine("Bulk data generation complete.");

// ---- Test query 1: patients with their primary policy ----
Console.WriteLine("\n-- Patients with primary policy --");
var primaryPolicies = db.PatientPolicies
    .Where(pp => pp.IsPrimary)
    .Include(pp => pp.Patient)
    .Include(pp => pp.Policy)
    .Select(pp => new { pp.Patient.FirstName, pp.Patient.LastName, pp.Policy.PayerName, pp.Policy.PlanName })
    .ToList();
foreach (var row in primaryPolicies)
    Console.WriteLine($"  {row.FirstName} {row.LastName} -> {row.PayerName} / {row.PlanName}");

// ---- Test query 2: appointment counts by status ----
Console.WriteLine("\n-- Appointment counts by status --");
var statusCounts = db.Appointments
    .GroupBy(a => a.Status)
    .Select(g => new { Status = g.Key, Count = g.Count() })
    .OrderBy(x => x.Status)
    .ToList();
foreach (var row in statusCounts)
    Console.WriteLine($"  status {row.Status}: {row.Count}");

// ---- Test query 3: confirm the new unique constraint is actually enforced ----
Console.WriteLine("\n-- Testing UQ_Appointment_Provider_Datetime --");
var existing = db.Appointments.First();
db.Appointments.Add(new Appointment
{
    PatientId = existing.PatientId,
    ProviderId = existing.ProviderId,
    RoomId = existing.RoomId,
    AppointmentDatetime = existing.AppointmentDatetime, // exact duplicate on purpose
    Reason = "Duplicate test",
    Status = AppointmentStatus.Scheduled
});
try
{
    db.SaveChanges();
    Console.WriteLine("  UNEXPECTED: duplicate insert succeeded — constraint is NOT enforced.");
}
catch (DbUpdateException)
{
    Console.WriteLine("  Correctly rejected: constraint is enforced.");
    db.ChangeTracker.Clear();
}
