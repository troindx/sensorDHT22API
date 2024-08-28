using SensorDataAPI.Data;
using SensorDataAPI.Services;
using Microsoft.EntityFrameworkCore;
using Dht22Reader;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

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
        builder.Services.AddSingleton<Dht22Service>();
        builder.Services.AddHostedService<SensorDataBackgroundService>();

        // Configure CORS to allow all origins 
        // (in future it should only load from one IP but lets see)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        
        var app = builder.Build();
        app.UseCors("AllowAll");
        
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