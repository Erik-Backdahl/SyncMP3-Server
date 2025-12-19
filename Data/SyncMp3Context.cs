using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SyncMP3.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace SyncMP3.Data;

public partial class SyncMp3Context : DbContext
{
    public SyncMp3Context()
    {
    }

    public SyncMp3Context(DbContextOptions<SyncMp3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<CurrentNetworkRegisteredSong> CurrentNetworkRegisteredSongs { get; set; }

    public virtual DbSet<CurrentUserRegisteredSong> CurrentUserRegisteredSongs { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Network> Networks { get; set; }

    public virtual DbSet<NetworkPassword> NetworkPasswords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

        string connectionString = config.GetConnectionString("DefaultConnection")!;

        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrentNetworkRegisteredSong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CurrentN__3214EC2705B22ACC");

            entity.HasIndex(e => e.SongGuid, "UQ__CurrentN__A6446AE113BB70E4").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OriginUuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("OriginUUID");
            entity.Property(e => e.SongGuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("SongGUID");
            entity.Property(e => e.SongName).HasMaxLength(250);

            entity.HasOne(d => d.OriginUu).WithMany(p => p.CurrentNetworkRegisteredSongs)
                .HasPrincipalKey(p => p.UserUuid)
                .HasForeignKey(d => d.OriginUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NetworkHistory_Users_UserUUID");
        });

        modelBuilder.Entity<CurrentUserRegisteredSong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CurrentU__3214EC27DD0CD608");

            entity.HasIndex(e => e.SongGuid, "UQ__CurrentU__A6446AE1027D7A99").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OriginUuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("OriginUUID");
            entity.Property(e => e.SongGuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("SongGUID");
            entity.Property(e => e.SongName).HasMaxLength(250);

            entity.HasOne(d => d.OriginUu).WithMany(p => p.CurrentUserRegisteredSongs)
                .HasPrincipalKey(p => p.UserUuid)
                .HasForeignKey(d => d.OriginUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserHistory_Users_UserUUID");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC27834D1F0D");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Message");
            entity.Property(e => e.MessageType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserUuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserUUID");

            entity.HasOne(d => d.UserUu).WithMany(p => p.Messages)
                .HasPrincipalKey(p => p.UserUuid)
                .HasForeignKey(d => d.UserUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Messages_Users_UserUUID");
        });

        modelBuilder.Entity<Network>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Networks__3214EC274F5B41CE");

            entity.HasIndex(e => e.NetworkGuid, "UQ__Networks__180B0B55373BFB8B").IsUnique();

            entity.HasIndex(e => e.MasterUser, "UQ__Networks__54A5BCCCF9F5632B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MasterUser)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.MaxUsers).HasDefaultValue(2);
            entity.Property(e => e.NetworkGuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NetworkGUID");
        });

        modelBuilder.Entity<NetworkPassword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NetworkP__3214EC27F70C267F");

            entity.HasIndex(e => e.NetworkGuid, "UQ__NetworkP__180B0B5597F6EE61").IsUnique();

            entity.HasIndex(e => e.CreatedBy, "UQ__NetworkP__655D054FFA381A3D").IsUnique();

            entity.HasIndex(e => e.Password, "UQ__NetworkP__87909B152E4F5401").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.NetworkGuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NetworkGUID");
            entity.Property(e => e.Password)
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithOne(p => p.NetworkPassword)
                .HasPrincipalKey<User>(p => p.UserUuid)
                .HasForeignKey<NetworkPassword>(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NetworkPasswords_Users_UserUUID");

            entity.HasOne(d => d.Network).WithOne(p => p.NetworkPassword)
                .HasPrincipalKey<Network>(p => p.NetworkGuid)
                .HasForeignKey<NetworkPassword>(d => d.NetworkGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NetworkPasswords_Networks_NetworkGUID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27E13942DC");

            entity.HasIndex(e => e.UserUuid, "UQ__Users__328A96E440CF6DED").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATED");
            entity.Property(e => e.NetworkGuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NetworkGUID");
            entity.Property(e => e.Permium).HasDefaultValue(false);
            entity.Property(e => e.UserUuid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserUUID");

            entity.HasOne(d => d.Network).WithMany(p => p.Users)
                .HasPrincipalKey(p => p.NetworkGuid)
                .HasForeignKey(d => d.NetworkGuid)
                .HasConstraintName("fk_Users_Networks_NetworkGUID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
