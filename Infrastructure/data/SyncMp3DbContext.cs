using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<DomainUser> DomainUsers { get; set; }
    public DbSet<NetWork> Networks { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<RequestedSong> RequestedSongs { get; set; }
    public DbSet<DownloadedSong> DownloadedSongs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPT inheritance for RequestedSong
        modelBuilder.Entity<Song>().ToTable("Songs");
        modelBuilder.Entity<RequestedSong>().ToTable("RequestedSongs");

        // Network -> Owner (one of the DomainUsers)
        modelBuilder.Entity<NetWork>()
            .HasOne(n => n.OwnerNavigation)
            .WithMany()
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Network -> Users (one-to-many)
        modelBuilder.Entity<NetWork>()
            .HasMany(n => n.Users)
            .WithOne(u => u.NetworkNavigation)
            .HasForeignKey(u => u.NetworkId);

        // Network -> Songs
        modelBuilder.Entity<Song>()
            .HasOne(s => s.NetworkNavigation)
            .WithMany(n => n.NetworkSongs)
            .HasForeignKey(s => s.NetworkId);

        // Song -> DownloadedSong (one-to-one)
        modelBuilder.Entity<Song>()
            .HasOne(s => s.DownloadedSongNavigation)
            .WithOne(ds => ds.SongNavigation)
            .HasForeignKey<DownloadedSong>(ds => ds.SongId)
            .OnDelete(DeleteBehavior.NoAction);

        // DownloadedSong -> Network
        modelBuilder.Entity<DownloadedSong>()
            .HasOne(ds => ds.NetWorkNavigation)
            .WithMany(n => n.DownloadedSongs)
            .HasForeignKey(ds => ds.NetworkId);

        // Song -> DownloadedBy (many-to-many with DomainUser)
        modelBuilder.Entity<Song>()
            .HasMany(s => s.DownloadedBy)
            .WithMany(u => u.CurrentSongs);

        // RequestedSong -> RequestedBy (many-to-many with DomainUser)
        modelBuilder.Entity<RequestedSong>()
            .HasMany(rs => rs.RequestedBy)
            .WithMany(u => u.RequestedSongs);
    }
}