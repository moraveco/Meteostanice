using Meteostanice.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Meteostanice.Api;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        app.MapGet("/health", async (MeteoDbContext db) =>
        {
            var latest = await db.MeteoRecords
                .OrderByDescending(r => r.FetchedAt)
                .FirstOrDefaultAsync();

            return Results.Ok(new
            {
                Status = "Running",
                LastFetch = latest?.FetchedAt,
                LastFetchOnline = latest?.IsOnline,
                TotalRecords = await db.MeteoRecords.CountAsync()
            });
        });

        app.MapGet("/dashboard", async (MeteoDbContext db) =>
        {
            var records = await db.MeteoRecords
                .OrderByDescending(r => r.FetchedAt)
                .Take(20)
                .ToListAsync();

            var rows = string.Join("\n", records.Select(r => $"""
                <tr>
                    <td>{r.FetchedAt:yyyy-MM-dd HH:mm:ss}</td>
                    <td>{(r.IsOnline ? "✅ Online" : "❌ Offline")}</td>
                    <td><pre>{r.JsonData ?? "No data"}</pre></td>
                </tr>
            """));

            var css = """
                          body { font-family: sans-serif; padding: 2rem; background: #f5f5f5; }
                          h1 { color: #333; }
                          table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
                          th { background: #2196F3; color: white; padding: 12px; text-align: left; }
                          td { padding: 10px 12px; border-bottom: 1px solid #eee; vertical-align: top; }
                          pre { margin: 0; font-size: 0.8rem; max-height: 150px; overflow-y: auto; }
                          tr:hover { background: #f9f9f9; }
                      """;

            var html = $"""
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <title>MeteoCollector Dashboard</title>
                                <meta charset="utf-8">
                                <meta http-equiv="refresh" content="60">
                                <style>{css}</style>
                            </head>
                            {rows}
                        """;

            return Results.Content(html, "text/html");
        });
    }
}