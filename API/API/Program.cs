
using API.Services;
using Base.Data;
using Base.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Polly;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureClients(b =>
{
    var connectionString = builder.Configuration.GetConnectionString("ServiceBus");
    b.AddServiceBusClient(connectionString);
});

builder.Services.AddDbContext<IntakerDbContext>(options =>
           {
               options.UseSqlServer(builder.Configuration.GetConnectionString(options.Options.ContextType.Name),
                               o => o.MigrationsAssembly(options.Options.ContextType.GetTypeInfo().Assembly.FullName));
           });

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IntakerTaskService>();
builder.Services.AddSingleton<IAsyncPolicy>(serviceProvider =>
       {
           var logger = serviceProvider.GetRequiredService<ILogger<ServiceBusHandler>>();
           return Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                   onRetry: (exception, timeSpan, retryCount, context) =>
                   {
                       logger.LogWarning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                   });
       });
builder.Services.AddSingleton<IServiceBusHandler, ServiceBusHandler>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=Home}/{action=Index}/{id?}");

app.Run();
