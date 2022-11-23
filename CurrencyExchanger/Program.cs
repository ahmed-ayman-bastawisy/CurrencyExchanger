using CurrencyExchanger.Models;
using CurrencyExchanger.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Serilog;




var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//create the logger and setup your sinks, filters and properties
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.MinimumLevel.Debug()
    .WriteTo.Console().WriteTo.File(@$"Logs/ExchangeRateLog-{DateTime.Now.Date.ToString("dd-MM-yyyy")}.txt"
    , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}"
    , rollingInterval: RollingInterval.Day,
    rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000000);
});

// Add services to the container.
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ExchangeServiceDBContext>(opt => opt.UseSqlServer(configuration.GetSection("ConnectionString").Value));
builder.Services.AddScoped<IExchangeServiceRepo, ExchangeServiceRepo>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();
builder.Services.AddHttpClient("ExchangeRatesAPI", client => {
    //add Rate Provider URI
    client.BaseAddress = new Uri("https://api.apilayer.com/exchangerates_data");
    //add Rate Provider API key to the default header
    client.DefaultRequestHeaders.Add("apikey", "gTvO8ffdlaBNtTOzl2r6NECHPbgFExGL");
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Auto Apply Migrations
//using (var scope = app.Services.CreateScope())
//{
//    var dataContext = scope.ServiceProvider.GetRequiredService<ExchangeServiceDBContext>();
//    try
//    {
//        dataContext.Database.Migrate();

//    }
//    catch (Exception)
//    {

//    }
//} 
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
