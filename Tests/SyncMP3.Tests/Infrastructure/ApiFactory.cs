using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SyncMP3.Web;

public class ApiFactory : WebApplicationFactory<Program>
{
    public string DbName { get; } = Guid.NewGuid().ToString();
    public const string JwtKey = "a9390acc-c1ea-446d-ab55-0be8a17f8f033e52275d-2d0c-419f-987e-8e99fa5e7efc";
    public const string JwtIssuer = "SyncMP3.Tests";
    public const string JwtAudience = "SyncMP3.Tests";

    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience
            });
        });
        builder.ConfigureServices(services =>
        {
            // Remove the built options
            services.RemoveAll<DbContextOptions<SyncMp3DbContext>>();

            // Remove the underlying configure-callback registration(s) — this is the one that was missing
            services.RemoveAll<IDbContextOptionsConfiguration<SyncMp3DbContext>>();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<SyncMp3DbContext>(options =>
                options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();
            db.Database.EnsureCreated();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
        await base.DisposeAsync();
    }
}