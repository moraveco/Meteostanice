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

            var totalRecords = await db.MeteoRecords.CountAsync();
            var onlineCount = await db.MeteoRecords.CountAsync(r => r.IsOnline);
            var latest = records.FirstOrDefault();
            var lastSeenText = latest != null
                ? $"{latest.FetchedAt:yyyy-MM-dd HH:mm:ss}"
                : "N/A";

            var rows = string.Join("\n", records.Select(r => $"""
                <tr>
                    <td>{r.FetchedAt:yyyy-MM-dd HH:mm:ss}</td>
                    <td>
                        <span class="badge {(r.IsOnline ? "bg-success" : "bg-danger")}">
                            {(r.IsOnline ? "Online" : "Offline")}
                        </span>
                    </td>
                    <td><pre class="mb-0 small">{r.JsonData ?? "No data"}</pre></td>
                </tr>
            """));

            var html = $"""
                <!DOCTYPE html>
                <html lang="cs">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <meta http-equiv="refresh" content="60">
                    <title>MeteoCollector Dashboard</title>
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet">
                </head>
                <body class="bg-light">
                    <nav class="navbar navbar-dark bg-primary mb-4">
                        <div class="container">
                            <span class="navbar-brand fw-bold">
                                <i class="bi bi-cloud-sun me-2"></i>MeteoCollector
                            </span>
                            <span class="text-white-50 small">Auto-refresh every 60s</span>
                        </div>
                    </nav>

                    <div class="container">
                        <div class="row g-3 mb-4">
                            <div class="col-md-4">
                                <div class="card border-0 shadow-sm">
                                    <div class="card-body">
                                        <div class="text-muted small mb-1"><i class="bi bi-database me-1"></i>Total Records</div>
                                        <div class="fs-3 fw-bold text-primary">{totalRecords}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="card border-0 shadow-sm">
                                    <div class="card-body">
                                        <div class="text-muted small mb-1"><i class="bi bi-wifi me-1"></i>Successful Fetches</div>
                                        <div class="fs-3 fw-bold text-success">{onlineCount}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="card border-0 shadow-sm">
                                    <div class="card-body">
                                        <div class="text-muted small mb-1"><i class="bi bi-clock me-1"></i>Last Fetch</div>
                                        <div class="fs-6 fw-bold text-dark">{lastSeenText}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card border-0 shadow-sm">
                            <div class="card-header bg-white d-flex justify-content-between align-items-center">
                                <h5 class="mb-0"><i class="bi bi-table me-2"></i>Last 20 Records</h5>
                                <span class="badge bg-secondary">Live</span>
                            </div>
                            <div class="card-body p-0">
                                <div class="table-responsive">
                                    <table class="table table-hover mb-0">
                                        <thead class="table-light">
                                            <tr>
                                                <th>Fetched At</th>
                                                <th>Status</th>
                                                <th>Data</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {rows}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            """;

            return Results.Content(html, "text/html");
        });
    }
}