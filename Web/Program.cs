using Microsoft.EntityFrameworkCore;

namespace SyncMP3.Web
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<CheckUserExistsMiddleware>();
            TestMapping.ConfigureEndpoints(app);

            app.Run();
        }
    }
}