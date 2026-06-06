namespace Meteostanice.Data;

using Microsoft.EntityFrameworkCore;

public class MeteoDbContext : DbContext
{
    public MeteoDbContext(DbContextOptions<MeteoDbContext> options) : base(options) { }

    public DbSet<MeteoRecord> MeteoRecords { get; set; }
}