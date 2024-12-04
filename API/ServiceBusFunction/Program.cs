using Base.Data.Services;
using Base.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<IntakerDbContext>(options =>
           {
               options.UseSqlServer(context.Configuration.GetConnectionString(options.Options.ContextType.Name),
                               o => o.MigrationsAssembly(options.Options.ContextType.GetTypeInfo().Assembly.FullName));
           });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IntakerTaskService>();
    })
    .Build();

host.Run();