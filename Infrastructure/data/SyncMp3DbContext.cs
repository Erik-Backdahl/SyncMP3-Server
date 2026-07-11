using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SyncMp3DbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<DomainUser> DomainUsers { get; set; }
    public DbSet<Network> Networks { get; set; }
    public DbSet<NetworkKey> NetworkKeys { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongRequest> SongRequests { get; set; }
    public DbSet<DownloadedSong> DownloadedSongs { get; set; }

    public SyncMp3DbContext(DbContextOptions<SyncMp3DbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------------- Network ----------------

        // Network -> Owner (DomainUser)
        modelBuilder.Entity<Network>()
            .HasOne(n => n.OwnerNavigation)
            .WithMany()
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Network -> Users (one-to-many)
        modelBuilder.Entity<Network>()
            .HasMany(n => n.Users)
            .WithOne(u => u.NetworkNavigation)
            .HasForeignKey(u => u.NetworkId)
            .OnDelete(DeleteBehavior.Restrict);

        // Network -> NetworkSongs (one-to-many)
        modelBuilder.Entity<Network>()
            .HasMany(n => n.NetworkSongs)
            .WithOne(s => s.NetworkNavigation)
            .HasForeignKey(s => s.NetworkId)
            .OnDelete(DeleteBehavior.Cascade);

        // Network -> DownloadedSongs (one-to-many)
        modelBuilder.Entity<Network>()
            .HasMany(n => n.DownloadedSongs)
            .WithOne(ds => ds.NetworkNavigation)
            .HasForeignKey(ds => ds.NetworkId)
            .OnDelete(DeleteBehavior.Restrict);

        // Network -> NetworkKeys (one-to-many)
        modelBuilder.Entity<Network>()
            .HasMany(n => n.NetworkKeys)
            .WithOne(k => k.NetworkNavigation)
            .HasForeignKey(k => k.NetworkId)
            .OnDelete(DeleteBehavior.Cascade);

        // Network -> SongRequests (one-to-many)
        modelBuilder.Entity<Network>()
            .HasMany(n => n.SongRequests)
            .WithOne(sr => sr.NetworkNavigation)
            .HasForeignKey(sr => sr.NetworkId)
            .OnDelete(DeleteBehavior.NoAction);

        // ---------------- Song ----------------

        // Song -> DownloadedSong (one-to-one, FK on DownloadedSong)
        modelBuilder.Entity<Song>()
            .HasOne(s => s.DownloadedSong)
            .WithOne(ds => ds.SongNavigation)
            .HasForeignKey<DownloadedSong>(ds => ds.SongId)
            .OnDelete(DeleteBehavior.NoAction);

        // Song -> SongRequests (one-to-many)
        modelBuilder.Entity<Song>()
            .HasMany(s => s.SongRequests)
            .WithOne(sr => sr.SongNavigation)
            .HasForeignKey(sr => sr.SongId)
            .OnDelete(DeleteBehavior.NoAction);

        // Song <-> DomainUser (many-to-many, downloaded songs per user)
        modelBuilder.Entity<Song>()
            .HasMany(s => s.DownloadedBy)
            .WithMany(u => u.LocalSongs)
            .UsingEntity(j => j.ToTable("UserDownloadedSongs"));

        // ---------------- SongRequest ----------------

        // SongRequest -> DomainUser (RequestedBy)
        modelBuilder.Entity<SongRequest>()
            .HasOne(sr => sr.RequestedByNavigation)
            .WithMany(u => u.SongRequests)
            .HasForeignKey(sr => sr.RequestedById)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------------- DownloadedSong ----------------

        modelBuilder.Entity<DownloadedSong>()
            .HasOne(ds => ds.UploadedByNavigation)
            .WithMany()
            .HasForeignKey(ds => ds.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DownloadedSong>()
        .HasIndex(d => new { d.SongId, d.NetworkId })
        .IsUnique();


        // DownloadedSong -> UploadedBy is a bare Guid (no navigation property),
        // so no relationship is configured for it. Leave as a plain scalar
        // unless you intend it to be an FK to DomainUser.
        // ----------------- NetworkKey ---------_------

        modelBuilder.Entity<NetworkKey>()
            .HasIndex(k => k.Code);
    }
}