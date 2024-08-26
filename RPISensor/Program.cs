using SensorDataAPI.Data;
using SensorDataAPI.Services;
using Microsoft.EntityFrameworkCore;
using Dht22Reader;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.Configure<Dht22Settings>(builder.Configuration.GetSection("Dht22Settings"));
        // Configure MariaDB connection
        var connectionString = builder.Configuration.GetConnectionString("MariaDBConnection");
        builder.Services.AddDbContext<SensorDataContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();

         // Register the background service
        builder.Services.AddSingleton<Dht22Service>(); // Ensure this is set up correctly with dependencies
        builder.Services.AddHostedService<SensorDataBackgroundService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}