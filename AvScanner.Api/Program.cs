using Microsoft.Extensions.Configuration;
using AvScanner.IoC;
using Microsoft.AspNetCore.Http.Features;
using AvScanner.Application.Models;

string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables()
   .Build();

var maxFileSizeBytes = configuration.GetValue<int>("Settings:MaxFileSizeBytes");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxFileSizeBytes; // Set your desired max size in bytes
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(30);
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScanningServices(configuration);
builder.Services.AddApplicationInsightsTelemetry(t => t.ConnectionString = configuration["ApplicationInsights:ConnectionString"]);


builder.Services.Configure<FormOptions>(options =>
{

    
    options.MultipartBodyLengthLimit = maxFileSizeBytes; // 30 MB
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


 app.MapControllers();


app.Run();
