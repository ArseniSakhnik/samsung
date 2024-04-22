using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.Gans;
using Mlsat.Models.Entities.Models.IsolationForests;
using Mlsat.Models.Entities.Models.Knns;
using Mlsat.Models.Entities.Models.Lofs;
using Mlsat.Models.Entities.Models.SiameseAutoencoders;
using Mlsat.Models.Entities.Models.SimpleAutoencoders;
using Mlsat.Models.Entities.Projects;
using Mlsat.Models.Entities.SpaceWeather;

namespace Mlsat.Services;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<DataSource> DataSources { get; set; } = default!;
    public DbSet<Model> Models { get; set; } = default!;
    public DbSet<Ap> Aps { get; set; } = default!;
    public DbSet<Kp> Kps { get; set; } = default!;
    public DbSet<Dst> Dsts { get; set; } = default!;
    public DbSet<Wolf> Wolfs { get; set; } = default!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ap>().HasKey(s => s.Date);
        modelBuilder.Entity<Kp>().HasKey(s => s.Date);
        modelBuilder.Entity<Dst>().HasKey(s => s.Date);
        modelBuilder.Entity<Wolf>().HasKey(s => s.Date);


        base.OnModelCreating(modelBuilder);
    }
}