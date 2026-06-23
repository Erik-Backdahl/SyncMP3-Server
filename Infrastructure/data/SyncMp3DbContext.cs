using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SyncMp3DbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<DomainUser> DomainUsers { get; set; }
    public DbSet<NetWork> Networks { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongRequest> SongRequests { get; set; }
    public DbSet<DownloadedSong> DownloadedSongs { get; set; }

    public SyncMp3DbContext(DbContextOptions<SyncMp3DbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Network -> Owner
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

        // Network -> Songs (one-to-many)
        modelBuilder.Entity<Song>()
            .HasOne(s => s.NetworkNavigation)
            .WithMany(n => n.NetworkSongs)
            .HasForeignKey(s => s.NetworkId);

        // Song -> DownloadedSong (one-to-one, FK on DownloadedSong)
        modelBuilder.Entity<Song>()
            .HasOne(s => s.DownloadedSong)
            .WithOne(ds => ds.SongNavigation)
            .HasForeignKey<DownloadedSong>(ds => ds.SongId)
            .OnDelete(DeleteBehavior.NoAction);

        // DownloadedSong -> Network (one-to-many)
        modelBuilder.Entity<DownloadedSong>()
            .HasOne(ds => ds.NetWorkNavigation)
            .WithMany(n => n.DownloadedSongs)
            .HasForeignKey(ds => ds.NetworkId);

        // SongRequest -> Song
        modelBuilder.Entity<SongRequest>()
            .HasOne(sr => sr.SongNavigation)
            .WithMany(s => s.SongRequests)
            .HasForeignKey(sr => sr.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        // SongRequest -> DomainUser
        modelBuilder.Entity<SongRequest>()
            .HasOne(sr => sr.RequestedByNavigation)
            .WithMany(u => u.SongRequests)
            .HasForeignKey(sr => sr.RequestedById)
            .OnDelete(DeleteBehavior.Cascade);

        // DomainUser <-> Song (many-to-many, user's downloaded songs)
        modelBuilder.Entity<DomainUser>()
            .HasMany(u => u.DownloadedSongs)
            .WithMany(s => s.DownloadedBy)
            .UsingEntity(j => j.ToTable("UserDownloadedSongs"));
    }
}