namespace Meteostanice.Data;

using Microsoft.EntityFrameworkCore;

public class MeteoDbContext(DbContextOptions<MeteoDbContext> options) : DbContext(options)
{
    public DbSet<MeteoRecord> MeteoRecords { get; set; }
}