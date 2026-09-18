# ClinicFlowDemo

EF Core implementation of the ClinicFlow schema (DB Lab — Clinic Appointments) — the
"map your final schema with EF" wrap-up deliverable. Mirrors the same 15-table design
built with raw SQL (`schema.sql` / `seed.sql`, in the `ClinicApp` database), but here
the schema, seed data, and bulk data generation are all driven through EF Core
migrations and C# instead of T-SQL scripts.

## Prerequisites

- .NET SDK 10.0+
- A local SQL Server instance (this project targets `Server=localhost` — a
  Windows/trusted-auth connection to the default instance, not a named instance
  like `SQLEXPRESS`)
- The `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef`

## Configuration

The connection string lives in `appsettings.json`, not hardcoded anywhere:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change `DefaultConnection` here if you need to point at a different server or
database name — nothing else needs editing.

## How to run it, end to end

```bash
dotnet build
dotnet run
```

A single `dotnet run` does everything:
1. **Applies any pending EF Core migrations** (`db.Database.Migrate()`), creating
   `ClinicFlowDb` from scratch if it doesn't exist yet.
2. **Seeds the hand-crafted scenario** (`SeedData.cs`) — 10-entry catalogs
   (Specialty/ClinicLocation/DiagnosisCode/Medication/Policy, matching `ClinicApp`'s
   `seed.sql`), plus 2 patients, 2 providers, and 4 appointments covering every
   status, exercising every table/relationship. Skips automatically if already seeded.
3. **Generates bulk data** (`BulkSeedData.cs`) — grows to ~1,000 patients and
   ~50 providers, then generates 50,000 appointments (with a realistic
   60/20/10/5/5 completed/scheduled/cancelled/no-show/checked-in split) and the
   full downstream chain of encounters, diagnoses, and prescriptions for every
   completed appointment. Also skips automatically if already generated.
4. **Runs 3 verification queries** — patients with their primary policy,
   appointment counts by status, and a deliberate duplicate-insert test proving
   the `UQ_Appointment_Provider_Datetime` constraint is actually enforced, not
   just present in the schema.

### Resetting to a clean state

Both seed steps detect existing data and skip themselves, so re-running
`dotnet run` on an already-populated database is a no-op past step 1. To start
completely fresh:

```bash
dotnet ef database drop --force
dotnet run
```

## Project structure

| Path | Purpose |
|---|---|
| `Models/` | 15 entity classes (one per table), plus `Gender.cs`/`AppointmentStatus.cs` enums |
| `ClinicFlowDbContext.cs` | All Fluent API configuration — keys, FKs, unique/filtered indexes, delete behaviors |
| `ConnectionStringsOptions.cs` | Strongly-typed binding target for the `ConnectionStrings` config section |
| `DesignTimeDbContextFactory.cs` | Lets `dotnet ef` CLI commands construct the context (they can't use the app's DI container) |
| `Migrations/` | Generated migration history |
| `SeedData.cs` | Hand-crafted Stage 3-equivalent scenario data |
| `BulkSeedData.cs` | Stage 4-equivalent bulk generation, batched for EF Core's change tracker |
| `Program.cs` | Entry point — builds the DI host, migrates, seeds, and runs the verification queries |
| `appsettings.json` | Connection string configuration |

## Key design notes

- **Connection string via `IOptions<T>`, not hardcoded** — `ClinicFlowDbContext`
  takes `IOptions<ConnectionStringsOptions>` through its constructor, resolved via
  DI at runtime (`Program.cs`) and manually via `DesignTimeDbContextFactory` for
  CLI tooling, which never runs `Program.cs` at all.
- **`Gender`/`AppointmentStatus` are real C# enums**, not raw ints — stored as
  their underlying `int` value in the database (matching the raw-SQL side's
  plain `INT` columns with no `CHECK` constraint), but with real compile-time
  type safety in C#.
- **`UQ_Appointment_Provider_Datetime`** (unique on `ProviderId` +
  `AppointmentDatetime`) prevents double-booking the same provider at the same
  instant — added after the initial schema via a second migration, matching a
  constraint added on the raw-SQL side during Stage 5 indexing work.
- **Delete behaviors are deliberately asymmetric**: `Cascade` toward
  junction/owned rows that are meaningless without their parent (e.g.
  `PatientPolicy` when its `Patient` is deleted), `Restrict` toward shared
  catalog data (e.g. `Policy`, `Specialty`, `DiagnosisCode`) so it can't be
  silently wiped out by an unrelated delete.
- **Bulk generation avoids EF Core's change-tracking overhead** by disabling
  `AutoDetectChangesEnabled`, batching `SaveChanges()` calls (5,000 rows at a
  time), calling `ChangeTracker.Clear()` between batches, and setting foreign
  keys as plain scalar ints instead of via navigation properties.
