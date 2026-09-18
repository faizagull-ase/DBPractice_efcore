using ClinicFlowDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlowDemo;

// Stage 5 queries (screens 4 + 6), implemented as LINQ instead of raw SQL —
// mirrors the same 4 queries built against ClinicApp on the raw-SQL side.
// No .Include(...) calls here: every query ends in a Select(...) projecting
// into an anonymous type, so EF Core auto-generates the necessary joins from
// the navigation properties referenced inside Where/GroupBy/Select directly —
// Include only matters when the result is real tracked entities, not a projection.
public static class Queries
{
    // Screen 4: daily schedule — all appointments for one provider on one day
    public static void DailySchedule(ClinicFlowDbContext db, int providerId, DateTime date)
    {
        var nextDay = date.Date.AddDays(1);

        var results = db.Appointments
            .Where(a => a.ProviderId == providerId
                     && a.AppointmentDatetime >= date.Date
                     && a.AppointmentDatetime < nextDay)
            .OrderBy(a => a.AppointmentDatetime)
            .Select(a => new
            {
                a.AppointmentId,
                a.AppointmentDatetime,
                a.Reason,
                a.Status,
                a.Patient.FirstName,
                a.Patient.LastName,
                RoomName = a.Room.Name
            })
            .ToList();

        Console.WriteLine($"\n-- Daily schedule: provider {providerId} on {date:yyyy-MM-dd} --");
        foreach (var r in results)
            Console.WriteLine($"  {r.AppointmentDatetime:t}  {r.FirstName} {r.LastName}  [{r.Status}]  Room {r.RoomName}  \"{r.Reason}\"");
        Console.WriteLine($"  ({results.Count} appointments)");
    }

    // Report: appointments per provider per month (top 15 shown)
    public static void AppointmentsPerProviderPerMonth(ClinicFlowDbContext db)
    {
        var results = db.Appointments
            .GroupBy(a => new { a.ProviderId, a.Provider.Name, a.AppointmentDatetime.Year, a.AppointmentDatetime.Month })
            .Select(g => new { g.Key.ProviderId, g.Key.Name, g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(15)
            .ToList();

        Console.WriteLine("\n-- Appointments per provider per month (top 15) --");
        foreach (var r in results)
            Console.WriteLine($"  {r.Name}  {r.Year}-{r.Month:00}: {r.Count}");
    }

    // Report: no-show rate by clinic
    public static void NoShowRateByClinic(ClinicFlowDbContext db)
    {
        var results = db.Appointments
            .GroupBy(a => new { a.Room.ClinicLocation.LocationId, a.Room.ClinicLocation.Name })
            .Select(g => new
            {
                g.Key.LocationId,
                g.Key.Name,
                Total = g.Count(),
                NoShows = g.Count(a => a.Status == AppointmentStatus.NoShow)
            })
            .OrderByDescending(x => (decimal)x.NoShows / x.Total)
            .ToList();

        Console.WriteLine("\n-- No-show rate by clinic --");
        foreach (var r in results)
        {
            var rate = r.Total == 0 ? 0 : (decimal)r.NoShows / r.Total * 100;
            Console.WriteLine($"  {r.Name}: {r.NoShows}/{r.Total} = {rate:0.00}%");
        }
    }

    // Report: most frequent diagnoses in a given quarter
    public static void MostFrequentDiagnosesInQuarter(ClinicFlowDbContext db, DateOnly quarterStart, DateOnly quarterEnd)
    {
        var results = db.EncounterDiagnoses
            .Where(ed => ed.Encounter.EncounterDate >= quarterStart && ed.Encounter.EncounterDate < quarterEnd)
            .GroupBy(ed => new { ed.DiagnosisCodeValue, ed.DiagnosisCode.Description })
            .Select(g => new { g.Key.DiagnosisCodeValue, g.Key.Description, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .ToList();

        Console.WriteLine($"\n-- Most frequent diagnoses: {quarterStart} to {quarterEnd} (top 3) --");
        foreach (var r in results)
            Console.WriteLine($"  {r.DiagnosisCodeValue} ({r.Description}): {r.Count}");
    }
}
